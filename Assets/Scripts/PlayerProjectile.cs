using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("=== 탄환 설정 ===")]
    [SerializeField] private float moveSpeed = 12f; // 플레이어 탄속
    [SerializeField] private float destroyY = 10f;  // 화면 위로 탈출 시 경계선

    void Update()
    {
        // 절대적인 월드 좌표 기준으로 매 프레임 위로 정직하게 날아갑니다.
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);

        // 화면 상단 천장을 넘어가면 화면에서 안 보이게 끕니다.
        if (transform.position.y >= destroyY)
        {
            gameObject.SetActive(false);
        }
    }

    // [충돌 감지 핵심] 2D 콜라이더 Trigger 충돌 시 실행
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 오직 "Enemy" 태그를 가진 적만 골라서 타격합니다.
        if (collision.CompareTag("Enemy"))
        {
            // 부딪힌 대상에게서 새로 바꾼 Health 컴포넌트를 가져옵니다.
            Health enemyHp = collision.GetComponent<Health>();

            if (enemyHp != null)
            {
                // 정직하게 대미지를 1 입힙니다.
                enemyHp.TakeDamage(1);
            }

            // 적을 맞추면 탄환은 즉시 스스로 삭제(비활성화)됩니다.
            gameObject.SetActive(false);
        }
    }
}
