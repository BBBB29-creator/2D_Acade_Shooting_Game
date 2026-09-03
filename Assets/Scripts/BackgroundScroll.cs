using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("스크롤 속도")]
    [SerializeField] private float scrollSpeed = 3f;

    private MeshRenderer meshRenderer;
    private Material targetMaterial;

    // Awake()를 Start()로 변경하여 초기화 타이밍 안정성을 확보합니다.
    void Start()
    {
        // 유니티가 인스펙터 컴포넌트 조립을 완료한 후 안전하게 가져옵니다.
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            targetMaterial = meshRenderer.material;
        }
        else
        {
            Debug.LogError("BackgroundScroll: Mesh Renderer를 찾을 수 없습니다! 오브젝트에 Quad 세팅을 확인하세요.");
        }
    }

    void Update()
    {
        if (targetMaterial != null)
        {
            Vector2 offset = targetMaterial.mainTextureOffset;
            offset.y += scrollSpeed * Time.deltaTime;
            targetMaterial.mainTextureOffset = offset;
        }
    }
}
