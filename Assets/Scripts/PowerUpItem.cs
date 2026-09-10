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
            // 💡 플레이어의 공격 기능을 담당하는 스크립트(예: PlayerController 등)를 찾습니다.
            // 아래의 PlayerController는 본인의 스크립트 이름으로 수정해 주세요.
            if (collision.TryGetComponent<PlayerController>(out var player))
            {
                // 플레이어 스크립트 내부에 공격력 상승 함수를 추가해 두고 여기서 때려줍니다.
                // player.UpgradeWeapon(); 
                Debug.Log("🎉 파워업 아이템 획득!");
            }

            Destroy(gameObject); // 아이템 소멸
        }
    }
}
