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

    [Header("=== 무기 강화 설정 ===")]
    [Tooltip("오브젝트 풀 키 이름 3개를 순서대로 적어줍니다. (기본탄, 1단계탄, 2단계탄)")]
    [SerializeField] private string[] bulletPoolKeys = new string[] { "PlayerBullet", "PlayerBulletLevel1", "PlayerBulletLevel2" };
    private int weaponLevel = 0; // 0: 기본, 1: 1단계, 2: 2단계 (최대 2)

    void Update()
    {
        // 방향키 및 WASD 입력 감지
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(h, v, 0f).normalized;

        // 등속 운동으로 플레이어 기체 좌표 이동
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float clampedX = Mathf.Clamp(transform.position.x, MIN_X, MAX_X);
        float clampedY = Mathf.Clamp(transform.position.y, MIN_Y, MAX_Y);

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
        if (firePoint == null) return;
        if (ObjectPoolManager.Instance == null) return;
        if (bulletPoolKeys == null || bulletPoolKeys.Length == 0) return;

        // 현재 무기 레벨에 맞는 오브젝트 풀 키 이름을 안전하게 계산합니다.
        int currentKeyIndex = Mathf.Clamp(weaponLevel, 0, bulletPoolKeys.Length - 1);
        string currentBulletKey = bulletPoolKeys[currentKeyIndex];

        // 계산된 키 이름으로 오브젝트 풀 매니저에서 탄환을 빌려옵니다.
        GameObject bullet = ObjectPoolManager.Instance.GetObject(currentBulletKey);

        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.identity;
        }
    }

    // 💡 [최종 업그레이드] 아이템 획득 시 무기 레벨을 올리거나 보너스 점수를 주는 함수
    public void UpgradeWeapon()
    {
        // 🎯 아직 최대 레벨(2)이 아니라면 무기를 정상 강화합니다.
        if (weaponLevel < 2)
        {
            weaponLevel++;
            Debug.Log("무기 레벨 업 완료! 현재 레벨: " + weaponLevel);
        }
        // 🎯 만약 이미 최고 레벨(2)인 상태에서 아이템을 또 먹었다면 보너스 점수를 퍼줍니다!
        else
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(1000); // 보너스 점수 1000점 주입 (원하는 숫자로 변경 가능)
                Debug.Log("무기 최대 강화 상태! 보너스 점수 +1000점 획득!");
            }
        }
    }
}
