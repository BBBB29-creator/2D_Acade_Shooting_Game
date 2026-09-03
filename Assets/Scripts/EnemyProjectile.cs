using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float moveSpeed = 5f;   // 적 탄은 회피할 수 있게 비교적 느림
    private float destroyY = -6f;  // 화면 아래로 나갈 때 삭제

    void Update()
    {
        // 적 탄은 아래로 날아감 (Vector3.down)
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);

        if (transform.position.y <= destroyY)
        {
            gameObject.SetActive(false); // 오브젝트 풀 반환
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어 체력 컴포넌트를 가져와 데미지 처리 (예시 코드)
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // player.TakeDamage(1); // 플레이어 데미지 메서드 호출
            }
            gameObject.SetActive(false); // 플레이어를 맞추면 탄환 삭제
        }
    }
}
