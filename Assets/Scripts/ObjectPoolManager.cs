using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    private static bool isShuttingDown = false;

    [Header("프리팹 설정")]
    [SerializeField] private GameObject playerBulletPrefab;
    // 🎯 [핵심 추가] 인스펙터에서 새로 만든 강화 탄환 프리팹 2개를 조립할 슬롯
    [SerializeField] private GameObject playerBulletLevel1Prefab;
    [SerializeField] private GameObject playerBulletLevel2Prefab;

    [SerializeField] private GameObject enemyBulletPrefab;

    [Space(10)]
    [SerializeField] private GameObject normalEnemyPrefab;
    [SerializeField] private GameObject mediumEnemyPrefab;
    [SerializeField] private GameObject heavyEnemyPrefab;

    // 최적화 내장 풀 시스템 변수
    private IObjectPool<GameObject> playerBulletPool;
    // 🎯 [핵심 추가] 강화 탄환용 독립 풀 변수 2개 추가
    private IObjectPool<GameObject> playerBulletLevel1Pool;
    private IObjectPool<GameObject> playerBulletLevel2Pool;

    private IObjectPool<GameObject> enemyBulletPool;

    private IObjectPool<GameObject> normalEnemyPool;
    private IObjectPool<GameObject> mediumEnemyPool;
    private IObjectPool<GameObject> heavyEnemyPool;

    [Header("최적화 용량 설정")]
    [SerializeField] private int defaultSize = 20;
    [SerializeField] private int maxSize = 100;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning($"[ObjectPoolManager] 중복된 매니저가 감지되어 파괴되었습니다: {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        // 기본탄 풀 생성
        playerBulletPool = CreatePool(playerBulletPrefab);
        // 🎯 [핵심 추가] 깨어날 때 강화 탄환 풀들도 안전하게 자동 빌드합니다.
        playerBulletLevel1Pool = CreatePool(playerBulletLevel1Prefab);
        playerBulletLevel2Pool = CreatePool(playerBulletLevel2Prefab);

        enemyBulletPool = CreatePool(enemyBulletPrefab);

        // 3종류 적 풀 생성
        normalEnemyPool = CreatePool(normalEnemyPrefab);
        mediumEnemyPool = CreatePool(mediumEnemyPrefab);
        heavyEnemyPool = CreatePool(heavyEnemyPrefab);
    }

    private IObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        if (prefab == null) return null;

        return new ObjectPool<GameObject>(
            createFunc: () => Instantiate(prefab, transform),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: defaultSize,
            maxSize: maxSize
        );
    }

    public GameObject GetObject(string type)
    {
        if (isShuttingDown) return null;

        string cleanType = type.Trim().ToLower();

        switch (cleanType)
        {
            case "playerbullet": return playerBulletPool?.Get();
            // 🎯 [핵심 추가] 플레이어 컨트롤러가 대소문자 섞어 부르더라도 안전하게 매칭하여 뱉어줍니다.
            case "playerbulletlevel1": return playerBulletLevel1Pool?.Get();
            case "playerbulletlevel2": return playerBulletLevel2Pool?.Get();

            case "enemybullet": return enemyBulletPool?.Get();

            case "normalenemy": return normalEnemyPool?.Get();
            case "mediumenemy": return mediumEnemyPool?.Get();
            case "heavyenemy": return heavyEnemyPool?.Get();
            default:
                Debug.LogError($"[ObjectPoolManager] 잘못된 타입 요청 들어옴 ➡️ [{type}]");
                return normalEnemyPool?.Get();
        }
    }

    public void ReleaseObject(GameObject obj, string type)
    {
        if (isShuttingDown) return;
        if (obj == null) return;

        string cleanType = type.Trim().ToLower();

        switch (cleanType)
        {
            case "playerbullet": playerBulletPool?.Release(obj); break;
            // 🎯 [핵심 추가] 발사된 강화 탄환들이 적에 닿아 사라질 때 제 방으로 똑바로 찾아오게 만듭니다.
            case "playerbulletlevel1": playerBulletLevel1Pool?.Release(obj); break;
            case "playerbulletlevel2": playerBulletLevel2Pool?.Release(obj); break;

            case "enemybullet": enemyBulletPool?.Release(obj); break;

            case "normalenemy": normalEnemyPool?.Release(obj); break;
            case "mediumenemy": mediumEnemyPool?.Release(obj); break;
            case "heavyenemy": heavyEnemyPool?.Release(obj); break;

            default:
                Debug.LogError($"[ObjectPoolManager] 알 수 없는 반환 타입 들어옴 ➡️ [{type}]");
                normalEnemyPool?.Release(obj);
                break;
        }
    }

    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
