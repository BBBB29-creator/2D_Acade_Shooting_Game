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

    [Space(10)]
    [SerializeField] private GameObject normalEnemyPrefab;
    [SerializeField] private GameObject mediumEnemyPrefab;
    [SerializeField] private GameObject heavyEnemyPrefab;

    // 최적화 내장 풀 시스템 변수
    private IObjectPool<GameObject> playerBulletPool;
    private IObjectPool<GameObject> enemyBulletPool;

    private IObjectPool<GameObject> normalEnemyPool;
    private IObjectPool<GameObject> mediumEnemyPool;
    private IObjectPool<GameObject> heavyEnemyPool;

    [Header("최적화 용량 설정")]
    [SerializeField] private int defaultSize = 20;
    [SerializeField] private int maxSize = 100;

    private void Awake()
    {
        // [방어 가드 2] 복사본 중복 생성 완벽 차단
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

        // 안전하게 통과된 원본만 풀 생성 로직을 실행합니다.
        playerBulletPool = CreatePool(playerBulletPrefab);
        enemyBulletPool = CreatePool(enemyBulletPrefab);

        // 3종류 적 풀 생성
        normalEnemyPool = CreatePool(normalEnemyPrefab);
        mediumEnemyPool = CreatePool(mediumEnemyPrefab);
        heavyEnemyPool = CreatePool(heavyEnemyPrefab);
    }

    // 람다식과 콜백을 활용한 정석 풀 생성 자동화 함수
    private IObjectPool<GameObject> CreatePool(GameObject prefab)
    {
        // [안전장치] 프리팹 연결을 깜빡했을 때 에러 뿜으며 튕기는 현상 원천 차단
        if (prefab == null) return null;

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

    public GameObject GetObject(string type)
    {
        if (isShuttingDown) return null;

        // 소문자로 강제 변환하여 대소문자 오타 원천 차단
        string cleanType = type.Trim().ToLower();

        switch (cleanType)
        {
            case "playerbullet": return playerBulletPool?.Get();
            case "enemybullet": return enemyBulletPool?.Get();

            case "normalenemy": return normalEnemyPool?.Get();
            case "mediumenemy": return mediumEnemyPool?.Get();
            case "heavyenemy": return heavyEnemyPool?.Get();
            default:
                Debug.LogError($"[ObjectPoolManager] 잘못된 타입 요청 들어옴 ➡️ [{type}]");
                return normalEnemyPool?.Get(); // 에러 나도 일단 노말 적이라도 뱉어내기
        }
    }


    public void ReleaseObject(GameObject obj, string type)
    {
        // 게임 종료 중일 때 반환 연산을 무시하여 NullReferenceException을 완벽 차단합니다.
        if (isShuttingDown) return;

        // 반환하려는 오브젝트가 이미 파괴되었거나 null 인지 검증
        if (obj == null) return;

        // [초강력 안전장치] 앞뒤 공백을 자르고 소문자로 강제 통일하여 비교합니다.
        string cleanType = type.Trim().ToLower();

        switch (cleanType)
        {
            case "playerbullet": playerBulletPool?.Release(obj); break;
            case "enemybullet": enemyBulletPool?.Release(obj); break;

            // 대소문자나 띄어쓰기 오타가 나도 무조건 정상 반환되도록 소문자로 매칭
            case "normalenemy": normalEnemyPool?.Release(obj); break;
            case "mediumenemy": mediumEnemyPool?.Release(obj); break;
            case "heavyenemy": heavyEnemyPool?.Release(obj); break;

            default:
                // 여전히 알 수 없는 타입이 들어오면 범인을 정확히 대괄호 안에 출력합니다.
                Debug.LogError($"[ObjectPoolManager] 알 수 없는 반환 타입 들어옴 ➡️ [{type}]");

                // [발표회장용 치트키] 에러가 나더라도 발표가 망하지 않게 무조건 일반 적 풀로 반환되게 강제 조치!
                normalEnemyPool?.Release(obj);
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
