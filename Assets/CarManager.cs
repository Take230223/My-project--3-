using UnityEngine;

public class CarManager : MonoBehaviour
{
    [Header("切り替える車のリスト")]
    public GameObject[] cars; // 車のゲームオブジェクト（Car1, Car2, Car3）
    
    [Header("カメラのコントローラー")]
    public CameraController cameraController; // 先ほど作ったカメラのスクリプト

    private int currentCarIndex = 0;

    void Start()
    {
        // ゲーム開始時は最初の車（1台目）だけを有効化する
        SelectCar(0);
    }

    void Update()
    {
        // 1, 2, 3 キーの入力を監視
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            SelectCar(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            SelectCar(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            SelectCar(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            SelectCar(3);
        }
    }

    void SelectCar(int index)
    {
        // 範囲外のインデックス指定や、リストが空の場合は処理しない
        if (index < 0 || index >= cars.Length || cars[index] == null) return;

        currentCarIndex = index;

        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i] == null) continue;

            // 選択された車だけをアクティブにし、それ以外は非アクティブ（一時停止）にする
            bool isActive = (i == currentCarIndex);
            
            // 完全に消してしまうと見えなくなるので、スクリプトと物理だけを止める
            var controller = cars[i].GetComponent<CarController>();
            var rb = cars[i].GetComponent<Rigidbody>();

            if (controller != null) controller.enabled = isActive;
            
            if (rb != null)
            {
                // 操作していない車は勝手に動かないように物理を眠らせる
                if (!isActive)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true; 
                }
                else
                {
                    rb.isKinematic = false;
                }
            }
        }

        // カメラの追従対象を、新しく選んだ車に切り替える
        if (cameraController != null)
        {
            GameObject activeCar = cars[currentCarIndex];
            cameraController.target = activeCar.transform;
            cameraController.targetRigidbody = activeCar.GetComponent<Rigidbody>();
        }
    }
}