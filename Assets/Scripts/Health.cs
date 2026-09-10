using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHp = 10;
    private int hp;

    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;
    private bool isFlashing = false;
    private WaitForSeconds flashWait = new WaitForSeconds(0.05f);

    [Header("=== Explosion Prefab ===")]
    [SerializeField] private GameObject explosionPrefab;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        hp = maxHp;
        isFlashing = false;
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }

    public void Setup(int newMaxHp)
    {
        maxHp = newMaxHp;
        hp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log($"{gameObject.name} hp: {hp}");

        if (spriteRenderer != null && !isFlashing && gameObject.activeInHierarchy)
        {
            StartCoroutine(FlashRoutine());
        }

        // 체력이 0 이하가 되었을 때만 사망 처리
        if (hp <= 0)
        {
            if (TryGetComponent<EnemyCharacter>(out var enemyCharacter))
            {
                enemyCharacter.ExecuteDeath();
            }
            else
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.TriggerGameOver();
                }

                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator FlashRoutine()
    {
        isFlashing = true;
        for (int i = 0; i < 4; i++)
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.4f);
            yield return flashWait;
            spriteRenderer.color = originalColor;
            yield return flashWait;
        }
        isFlashing = false;
    }
}
