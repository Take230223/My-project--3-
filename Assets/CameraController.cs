using UnityEngine;
using TMPro; // 速度表示用

public class CameraController : MonoBehaviour
{
    [Header("ターゲット（車）")]
    public Transform target;
    public Rigidbody targetRigidbody;

    [Header("速度表示UI")]
    public TextMeshProUGUI speedText;

    [Header("視点1: 後方追従の設定")]
    public float distance = 6.0f;
    public float height = 2.0f;
    public float rotationDamping = 3.0f;
    public float heightDamping = 2.0f;

    [Header("視点2: マウス見回しの設定")]
    public float mouseSensitivity = 3.0f;
    public float minVerticalAngle = -10f;
    public float maxVerticalAngle = 60f;

    private bool isLookAroundMode = false; // false = 追従, true = 見回し
    private float mouseX = 0f;
    private float mouseY = 0f;

    void Start()
    {
        // マウスカーソルをゲーム画面内にロック（Escキーで解除）
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Cキーでカメラモードを切り替え
        if (Input.GetKeyDown(KeyCode.C))
        {
            isLookAroundMode = !isLookAroundMode;
            
            // 見回しモードに入る瞬間に、現在のカメラの向きを初期値にする
            if (isLookAroundMode)
            {
                mouseX = target.eulerAngles.y;
                mouseY = 15f; 
            }
        }

        // 速度の表示処理
        if (targetRigidbody != null && speedText != null)
        {
            // 秒速(m/s)から時速(km/h)に変換
            float speedKmH = targetRigidbody.linearVelocity.magnitude * 3.6f;
            speedText.text = "Speed: " + Mathf.RoundToInt(speedKmH) + " km/h";
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        if (!isLookAroundMode)
        {
            // 【視度1】常に後ろから追従するモード
            float wantedRotationAngle = target.eulerAngles.y;
            float wantedHeight = target.position.y + height;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            transform.position = target.position;
            transform.position -= currentRotation * Vector3.forward * distance;

            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
            transform.LookAt(target.position + Vector3.up * 1.0f);
        }
        else
        {
            // 【視点2】マウスカーソルの動きに合わせて車体を中心に回るモード
            mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            mouseY = Mathf.Clamp(mouseY, minVerticalAngle, maxVerticalAngle);

            Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
            Vector3 position = target.position - (rotation * Vector3.forward * distance) + (Vector3.up * 1.0f);

            transform.rotation = rotation;
            transform.position = position;
        }
    }
}