using UnityEngine;
using TMPro; // TextMeshPro를 사용한다면 주석을 해제하고 사용하세요.
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI 연동")]
    [SerializeField] private TMP_Text scoreText;

    private int currentScore = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE : {currentScore:D6}"; // 000120 같은 형태로 예쁘게 출력
        }
    }
}
