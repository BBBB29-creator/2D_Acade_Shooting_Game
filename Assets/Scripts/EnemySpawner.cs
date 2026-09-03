using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("=== 스폰 설정 ===")]
    [SerializeField] private float spawnDelay = 2.0f; // 적이 나타나는 시간 간격 (2초마다)

    [Header("=== 화면 생성 범위 (16:9 해상도 기준) ===")]
    // Full HD 16:9 기준 화면 좌우 끝 수치입니다.
    private const float MIN_X = -7.5f;
    private const float MAX_X = 7.5f;
    private const float SPAWN_Y = 6.5f; // 화면 맨 위 천장 바로 바깥 좌표 (숨어서 스폰됨)

    private void Start()
    {
        // 게임이 시작되면 무한 스폰 루프 코루틴을 실행합니다.
        StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        // 게임이 플레이 중인 동안 무한 반복
        while (true)
        {
            // 다음 적이 나오기 전까지 설정한 시간만큼 대기 (예: 2초)
            yield return new WaitForSeconds(spawnDelay);

            // 🛡️ [방어 가드] 혹시 풀 매니저가 메모리에서 사라졌다면 스폰을 중단해 에러를 차단합니다.
            if (ObjectPoolManager.Instance == null) continue;

            // 1. 우리가 완성한 방어막 풀 매니저에서 일반 적 비행기를 하나 빌려옵니다.
            GameObject enemy = ObjectPoolManager.Instance.GetObject("NormalEnemy");

            if (enemy != null)
            {
                // 2. 화면 좌측 끝(-7.5)부터 우측 끝(7.5) 사이의 무작위 X 좌표를 계산합니다.
                float randomX = Random.Range(MIN_X, MAX_X);

                // 3. 적 비행기의 위치를 화면 머리 위 천장(Y: 6.5)의 무작위 X 좌표로 순간 이동시킵니다.
                enemy.transform.position = new Vector3(randomX, SPAWN_Y, 0f);

                // 4. 생성 시 적 비행기의 회전값을 정방향(0,0,0)으로 깔끔하게 리셋합니다.
                enemy.transform.rotation = Quaternion.identity;
            }
        }
    }
}
