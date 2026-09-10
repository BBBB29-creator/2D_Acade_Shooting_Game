using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    [Header("엑셀 데이터베이스 연동")]
    [SerializeField] private EnemyDatabaseSO enemyDatabase;

    [Tooltip("EnemyDatabaseSO에 등록된 정확한 적의 이름 (Normal, Medium, Heavy)")]
    [SerializeField] private string enemyName = "Normal";

    private string currentPoolType;
    private int currentKillScore;
    private Health health;

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

    // 💡 Health.cs에서 체력이 0이 되면 직접 이 함수를 호출하게 만듭니다.
    public void ExecuteDeath()
    {
        // 1. 점수 누적
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(currentKillScore);
        }

        // 2. 오브젝트 풀링 매니저로 완벽 반환
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReleaseObject(gameObject, currentPoolType);
        }
    }

    public float GetMoveSpeed()
    {
        if (enemyDatabase != null)
        {
            return enemyDatabase.GetEnemyData(enemyName).moveSpeed;
        }
        return 3f;
    }
}
