using UnityEngine;
using UnityEngine.UI; // Legacy UIのTextを使う場合

public class TurretController : MonoBehaviour
{
    [Header("追従と旋回設定")]
    public Transform carTransform; 
    public Vector3 offset = new Vector3(0, 1.5f, 0);
    public float rotationSpeed = 5.0f;

    [Header("砲身（上下可動用）")]
    public Transform barrelTransform; // Barrelオブジェクトをアサイン
    public float minElevation = -10f; // 俯角（下を向ける限界角度、マイナス値）
    public float maxElevation = 25f;  // 仰角（上を向ける限界角度）

    [Header("照準（UI）")]
    public Text distanceText; // Canvas内のTextをアサイン
    public float maxDistance = 500f; // 測定可能な最大距離

    private float currentElevation = 0f;

    void Update()
    {
        // 1. 車への追従
        if (carTransform != null)
        {
            transform.position = carTransform.position + carTransform.TransformDirection(offset);
        }

        // 2. マウス位置への旋回（左右）と、距離の測定
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // 画面中央からのレイ
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            targetPoint = hit.point;
            
            // 距離をテキストに表示（小数点第1位まで）
            if (distanceText != null)
            {
                distanceText.text = $"{hit.distance:F1}m";
            }
        }
        else
        {
            // 何も当たらない（空など）場合は最大射程の先をターゲットにする
            targetPoint = ray.GetPoint(maxDistance);

            // 測定不可表示
            if (distanceText != null)
            {
                distanceText.text = "-";
            }
        }

        // --- 左右旋回（砲塔：Turret） ---
        Vector3 targetDirection = targetPoint - transform.position;
        Vector3 horizontalDirection = new Vector3(targetDirection.x, 0, targetDirection.z);
        
        if (horizontalDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // --- 上下俯仰（砲身：Barrel） ---
        if (barrelTransform != null)
        {
            // 砲塔から見たターゲットのローカル方向を計算
            Vector3 localTarget = transform.InverseTransformPoint(targetPoint);
            
            // アークタンジェントを使って上下の角度（ラジアン -> 度）を計算
            float targetAngle = Mathf.Atan2(localTarget.y, localTarget.z) * Mathf.Rad2Deg;

            // 角度を設定された仰角・俯角の範囲内に制限（クランプ）
            currentElevation = Mathf.Clamp(targetAngle, minElevation, maxElevation);

            // 砲身のローカル回転（X軸回転）に適用
            barrelTransform.localRotation = Quaternion.Euler(-currentElevation, 0, 0);
        }
    }
}