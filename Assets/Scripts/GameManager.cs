using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 💡 화면을 어둡게 덮을 Image 컴포넌트 제어를 위해 추가

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("=== 게임오버 연출 설정 ===")]
    [Tooltip("슬로우 모션 배율 (0f에 가까울수록 시간이 극도로 느려집니다)")]
    [SerializeField] private float slowMotionScale = 0.2f;

    [Header("=== UI 및 연출 연동 ===")]
    [Tooltip("상시 켜두는 그 부모 UI 판넬을 그대로 연결")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("화면을 조금씩 어둡게 채울 검은색 배경 이미지 컴포넌트")]
    [SerializeField] private Image blackFadeImage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        // [자동 청소] 플레이 모드를 켜자마자 UI를 끄고, 시간 배율을 정상(1f)으로 리셋합니다.
        Time.timeScale = 1f;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (blackFadeImage != null) blackFadeImage.color = new Color(0f, 0f, 0f, 0f);
    }

    // 💡 플레이어의 Health.cs에서 죽는 순간 이 함수를 쾅 때려줍니다.
    public void TriggerGameOver()
    {
        // 🎯 [핵심 기믹 1]: 게임의 전반적인 흐름 속도를 20% 수준으로 뚝 떨어트려 
        // 폭발 이펙트와 주변 적들이 웅장하고 느리게 움직이는 슬로우 모션을 연출합니다.
        Time.timeScale = slowMotionScale;

        // 🎯 [핵심 기믹 2]: 화면을 부드럽게 어둡게 덮으면서 글자를 띄우는 시퀀스 가동!
        StartCoroutine(GameOverPresentationRoutine());
    }

    // 🎬 오락실 정석 지연 및 페이드 연출 코루틴
    private IEnumerator GameOverPresentationRoutine()
    {
        // 1. 부모 UI 판넬을 즉시 켜서 자식들의 전원을 공급합니다.
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // 2. 검은 배경 이미지의 투명도(Alpha)를 조금씩 올리며 화면을 서서히 어둡게 만듭니다.
        // 슬로우 모션 영향을 받지 않도록 WaitForSecondsRealtime을 사용합니다.
        float alpha = 0f;
        while (alpha < 0.85f) // 완전히 새까맣게 변하기 직전(85%)까지 채우기
        {
            alpha += 0.05f;
            if (blackFadeImage != null) blackFadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForSecondsRealtime(0.05f);
        }

        Debug.Log("페이드아웃 완료. 5초 대기 후 타이틀 씬으로 복귀합니다.");

        // 3. 화면이 다 어두워진 상태로 유저가 여운을 느낄 수 있도록 5초간 대기합니다.
        yield return new WaitForSecondsRealtime(5.0f);

        // 4. 다음 판을 위해 시간 배율을 정방향(1f)으로 완벽 복구해 두고 복귀합니다!
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
