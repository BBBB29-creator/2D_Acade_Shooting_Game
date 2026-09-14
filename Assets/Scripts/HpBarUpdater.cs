using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HpBarUpdater : MonoBehaviour
{
    [Header("=== 연결할 대상 ===")]
    [Tooltip("플레이어 기체의 Health 스크립트를 드래그해서 넣어주세요.")]
    [SerializeField] private Health playerHealth;

    [Header("=== 연출 설정 ===")]
    [Tooltip("체력이 0이 된 후 완전히 투명해지는 데 걸리는 시간 (초)")]
    [SerializeField] private float fadeDuration = 1.0f;

    private Slider hpSlider;
    private CanvasGroup canvasGroup;
    private bool isFading = false;

    private void Awake()
    {
        hpSlider = GetComponent<Slider>();

        // 투명도 조절을 위해 CanvasGroup 컴포넌트를 자동으로 가져오거나 없으면 붙여줍니다.
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        if (playerHealth != null && hpSlider != null)
        {
            hpSlider.minValue = 0;
            hpSlider.maxValue = 1;
            canvasGroup.alpha = 1f; // 시작할 때는 완전히 선명하게
            UpdateHpUI();
        }
    }

    private void Update()
    {
        // 이미 투명화 연출이 시작되었다면 실시간 업데이트를 중단합니다.
        if (isFading) return;

        UpdateHpUI();
    }

    private void UpdateHpUI()
    {
        if (playerHealth == null || hpSlider == null) return;

        int currentHp = playerHealth.GetHp();
        int maxHp = playerHealth.GetMaxHp();

        if (maxHp > 0)
        {
            float hpRatio = (float)currentHp / maxHp;
            hpSlider.value = Mathf.Clamp01(hpRatio);

            // 체력이 0 이하가 되는 순간 서서히 투명해지는 연출 시작!
            if (currentHp <= 0)
            {
                StartCoroutine(FadeOutHpBarRoutine());
            }
        }
    }

    //  UI가 서서히 투명해지는 코루틴 연출
    private IEnumerator FadeOutHpBarRoutine()
    {
        isFading = true;
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        // 게임오버 시 슬로우 모션(Time.timeScale)이 걸려도 
        // 투명화는 정상적인 속도로 부드럽게 흐르도록 unscaledDeltaTime을 사용합니다.
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f; // 마지막엔 완전히 투명하게 고정
        gameObject.SetActive(false); // 오브젝트도 깔끔하게 꺼버림
    }
}
