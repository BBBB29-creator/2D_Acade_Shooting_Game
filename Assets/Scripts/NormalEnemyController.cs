using UnityEngine;

public class NormalEnemyController : MonoBehaviour
{
    [Header("=== 이동 설정 ===")]
    [SerializeField] private float moveSpeed = 3f; // 화면 위에서 아래로 천천히 내려오는 속도

    [Header("=== 화면 외 삭제 경계선 ===")]
    // Full HD 16:9 해상도(카메라 Size 5) 기준 화면 맨 아래 바닥 너머 좌표입니다.
    private const float DESTROY_Y = -6.5f;

    // 오브젝트 풀에서 꺼내져서 화면에 켜질 때마다 매번 실행되는 초기화 함수
    private void OnEnable()
    {
        // 새로 태어날 때마다 상태나 오차가 초기화되도록 방어선을 칩니다.
        // (필요 시 나중에 체력 리셋 코드가 들어올 자리입니다)
    }

    void Update()
    {
        // 1. 다른 오브젝트나 회전값에 영향받지 않고, 무조건 절대적인 화면 아래(World 기준)로 전진합니다.
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);

        // 2. 아래로 내려가다가 화면 아래 경계선(카메라 범위 밖)을 완전히 벗어났는지 체크합니다.
        if (transform.position.y <= DESTROY_Y)
        {
            // 3. 4중 방어막을 갖춘 우리 풀 매니저에게 나(노말 적)를 안전하게 반환하라고 요청합니다.
            // (가비지 가드 및 NullReferenceException이 차단됩니다)
            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.ReleaseObject(gameObject, "NormalEnemy");
            }
        }
    }
}
