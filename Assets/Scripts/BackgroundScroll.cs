using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("스크롤 속도")]
    [SerializeField] private float scrollSpeed = 3f;

    private float backgroundHeight;
    private Transform myTransform;
    private Transform cloneTransform;

    void Start()
    {
        myTransform = transform;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // 1. 이미지의 실제 세로 크기(Height) 계산
            backgroundHeight = spriteRenderer.bounds.size.y;

            // 2. 맵 루프를 위해 똑같은 이미지를 위에 하나 더 복제(Clone) 생성
            GameObject clone = Instantiate(gameObject, myTransform.parent);
            Destroy(clone.GetComponent<BackgroundScroll>()); // 무한 복제 방지

            cloneTransform = clone.transform;
            // 복제본을 원본 바로 위에 딱 붙여서 배치
            cloneTransform.position = myTransform.position + Vector3.up * backgroundHeight;
        }
        else
        {
            Debug.LogError("BackgroundScroll: Sprite Renderer를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        if (backgroundHeight <= 0) return;

        // 3. 두 개의 배경을 동시에 아래로 이동
        float moveAmount = scrollSpeed * Time.deltaTime;
        myTransform.position += Vector3.down * moveAmount;
        cloneTransform.position += Vector3.down * moveAmount;

        // 4. 화면 아래로 완전히 내려가면 다시 위로 올려서 무한 루프 구현
        if (myTransform.position.y <= -backgroundHeight)
        {
            myTransform.position = cloneTransform.position + Vector3.up * backgroundHeight;
        }
        if (cloneTransform.position.y <= -backgroundHeight)
        {
            cloneTransform.position = myTransform.position + Vector3.up * backgroundHeight;
        }
    }
}
