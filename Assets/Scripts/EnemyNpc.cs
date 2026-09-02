using System.Collections;
using UnityEngine;

public class EnemyNpc : MonoBehaviour
{
    // [중요] 현재 5연발 탄막을 열심히 쏘고 있는 중인지 체크하는 안전장치 변수
    public bool IsBurstFiring { get; private set; } = false;
    private WaitForSeconds burstWaitObj = new WaitForSeconds(0.1f);
    // [오후 캐싱 적용] 딱 필요한 2가지 상태 객체만 메모리에 올려둡니다.
    public INpcState AppearStateObj { get; private set; }
    public INpcState CombatStateObj { get; private set; }
    private INpcState currentState;
    // [추가] 현재 장착 중인 사격 부품 부모 인터페이스
    public IAttackPattern CurrentAttackPattern { get; private set; }
    // [추가] 사용할 사격 부품들을 미리 생성하여 캐싱 (GC 발생 방지)
    public IAttackPattern SingleAttackComponent { get; private set; }
    public IAttackPattern RandomBurstAttackComponent { get; private set; }

    // 탄 발사용 프리랩과 총구 위치 컴포넌트
    public GameObject projectilePrefab;
    public Transform firePoint;

    void Start()
    {
        // 💡 [수치 복구 핵심] 0.5f를 0.12f로 대폭 줄여줍니다.
        // 탄과 탄 사이의 시간 간격을 좁혀야 사선 모양의 계단 궤적이 촘촘하게 출력됩니다.
        burstWaitObj = new WaitForSeconds(0.12f);

        // 1. 사용할 상태 객체 생성
        AppearStateObj = new AppearState();
        CombatStateObj = new HorizontalCombatState();

        // 2. 사격 패턴 부품들 조립 및 캐싱
        SingleAttackComponent = new SingleAttack();
        RandomBurstAttackComponent = new RandomBurstAttack();

        // 3. 최초로 사용할 사격 부품 장착
        SetAttackPattern(RandomBurstAttackComponent);

        // 4. 시작 위치 배치 및 상태 전환
        transform.position = new Vector3(0f, 8f, 0f);
        ChangeState(AppearStateObj);
    }

    // [추가] 언제든 원하는 무기(패턴)로 실시간 체인지해주는 함수
    public void SetAttackPattern(IAttackPattern newPattern)
    {
        CurrentAttackPattern = newPattern;
    }

    void Update()
    {
        // 매 프레임 현재 상태의 Update를 실행
        if (currentState != null)
        {
            currentState.Update(this);
        }
    }

    // 상태 전환 함수
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

    // 컴뱃 상태에서 호출할 실제 탄 발사 함수
    public void FireProjectile()
    {
        // 아까 추가했던 if (isBurstFiring) return; 코드는 완전히 지워주세요!
        // (이 코드가 있으면 타이밍이 꼬여 코루틴까지 멈추게 됩니다.)

        if (firePoint != null)
        {
            GameObject bullet = ProjectilePool.Instance.GetProjectile();
            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = firePoint.rotation;
                bullet.SetActive(true);
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
        IsBurstFiring = true; // 문 잠금
        int shootCount = 5;

        for (int i = 0; i < shootCount; i++)
        {
            if (firePoint != null)
            {
                GameObject bullet = ProjectilePool.Instance.GetProjectile();
                if (bullet != null)
                {
                    bullet.transform.position = new Vector3(firePoint.position.x, firePoint.position.y, 0f);
                    bullet.transform.rotation = firePoint.rotation;
                    bullet.SetActive(true);
                }
            }
            yield return burstWaitObj;
        }
        IsBurstFiring = false; // 문 열기
    }
}
