using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float destroyY = -6f;

    // 💡 물리 엔진 간섭 없이 직진할 발사 방향 벡터 (기본값 아래 방향)
    private Vector3 customFlyDirection = Vector3.down;

    // 💡 원격으로 노말 조준 패턴(PlayerTargetAttack)이 이 함수를 때려 방향과 고속 스피드를 먹여줍니다.
    public void SetTargetingDirection(Vector3 direction, float speed)
    {
        customFlyDirection = direction.normalized;
        moveSpeed = speed; // 라이덴 스타일 고속 스피드(8.5f) 주입
    }

    private void OnEnable()
    {
        // 풀에서 새로 꺼내질 때마다 미디움 적용 기본 상태(하강탄)로 안전하게 초기화
        customFlyDirection = Vector3.down;
        moveSpeed = 5f; // 미디움 기본 속도 복구
    }

    void Update()
    {
        // [버그 완전 박멸] 물리 세팅(Kinematic)의 간섭을 받지 않고, 
        // 무조건 내가 주입받은 나만의 발사 방향으로 다이렉트 전진합니다!
        transform.Translate(customFlyDirection * moveSpeed * Time.deltaTime, Space.World);

        if (transform.position.y <= destroyY)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 플레이어 오브젝트에 붙어있는 진짜 체력 바구니(Health)를 찾아옵니다!
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth != null)
            {
                // 플레이어에게 정확하게 1의 대미지를 입힙니다. (이제 무적이 풀립니다!)
                playerHealth.TakeDamage(1);
            }

            gameObject.SetActive(false); // 플레이어를 맞추면 총알 풀 반환
        }
    }
}
