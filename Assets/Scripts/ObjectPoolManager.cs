using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    // 외부에서 접근할 수 있는 유일한 통로
    public static ObjectPoolManager Instance { get; private set; }

    // [방어 가드 1] 게임 종료 시 생명주기 꼬임으로 인한 에러 차단 플래그
    private static bool isShuttingDown = false;

    [Header("프리팹 설정")]
    [SerializeField] private GameObject playerBulletPrefab;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private GameObject normalEnemyPrefab;

    // 최적화 내장 풀 시스템 변수
    private IObjectPool<GameObject> playerBulletPool;
    private IObjectPool<GameObject> enemyBulletPool;
    private IObjectPool<GameObject> normalEnemyPool;

    [Header("최적화 용량 설정")]
    [SerializeField] private int defaultSize = 20;
    [SerializeField] private int maxSize = 100;

    private void Awake()
    {
        // [방어 가드 2] 복사본 중복 생성 완벽 차단
        if (Instance == null)
        {
            Instance = this;
            // 씬이 바뀌어도 매니저가 파괴되지 않고 유지되기를 원한다면 아래 주석을 해제하세요.
            // DontDestroyOnLoad(gameObject); 
        }
        else if (Instance != this)
        {
            // 이미 원본이 있는데 또 태어난 복사본은 가차 없이 즉시 파괴
            Debug.LogWarning($"[ObjectPoolManager] 중복된 매니저가 감지되어 파괴되었습니다: {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        // 안전하게 통과된 원본만 풀 생성 로직을 실행합니다.
        playerBulletPool = CreatePool(playerBulletPrefab);
        enemyBulletPool = CreatePool(enemyBulletPrefab);
        normalEnemyPool = CreatePool(normalEnemyPrefab);
    }

    // 람다식과 콜백을 활용한 정석 풀 생성 자동화 함수
    private IObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab, transform),
            actionOnGet: (obj) => obj.SetActive(true),      // 꺼낼 때 켜기
            actionOnRelease: (obj) => obj.SetActive(false),  // 반환할 때 끄기
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true, // [방어 가드 3] 이미 풀에 있는데 또 넣으려고 하는 중복 반환 버그 감시
            defaultCapacity: defaultSize,
            maxSize: maxSize
        );
    }

    /// <summary>
    /// 안전하게 최적화된 풀에서 오브젝트를 꺼내옵니다.
    /// </summary>
    public GameObject GetObject(string type)
    {
        // [방어 가드 1-1] 게임이 꺼지는 중이라면 Null을 뱉어 에러를 차단합니다.
        if (isShuttingDown) return null;

        switch (type)
        {
            case "PlayerBullet": return playerBulletPool.Get();
            case "EnemyBullet": return enemyBulletPool.Get();
            case "NormalEnemy": return normalEnemyPool.Get();
            default:
                Debug.LogError($"[ObjectPoolManager] 잘못된 타입 요청: {type}");
                return null;
        }
    }

    /// <summary>
    /// 사용이 끝난 오브젝트를 안전하게 풀 바구니로 되돌립니다.
    /// </summary>
    public void ReleaseObject(GameObject obj, string type)
    {
        // [방어 가드 1-2] 게임 종료 중일 때 반환 연산을 무시하여 NullReferenceException을 완벽 차단합니다.
        if (isShuttingDown) return;

        // 반환하려는 오브젝트가 이미 파괴되었거나 null 인지 2차 검증
        if (obj == null) return;

        switch (type)
        {
            case "PlayerBullet": playerBulletPool.Release(obj); break;
            case "EnemyBullet": enemyBulletPool.Release(obj); break;
            case "NormalEnemy": normalEnemyPool.Release(obj); break;
            default:
                Debug.LogError($"[ObjectPoolManager] 알 수 없는 반환 타입: {type}");
                break;
        }
    }

    // 유니티 시스템이 게임 종료 버튼(혹은 앱 종료)을 감지하는 순간 실행됩니다.
    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    // 씬이 전환되거나 오브젝트가 파괴될 때 안전하게 플래그를 동기화합니다.
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
