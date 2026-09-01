using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 7f; // 조작감이 쾌적하도록 속도를 살짝 올렸습니다.

    [Header("맵 제한 설정")]
    [SerializeField] private Transform backgroundTransform;

    private float minX, maxX;
    private float minY, maxY;
    private float objectWidth;
    private float objectHeight;

    void Start()
    {
        // 플레이어 스프라이트 반경 계산
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            objectWidth = spriteRenderer.bounds.extents.x;
            objectHeight = spriteRenderer.bounds.extents.y;
        }

        // 사막 맵 기준 경계선 계산
        if (backgroundTransform != null)
        {
            float mapHalfWidth = backgroundTransform.localScale.x / 2f;
            float mapHalfHeight = backgroundTransform.localScale.y / 2f;
            Vector3 mapCenter = backgroundTransform.position;

            minX = mapCenter.x - mapHalfWidth + objectWidth;
            maxX = mapCenter.x + mapHalfWidth - objectWidth;
            minY = mapCenter.y - mapHalfHeight + objectHeight;
            maxY = mapCenter.y + mapHalfHeight - objectHeight;
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
        // GetAxisRaw는 소수점 없이 -1, 0, 1만 반환하므로 반응이 즉각적이고 미끄러지지 않습니다.
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 대각선 이동 시 속도가 1.4배 빨라지는 것을 방지 (.normalized)
        Vector3 moveDirection = new Vector3(moveX, moveY, 0f).normalized;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void RestrictMovement()
    {
        if (backgroundTransform == null) return;

        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, minX, maxX);
        viewPos.y = Mathf.Clamp(viewPos.y, minY, maxY);
        transform.position = viewPos;
    }
}
