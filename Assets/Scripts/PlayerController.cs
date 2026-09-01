using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 7f;

    [Header("최적화 배경 설정")]
    [Tooltip("Hierarchy 창에 있는 '14' 오브젝트(사막 맵)를 여기에 드래그해서 연결하세요.")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    private float minX, maxX;
    private float minY, maxY;

    void Start()
    {
        float objectWidth = 0.5f;
        float objectHeight = 0.5f;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            objectWidth = spriteRenderer.bounds.extents.x;
            objectHeight = spriteRenderer.bounds.extents.y;
        }

        // [변경] GameObject.Find 문자열 탐색을 완전히 지우고, 드래그 연결된 렌더러를 바로 사용 (가비지 0B)
        if (backgroundRenderer != null)
        {
            Bounds mapBounds = backgroundRenderer.bounds;
            minX = mapBounds.min.x + objectWidth;
            maxX = mapBounds.max.x - objectWidth;
        }
        else
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                float camWidth = mainCam.orthographicSize * mainCam.aspect;
                minX = -camWidth + objectWidth;
                maxX = camWidth - objectWidth;
            }
            Debug.LogWarning("PlayerController: 배경 렌더러가 지정되지 않아 카메라 기준으로 좌우를 제한합니다.");
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float camHeight = mainCamera.orthographicSize;
            Vector3 camPosition = mainCamera.transform.position;

            minY = camPosition.y - camHeight + objectHeight;
            maxY = camPosition.y + camHeight - objectHeight;
        }
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
        // GetAxisRaw는 소수점 가비지 없이 정수(-1, 0, 1)만 튀어나오므로 안전합니다.
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Vector3는 구조체(Value Type)라 스택 메모리를 써서 GC를 유발하지 않습니다.
        Vector3 moveDirection = new Vector3(moveX, moveY, 0f).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void RestrictMovement()
    {
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, minX, maxX);
        viewPos.y = Mathf.Clamp(viewPos.y, minY, maxY);
        transform.position = viewPos;
    }
}
