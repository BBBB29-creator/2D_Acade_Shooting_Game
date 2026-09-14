using UnityEngine;

public class HealItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 플레이어가 아니면 즉시 차단합니다.
        if (!other.CompareTag("Player")) return;

        Health playerHealth = other.GetComponent<Health>();

        if (playerHealth != null)
        {
            int current = playerHealth.GetHp();
            int max = playerHealth.GetMaxHp();

            // 🎯 피가 깎여있는 상태라면 정직하게 체력을 1 회복시킵니다.
            if (current < max)
            {
                playerHealth.Setup(current + 1);
                Debug.Log("체력 1 회복 완료!");
            }
            // 🎯 [핵심 추가] 이미 풀 피(최대 체력)라면 스코어 매니저에게 보너스 점수를 넘깁니다!
            else
            {
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddScore(500); // 💡 보너스 점수 500점 주입 (원하는 숫자로 변경 가능)
                    Debug.Log("최대 체력 상태! 회복 아이템 보너스 점수 +500점 획득!");
                }
            }
        }

        // 아이템 획득 처리가 끝났으므로 소멸
        gameObject.SetActive(false);
    }
}
