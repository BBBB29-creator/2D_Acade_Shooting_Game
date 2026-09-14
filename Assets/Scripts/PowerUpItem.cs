using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 2f;

    private void Update()
    {
        // 아이템 아래로 스르륵 하강
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime, Space.World);

        if (transform.position.y < -6.5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerController>(out var player))
            {
                // 빗장 해제 완료: 이제 아이템을 먹으면 플레이어 무기가 진화하거나 만렙 시 1000점을 퍼줍니다!
                player.UpgradeWeapon();
                Debug.Log("🎉 파워업 아이템 획득!");
            }

            Destroy(gameObject); // 아이템 소멸
        }
    }
}
