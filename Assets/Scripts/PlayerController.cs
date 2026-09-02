using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 7f; // 비행기 이동 속도

    [Header("사격 설정")]
    public Transform firePoint;         // 플레이어 비행기 총구 위치
    public float attackCooldown = 0.2f; // 연사 속도 (0.2초마다 발사)
    private float attackTimer = 0f;

    void Update()
    {
        // 1. 키보드 이동 로직 (방향키 및 WASD 입력 감지)
        float h = Input.GetAxisRaw("Horizontal"); // 좌우 입력 (-1, 0, 1)
        float v = Input.GetAxisRaw("Vertical");   // 위아래 입력 (-1, 0, 1)

        // 대각선 이동 시 속도가 빨라지지 않도록 방향 벡터 정규화(normalized) 처리
        Vector3 moveDir = new Vector3(h, v, 0f).normalized;

        // 등속 운동으로 플레이어 기체 좌표 이동
        transform.position += moveDir * moveSpeed * Time.deltaTime;


        // 2. 스페이스바 사격 타이머 로직 (기존 유지)
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
            GameObject bullet = ProjectilePool.Instance.GetProjectile();
            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;

                // 💡 [방향 고정 핵심] 총구 오브젝트의 회전값에 버그가 있더라도 
                // 무조건 절대적인 앞방향(Z: 0도)으로 회전값을 강제 고정하여 위로 날아가게 만듭니다.
                bullet.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                // 태그 설정 (기존 유지)
                Projectile projectileScript = bullet.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.ownerTag = "Player";
                }

                bullet.SetActive(true);
            }
        }
    }

}
