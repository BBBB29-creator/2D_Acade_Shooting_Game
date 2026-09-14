using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHp = 10;
    private int hp;

    [Header("=== 캐릭터 타입 설정 ===")]
    [SerializeField] private bool isPlayer = false;

    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;
    private bool isFlashing = false;
    private bool isDead = false;

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
        isDead = false;
        isFlashing = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }

        if (TryGetComponent<Collider2D>(out var col)) col.enabled = true;
    }

    public void Setup(int newMaxHp)
    {
        maxHp = newMaxHp;
        hp = maxHp;
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        hp -= damage;
        Debug.Log($"{gameObject.name} hp: {hp}");

        if (spriteRenderer != null && !isFlashing && gameObject.activeInHierarchy)
        {
            StartCoroutine(FlashRoutine());
        }

        if (hp > 0) return;

        // 사망 확정
        isDead = true;

        // ==========================================
        // 💥 [공통] 폭발 프리팹 생성
        // ==========================================
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // ==========================================
        // 🕹️ 플레이어 사망 (즉시 증발 구조 복원)
        // ==========================================
        if (isPlayer)
        {
            // 🎯 [완벽 복구]: 피가 0이 되는 0초 만에 내 비행기 상자를 통째로 꺼버립니다!
            // 이제 폭발이 터지는 순간 비행기 이미지가 뒤에 겹쳐서 남는 버그가 무조건 해결됩니다.
            gameObject.SetActive(false);

            // 비행기가 깔끔하게 사라진 직후, 슬로우 모션 사령탑을 작동시킵니다.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
        // ==========================================
        // 🛸 적 사망
        // ==========================================
        else
        {
            if (TryGetComponent<EnemyCharacter>(out var enemyCharacter)) enemyCharacter.ExecuteDeath();
            else gameObject.SetActive(false);
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

    public int GetHp() => hp;
    public int GetMaxHp() => maxHp;
}
