using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("화면 제한 경계선")]
    private const float MIN_X = -8.3f;
    private const float MAX_X = 8.3f;
    private const float MIN_Y = -4.4f;
    private const float MAX_Y = 4.4f;

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 7f; // 비행기 이동 속도

    [Header("사격 설정")]
    public Transform firePoint;         // 플레이어 비행기 총구 위치
    public float attackCooldown = 0.2f; // 연사 속도 (0.2초마다 발사)
    private float attackTimer = 0f;

    void Update()
    {
        // 방향키 및 WASD 입력 감지
        float h = Input.GetAxisRaw("Horizontal"); // 좌우 입력 (-1, 0, 1)
        float v = Input.GetAxisRaw("Vertical");   // 위아래 입력 (-1, 0, 1)

        Vector3 moveDir = new Vector3(h, v, 0f).normalized;

        // 등속 운동으로 플레이어 기체 좌표 이동
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, MIN_X, MAX_X); // 좌우 경계선
        float clampedY = Mathf.Clamp(transform.position.y, MIN_Y, MAX_Y); // 상하 경계선

        // 필터링 된 안전한 좌표를 플레이어 포지션에 최종 대입
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);


        // 스페이스바 사격 타이머 로직
        attackTimer += Time.deltaTime;

        if (Input.GetKey(KeyCode.Space) && attackTimer >= attackCooldown)
        {
            Fire();
            
            attackTimer = 0f;
        }
    }

    void Fire()
    {
        if (firePoint != null)
        {
            // 최적화 통합 매니저에서 플레이어 탄환 요청
            GameObject bullet = ObjectPoolManager.Instance.GetObject("PlayerBullet");

            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;

                // 무조건 절대적인 앞방향(Z: 0도)으로 회전값 강제 고정
                bullet.transform.rotation = Quaternion.identity;
            }
        }
    }
}
