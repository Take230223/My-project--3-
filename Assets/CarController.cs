using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Wheel Colliders (物理タイヤ)")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;

    [Header("★車の性能設定 (インスペクターで調整)")]
    [Tooltip("加速のガツン度（大きくすると一瞬で最高速になります）")]
    public float acceleration = 10f; 
    
    [Tooltip("出せる最高速度 (km/h)")]
    public float maxSpeedKmH = 100f; 
    
    [Tooltip("ブレーキの強さ（大きくするとピタッと止まります）")]
    public float brakeForce = 5000f; 

    [Header("ハンドルの設定")]
    public float maxSteerAngle = 30f;

    private Rigidbody rb;
    private float currentSpeedKmH;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 重心を下げて走破性と安定性を劇的にアップさせる
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void FixedUpdate()
    {
        // キーボード入力（矢印キーまたはW/S/A/D）
        float moveInput = Input.GetAxis("Vertical");
        float steerInput = Input.GetAxis("Horizontal");

        // 現在の時速(km/h)を計算
        currentSpeedKmH = rb.linearVelocity.magnitude * 3.6f;

        // 1. ハンドル処理
        float steerAngle = steerInput * maxSteerAngle;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;

        // 2. アクセル＆最高速度制限のロジック
        if (Mathf.Abs(moveInput) > 0.1f && currentSpeedKmH < maxSpeedKmH)
        {
            // 最高速度未満のときだけ、設定した「acceleration」に応じたパワーをかける
            // 質量(Mass)を掛け合わせることで、車が重くても同じように加速できるようにしています
            float torque = moveInput * acceleration * (rb.mass / 4f);
            
            rearLeftWheel.motorTorque = torque;
            rearRightWheel.motorTorque = torque;
            
            // アクセルを踏んでいるときはブレーキを解除
            ApplyBrake(0f);
        }
        else
        {
            // アクセルを離しているか、最高速度に達したらモーターをニュートラルにする
            rearLeftWheel.motorTorque = 0f;
            rearRightWheel.motorTorque = 0f;
        }

        // 3. ブレーキ処理（スペースキー）
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyBrake(brakeForce);
        }
        else if (Mathf.Abs(moveInput) < 0.1f)
        {
            // アクセルもブレーキも押していないとき、少しだけ自然減速させる
            ApplyBrake(100f);
        }
    }

    // 4つのタイヤに一括でブレーキをかける
    void ApplyBrake(float force)
    {
        frontLeftWheel.brakeTorque = force;
        frontRightWheel.brakeTorque = force;
        rearLeftWheel.brakeTorque = force;
        rearRightWheel.brakeTorque = force;
    }
}