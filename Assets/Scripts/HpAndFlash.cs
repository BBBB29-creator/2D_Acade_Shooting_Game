using System.Collections;
using UnityEngine;

public class HpAndFlash : MonoBehaviour
{
    public int hp = 10;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;

    // 가비지 방지 및 빠른 3시 빌드를 위해 대기 시간 객체 미리 생성 (0.05초로 더 타이트하게 변경)
    private WaitForSeconds flashWait = new WaitForSeconds(0.05f);

    void Awake()
    {
        // 💡 [중요] 자기 자신 또는 자식 오브젝트에 있는 SpriteRenderer까지 싹 긁어와서 참조합니다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} 피격 발생! 남은 체력: {hp}");

        // 💡 오브젝트가 켜져있고, SpriteRenderer가 존재할 때만 깜빡임 코루틴을 강제 구동합니다.
        if (spriteRenderer != null && !isFlashing && gameObject.activeInHierarchy)
        {
            StartCoroutine(FlashRoutine());
        }

        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator FlashRoutine()
    {
        isFlashing = true;

        // 💡 [시각 효과 극대화] 단순히 빨간색으로만 바꾸면 배경이나 기체 색상에 묻힐 수 있으므로
        // 빨간색 투명(알파값 차감)과 원래 색상을 4번 빠르게 교차시켜 눈에 무조건 띄게 만듭니다.
        for (int i = 0; i < 4; i++)
        {
            // 1. 피격 상태 (빨간색 변환 + 살짝 투명화로 깜빡임 유도)
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.4f);
            yield return flashWait;

            // 2. 정상 상태 복구
            spriteRenderer.color = originalColor;
            yield return flashWait;
        }

        // 코루틴 종료 시 무조건 원래 색상으로 안전 복귀 명시
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }
}
