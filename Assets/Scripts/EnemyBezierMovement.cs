using UnityEngine;

public class EnemyBezierMovement : MonoBehaviour
{
    [Header("이동 경로 설정 (점 4개)")]
    public Transform p0;
    public Transform p1;
    public Transform p2;
    public Transform p3;

    [Header("이동 속도")]
    [SerializeField] private float speed = 0.5f;

    [Header("사막 맵 출현 범위 제한")]
    [Tooltip("플레이어 오브젝트나 사막 배경을 할당하여 가로 제한 폭을 참고합니다.")]
    [SerializeField] private Transform backgroundTransform;

    private float minX, maxX;
    private float currentTime = 0f;
    private SpriteRenderer myRenderer; // 적의 모습을 끄고 켤 컴포넌트

    void Start()
    {
        myRenderer = GetComponent<SpriteRenderer>();

        // 하이어라키 창의 Background 스케일값을 기준으로 가로폭(X) 제한 범위 자동 계산
        if (backgroundTransform != null)
        {
            float mapHalfWidth = backgroundTransform.localScale.x / 2f;
            float mapCenterX = backgroundTransform.position.x;

            minX = mapCenterX - mapHalfWidth;
            maxX = mapCenterX + mapHalfWidth;
        }
    }

    void Update()
    {
        currentTime += Time.deltaTime * speed;

        if (currentTime >= 1f)
        {
            currentTime = 1f;
            Destroy(gameObject); // 화면 아래로 나가면 삭제
        }

        // 1. 베지에 곡선으로 이동 처리
        transform.position = CalculateBezierPoint(currentTime, p0.position, p1.position, p2.position, p3.position);

        // 2. [핵심] 사막 맵 밖으로 나가면 투명하게 숨기기
        if (backgroundTransform != null && myRenderer != null)
        {
            // 현재 적의 X 좌표가 사막 가로 경계선 내부(안쪽)에 있을 때만 스프라이트 활성화
            if (transform.position.x >= minX && transform.position.x <= maxX)
            {
                myRenderer.enabled = true;  // 사막 안에서는 보임
            }
            else
            {
                myRenderer.enabled = false; // 사막 밖(검은 배경 구역)에서는 안 보임
            }
        }
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);

        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(d, e, t);
    }
}
