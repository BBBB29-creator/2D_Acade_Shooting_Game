using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("=== 일반 적 스폰 설정 ===")]
    [SerializeField] private float spawnDelay = 2.0f;

    [Header("=== 중간 적(Medium) 스폰 설정 ===")]
    [SerializeField] private float mediumSpawnDelay = 15.0f;
    private const int MAX_MEDIUM_COUNT = 3;

    // 화면에 살아있는 중간 적들을 추적하기 위한 리스트
    private List<GameObject> activeMediumEnemies = new List<GameObject>();

    // 겹치지 않게 하기 위한 고정 좌우(X) 및 높이(Y) 좌표 배열 (3개 자리)
    private float[] mediumSpawnXPositions = new float[] { -5.0f, 0f, 5.0f };
    private float[] mediumSpawnYPositions = new float[] { 4.5f, 5.5f, 5.0f };

    // 각 스폰 자리가 사용 중인지 체크하는 배열 (true = 사용 중)
    private bool[] isPositionOccupied = new bool[] { false, false, false };

    [Header("=== 화면 생성 범위 (16:9 해상도 기준) ===")]
    private const float MIN_X = -7.5f;
    private const float MAX_X = 7.5f;
    private const float SPAWN_Y = 6.5f;

    private void Start()
    {
        // 1. 일반 에너미 무한 스폰 루프 실행 (질문자님의 100% 정상 원본 코드)
        StartCoroutine(SpawnEnemyRoutine());

        // 2. 중간 에너미 스폰 타이머 작동 (코루틴 버그 원천 차단)
        // 게임 시작 후 1초 뒤부터 mediumSpawnDelay(3초) 간격으로 무한 반복 실행됩니다.
        InvokeRepeating("TrySpawnMediumEnemy", 1.0f, mediumSpawnDelay);
    }

    // 🎬 1. 일반 에너미 무한 스폰
    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            if (ObjectPoolManager.Instance == null) continue;

            GameObject enemy = ObjectPoolManager.Instance.GetObject("NormalEnemy");

            if (enemy != null)
            {
                float randomX = Random.Range(MIN_X, MAX_X);
                enemy.transform.position = new Vector3(randomX, SPAWN_Y, 0f);
                enemy.transform.rotation = Quaternion.identity;
            }
        }
    }

    // 🎬 2. 타이머에 의해 3초마다 안전하게 호출되는 중간 에너미 스폰 함수
    private void TrySpawnMediumEnemy()
    {
        // 1) 리스트에서 죽은(꺼진) 애들을 가장 먼저 청소합니다.
        CleanActiveMediumList();

        // 2) 현재 화면에 살아있는 마리수가 3마리 이상이면 스폰을 건너뜁니다.
        if (activeMediumEnemies.Count >= MAX_MEDIUM_COUNT) return;

        // 3) 오브젝트 풀 매니저가 유효한지 확인합니다.
        if (ObjectPoolManager.Instance == null) return;

        // 4) 비어있는 스폰 자리 번호(0, 1, 2)를 찾습니다.
        int availableIndex = GetAvailableSpawnIndex();
        if (availableIndex == -1) return; // 모든 자리가 꽉 찼다면 리턴

        // 5) 대문자 "MediumEnemy"로 풀 매니저에서 오브젝트를 안전하게 빌려옵니다.
        GameObject MediumEnemy = ObjectPoolManager.Instance.GetObject("MediumEnemy");
        if (MediumEnemy == null) return;

        // 6) 해당 자리를 사용 중으로 전환합니다.
        isPositionOccupied[availableIndex] = true;

        // 7) 지정된 겹치지 않는 X, Y 고정 좌표로 배치합니다.
        float spawnX = mediumSpawnXPositions[availableIndex];
        float spawnY = mediumSpawnYPositions[availableIndex];
        MediumEnemy.transform.position = new Vector3(spawnX, spawnY, 0f);
        MediumEnemy.transform.rotation = Quaternion.identity;

        // 8) 징검다리 데이터 스크립트가 있다면 스포너 참조와 자리 번호를 기억시킵니다.
        EnemyCharacter data = MediumEnemy.GetComponent<EnemyCharacter>();
        if (data != null)
        {
            data.SetupSpawnerReference(this, availableIndex);
        }

        // 9) 추적 리스트에 추가합니다.
        activeMediumEnemies.Add(MediumEnemy);
    }

    // 🛠️ 리스트를 뒤에서부터 순회하며 죽은(비활성화) 적을 목록에서 지워주는 함수
    private void CleanActiveMediumList()
    {
        for (int i = activeMediumEnemies.Count - 1; i >= 0; i--)
        {
            if (activeMediumEnemies[i] == null || !activeMediumEnemies[i].activeInHierarchy)
            {
                activeMediumEnemies.RemoveAt(i);
            }
        }
    }

    // 🛠️ 3개의 라인 중 비어있는(false) 자리를 찾는 함수
    private int GetAvailableSpawnIndex()
    {
        for (int i = 0; i < isPositionOccupied.Length; i++)
        {
            if (isPositionOccupied[i] == false)
            {
                return i;
            }
        }
        return -1;
    }

    // 💡 중간 에너미가 죽어서 꺼질 때(OnDisable) 자리를 해제해주는 통로 함수
    public void ReleasePosition(int index)
    {
        if (index >= 0 && index < isPositionOccupied.Length)
        {
            isPositionOccupied[index] = false;
        }
    }
}
