using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("맵 제한 설정")]
    [Tooltip("Hierarchy 창에 있는 'Background' 오브젝트를 여기에 드래그 앤 드롭 하세요.")]
    [SerializeField] private Transform backgroundTransform;

    private Camera mainCamera;
    private float minX, maxX;
    private float minY, maxY;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        mainCamera = Camera.main;

        // 1. 플레이어 스프라이트 크기 계산 (화면 끝에 걸치게 설정)
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            objectWidth = spriteRenderer.bounds.extents.x;
            objectHeight = spriteRenderer.bounds.extents.y;
        }

        // 2. 배경 오브젝트를 기반으로 이동 제한 영역(Min/Max) 계산
        CalculateMapBounds();
    }

    void Update()
    {
        Move();
    }

    void LateUpdate()
    {
        RestrictMovement();
    }

    private void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(moveX, moveY, 0f).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void CalculateMapBounds()
    {
        if (backgroundTransform != null)
        {
            // 인스펙터 창의 Scale 값(X=10, Y=18)을 기준으로 맵의 중심에서 좌우, 상하 반경 계산
            float mapHalfWidth = backgroundTransform.localScale.x / 2f;
            float mapHalfHeight = backgroundTransform.localScale.y / 2f;
            Vector3 mapCenter = backgroundTransform.position;

            // 좌우(X축) 제한 범위 계산
            minX = mapCenter.x - mapHalfWidth + objectWidth;
            maxX = mapCenter.x + mapHalfWidth - objectWidth;

            // 상하(Y축) 제한 범위 계산 (카메라가 같이 움직이는 게임이라면 카메라 영역과 조합해야 할 수 있음)
            minY = mapCenter.y - mapHalfHeight + objectHeight;
            maxY = mapCenter.y + mapHalfHeight - objectHeight;
        }
        else if (mainCamera != null)
        {
            // 배경 오브젝트가 할당되지 않았을 때의 예외 처리 (카메라 기준 복원)
            float camHeight = mainCamera.orthographicSize;
            float camWidth = camHeight * mainCamera.aspect;

            minX = -camWidth + objectWidth;
            maxX = camWidth - objectWidth;
            minY = -camHeight + objectHeight;
            maxY = camHeight - objectHeight;

            Debug.LogWarning("PlayerController: 배경(Background) 오브젝트가 할당되지 않아 카메라 기준으로 제한합니다.");
        }
    }

    private void RestrictMovement()
    {
        Vector3 viewPos = transform.position;

        // 계산된 배경 범위 내로 플레이어 위치를 강제 가둠
        viewPos.x = Mathf.Clamp(viewPos.x, minX, maxX);
        viewPos.y = Mathf.Clamp(viewPos.y, minY, maxY);

        transform.position = viewPos;
    }
}
