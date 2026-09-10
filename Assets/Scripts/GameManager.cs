using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 외부에서 접근할 수 있는 유일한 통로 (싱글톤)
    public static GameManager Instance { get; private set; }

    [Header("=== 게임 오버 연출 UI ===")]
    [Tooltip("인게임 Canvas 하위에 만든 GAME OVER 텍스트나 스프라이트 이미지 오브젝트")]
    [SerializeField] private GameObject gameOverUI;

    [Tooltip("화면 전체를 덮을 검은색 UI 이미지 (암전 패널)")]
    [SerializeField] private Image blackoutPanel;

    private void Awake()
    {
        // 중복 생성 차단 방어막
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 게임 시작 시 관련 UI는 모두 꺼둡니다.
        if (gameOverUI != null) gameOverUI.SetActive(false);

        if (blackoutPanel != null)
        {
            blackoutPanel.gameObject.SetActive(true);
            // 처음에는 완전히 투명하게(Alpha = 0) 설정
            Color c = blackoutPanel.color;
            c.a = 0f;
            blackoutPanel.color = c;
        }

        // 혹시 이전 게임 오버로 인해 느려진 시간을 정상(1배속)으로 초기화
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 플레이어가 죽는 순간 호출되는 게임오버 이벤트 사령탑
    /// </summary>
    public void TriggerGameOver()
    {
        StartCoroutine(GameOverPresentationRoutine());
    }

    // 🎬 슬로우 모션 + 암전 + UI 메시지 연출 코루틴
    private IEnumerator GameOverPresentationRoutine()
    {
        float duration = 2.0f; // 연출이 진행될 총 시간 (2초)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 게임 내 속도(Time.timeScale)와 관계없이 흐르는 실제 절대 시간(unscaledDeltaTime) 사용
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = elapsed / duration;

            // 1. 프레임이 느려지는 연출 (시간 배율을 1.0에서 0.1 초슬로우까지 서서히 낮춤)
            Time.timeScale = Mathf.Lerp(1.0f, 0.1f, normalizedTime);

            // 2. 화면이 조금씩 어두워지는 연출 (검은 패널의 알파값을 0에서 0.75까지 높임)
            if (blackoutPanel != null)
            {
                Color c = blackoutPanel.color;
                c.a = Mathf.Lerp(0f, 0.75f, normalizedTime);
                blackoutPanel.color = c;
            }

            yield return null;
        }

        // 연출 완료 후 완전히 멈추지 않고 초슬로우 상태(0.05배속) 고정
        Time.timeScale = 0.05f;

        // 3. 가운데 뜨는 GAME OVER UI 메시지/스프라이트 활성화
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    /// <summary>
    /// [버튼 연결용] 현재 인게임 씬을 처음부터 다시 시작
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // 재시작할 때는 반드시 시간 배율을 1로 돌려놔야 합니다!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// [버튼 연결용] 타이틀에서 게임 스테이지("InGame")로 전환
    /// </summary>
    public void LoadGameStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("InGame");
    }
}
