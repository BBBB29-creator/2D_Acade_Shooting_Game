using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    [Header("엑셀 데이터베이스 연동")]
    [SerializeField] private EnemyDatabaseSO enemyDatabase;

    [Tooltip("EnemyDatabaseSO에 등록된 정확한 적의 이름 (Normal, Medium, Heavy)")]
    [SerializeField] private string enemyName = "Normal";

    [Header("=== 아이템 드롭 설정 ===")]
    [Tooltip("0번: 노말 적이 줄 회복템, 1번: 미디움 적이 줄 강화템")]
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private float dropChance = 10f;


    private string currentPoolType;
    private int currentKillScore;
    private Health health;

    // 💡 [새로 추가] 내가 태어난 스포너의 정보와 내 자리 번호(인덱스)를 저장할 변수
    private EnemySpawner spawner;
    private int mySpawnIndex = -1;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        // 💡 오브젝트 풀에서 깨어날 때 데이터베이스 수치 동기화
        if (enemyDatabase != null)
        {
            enemyDatabase.InitializeCache();
            EnemyDataRow data = enemyDatabase.GetEnemyData(enemyName);

            // 엑셀 체력 주입
            if (health != null)
            {
                health.Setup(data.maxHP);
            }

            currentKillScore = data.score;
            currentPoolType = $"{data.enemyName}Enemy"; // 예: NormalEnemy
        }
    }

    public void ExecuteDeath()
    {
        // 🎯 확률 계산 가동 (난수가 설정한 dropChance보다 작을 때만 드롭 성공)
        if (itemPrefabs != null && itemPrefabs.Length > 0 && Random.Range(0f, 100f) <= dropChance)
        {
            // 내 이름에 따라 떨굴 아이템 주소(인덱스)를 정합니다.
            int itemIndex = -1;
            if (enemyName == "Normal") itemIndex = 0; // 노말 적은 0번 (회복템)
            if (enemyName == "Medium") itemIndex = 1; // 미디움 적은 1번 (강화템)

            // 올바른 아이템 주소가 잡혔다면 월드(Z=0)에 소환합니다.
            if (itemIndex >= 0 && itemIndex < itemPrefabs.Length && itemPrefabs[itemIndex] != null)
            {
                Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, 0f);
                Instantiate(itemPrefabs[itemIndex], spawnPos, Quaternion.identity);
            }
        }

        // --- 이하 기존의 점수 누적 및 오브젝트 풀 반환 코드 100% 그대로 유지 ---
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(currentKillScore);
        if (ObjectPoolManager.Instance != null) ObjectPoolManager.Instance.ReleaseObject(gameObject, currentPoolType);
    }

    public float GetMoveSpeed()
    {
        if (enemyDatabase != null)
        {
            return enemyDatabase.GetEnemyData(enemyName).moveSpeed;
        }
        return 3f;
    }

    // =========================================================================
    // 🎯 [새로 추가] 스포너 자리 관리용 함수들
    // =========================================================================

    // 1. 스포너가 소환할 때 자리 번호를 넘겨받는 통로 함수
    public void SetupSpawnerReference(EnemySpawner enemySpawner, int index)
    {
        spawner = enemySpawner;
        mySpawnIndex = index;
    }

    // 2. 죽어서 오브젝트 풀로 돌아가 비활성화되는 순간 자동으로 실행되는 유니티 내장 함수
    private void OnDisable()
    {
        // 스포너 사령탑에게 "저 죽었으니 제가 쓰던 자리 비워주세요"라고 신호를 보냅니다.
        if (spawner != null && mySpawnIndex != -1)
        {
            spawner.ReleasePosition(mySpawnIndex);
            mySpawnIndex = -1; // 내 자리 번호 기억 초기화
        }
    }
}
