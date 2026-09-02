using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float moveSpeed = 8f;
    private float destroyY = -6f;

    // 💡 탄환을 발사한 태그를 구별하여 자해 방지 (예: "Enemy" 또는 "Player")
    public string ownerTag = "Enemy";

    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);

        if (transform.position.y <= destroyY || transform.position.y >= 10f)
        {
            gameObject.SetActive(false);
        }
    }

    // [충돌 감지 핵심] 2D 콜라이더 Trigger 충돌 시 실행
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // [에러 종결 핵심] 만약 어떤 이유로든 ownerTag가 null이거나 비어있다면, 
        // 시스템이 멈추지 않도록 강제로 "Enemy" 태그를 채워 넣어 유니티의 CompareTag 에러를 완전히 차단합니다.
        if (string.IsNullOrEmpty(ownerTag))
        {
            ownerTag = "Enemy";
        }

        // 자기가 쏜 탄에 자기가 맞는 것 방지 (기존 코드)
        if (collision.CompareTag(ownerTag)) return;

        // 피격 및 데미지 처리 (기존 코드)
        HpAndFlash targetHp = collision.GetComponent<HpAndFlash>();
        if (targetHp != null)
        {
            targetHp.TakeDamage(1);
            gameObject.SetActive(false);
        }
    }
}
