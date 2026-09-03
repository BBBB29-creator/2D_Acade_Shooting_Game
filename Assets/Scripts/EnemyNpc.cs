using System.Collections;
using UnityEngine;

public class EnemyNpc : MonoBehaviour
{
    // [기존 정석 수치 및 변수들 100% 원본 복구]
    public bool IsBurstFiring { get; private set; } = false;
    private WaitForSeconds burstWaitObj = new WaitForSeconds(0.1f);

    public INpcState AppearStateObj { get; private set; }
    public INpcState CombatStateObj { get; private set; }
    private INpcState currentState;

    public IAttackPattern CurrentAttackPattern { get; private set; }
    public IAttackPattern SingleAttackComponent { get; private set; }
    public IAttackPattern RandomBurstAttackComponent { get; private set; }

    public GameObject projectilePrefab;
    public Transform firePoint;

    // 원래 사용하시던 Start() 초기화 구조로 완벽히 복구했습니다!
    void Start()
    {
        burstWaitObj = new WaitForSeconds(0.12f);

        // 1. 사용할 상태 객체 생성
        AppearStateObj = new AppearState();
        CombatStateObj = new HorizontalCombatState();

        // 2. 사격 패턴 부품들 조립 및 캐싱
        SingleAttackComponent = new SingleAttack();
        RandomBurstAttackComponent = new RandomBurstAttack();

        // 3. 최초로 사용할 사격 부품 장착
        SetAttackPattern(RandomBurstAttackComponent);

        // 4. 원래 적 고유의 시작 위치 배치 및 시동
        transform.position = new Vector3(0f, 8f, 0f);
        ChangeState(AppearStateObj);
    }

    private void OnEnable()
    {
        // 최초 Start()가 실행되기 전인 게임 극초반 null 에러를 방지합니다.
        if (AppearStateObj != null)
        {
            currentState = null;
            transform.position = new Vector3(0f, 8f, 0f); // 원래 적의 고유 스폰 Y축 복구
            ChangeState(AppearStateObj);
        }
    }

    public void SetAttackPattern(IAttackPattern newPattern)
    {
        CurrentAttackPattern = newPattern;
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.Update(this);
        }
    }

    public void ChangeState(INpcState newState)
    {
        if (currentState == newState) return;

        if (currentState != null)
        {
            currentState.Exit(this);
        }

        currentState = newState;
        currentState.Enter(this);
    }

    // 적 전용 총알("EnemyBullet")을 풀 매니저에서 안전하게 꺼내 쏘도록 유지
    public void FireProjectile()
    {
        if (firePoint != null)
        {
            GameObject bullet = ObjectPoolManager.Instance.GetObject("EnemyBullet");
            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = firePoint.rotation;
            }
        }
    }

    public void FireBurstPattern()
    {
        if (IsBurstFiring) return;
        StartCoroutine(BurstFireRoutine());
    }

    private IEnumerator BurstFireRoutine()
    {
        IsBurstFiring = true;
        int shootCount = 5;

        for (int i = 0; i < shootCount; i++)
        {
            if (firePoint != null)
            {
                GameObject bullet = ObjectPoolManager.Instance.GetObject("EnemyBullet");
                if (bullet != null)
                {
                    bullet.transform.position = new Vector3(firePoint.position.x, firePoint.position.y, 0f);
                    bullet.transform.rotation = firePoint.rotation;
                }
            }
            yield return burstWaitObj;
        }
        IsBurstFiring = false;
    }
}
