using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject bulletPrefab; // 飛ばしたい弾のプレハブ
    public float bulletSpeed = 30.0f; // 弾の速度

    void Update()
    {
        // マウスの左クリック（または特定のボタン）が押されたか
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // FirePointの位置と向きに、弾を生成
        GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);

        // 弾のRigidbodyを取得して、前方向（transform.forward）に力を加える
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
        }

        // 3秒後に自動的に弾を消去する（メモリ対策）
        Destroy(bullet, 3.0f);
    }
}