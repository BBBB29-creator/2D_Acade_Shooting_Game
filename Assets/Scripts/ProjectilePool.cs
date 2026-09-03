using UnityEngine;
using UnityEngine.Pool; // 💡 최적화를 위한 필수 라이브러리

public class ProjectilePool : MonoBehaviour
{
    // 어디서나 쉽게 접근할 수 있도록 싱글톤 인스턴스 구성
    public static ProjectilePool Instance { get; private set; }

    [Header("=== 프리팹 설정 ===")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;

    // 유니티 공식 최적화 풀 시스템 선언
    private IObjectPool<GameObject> playerPool;
    private IObjectPool<GameObject> enemyPool;

    [Header("=== 최적화 용량 설정 ===")]
    [SerializeField] private int defaultSize = 20; // 최초로 가질 메모리 방 개수
    [SerializeField] private int maxSize = 100;     // 렉 폭발 방지를 위한 최대 상한선

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 1. 플레이어 탄환 풀 최적화 세팅
        playerPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(playerPrefab, transform), // 부족할 때 생성하는 규칙
            actionOnGet: (obj) => obj.SetActive(true),              // 바구니에서 꺼낼 때 켜기
            actionOnRelease: (obj) => obj.SetActive(false),          // 바구니에 넣을 때 끄기
            actionOnDestroy: (obj) => Destroy(obj),                  // maxSize 넘쳐서 버릴 때 파괴
            collectionCheck: true,                                   // 중복 반환 버그 철저히 감시
            defaultCapacity: defaultSize,
            maxSize: maxSize
        );

        // 2. 적 탄환 풀 최적화 세팅
        enemyPool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(enemyPrefab, transform),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: defaultSize,
            maxSize: maxSize
        );
    }

    /// <summary>
    /// 매개변수로 "Player" 혹은 "Enemy"를 입력받아 최적화된 풀에서 탄환을 꺼내줍니다.
    /// </summary>
    public GameObject GetProjectile(string type)
    {
        if (type == "Player") return playerPool.Get();
        else return enemyPool.Get();
    }

    /// <summary>
    /// 화면 밖으로 나가거나 적과 충돌한 탄환을 최적화된 풀로 안전하게 되돌립니다.
    /// </summary>
    public void ReleaseProjectile(GameObject obj, string type)
    {
        if (type == "Player") playerPool.Release(obj);
        else enemyPool.Release(obj);
    }
}
