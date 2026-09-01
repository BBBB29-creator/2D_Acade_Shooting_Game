using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("스크롤 속도")]
    [SerializeField] private float scrollSpeed = 0.2f;

    private Material backgroundMaterial;
    private Vector2 savedOffset;

    void Start()
    {
        // Quad에 적용된 Renderer 컴포넌트에서 머티리얼을 가져옵니다.
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            backgroundMaterial = meshRenderer.material;
        }

        // 현재 오프셋 값을 초기화합니다.
        savedOffset = backgroundMaterial.mainTextureOffset;
    }

    void Update()
    {
        // 시간에 따라 Y축 오프셋 값을 증가시킵니다. (종스크롤 타입)
        float newOffsetY = Mathf.Repeat(Time.time * scrollSpeed, 1f);

        // 새로운 오프셋 값을 벡터로 만듭니다.
        Vector2 offset = new Vector2(savedOffset.x, newOffsetY);

        // 머티리얼에 오프셋을 적용하여 이미지를 움직입니다.
        backgroundMaterial.mainTextureOffset = offset;
    }
}
