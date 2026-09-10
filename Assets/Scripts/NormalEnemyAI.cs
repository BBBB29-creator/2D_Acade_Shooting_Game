using System.Collections;
using UnityEngine;

public class NormalEnemyAI : MonoBehaviour
{
    private EnemyCharacter enemyCharacter;
    private EnemyNpc enemyNpc; // 사격 연동용 컴포넌트
    private float speed = 3f;

    // 두 가지 패턴 정의
    private enum AIPattern { DiagonalFormation, HitAndRun }
    private AIPattern chosenPattern;

    // [패턴 1: 대각선용] 이동 방향 벡터
    private Vector3 moveDirection;

    // [패턴 2: 히트앤런용] 상태 및 목적지 Y
    private enum HitAndRunState { MoveDown, Shooting, Retreat }
    private HitAndRunState currentHRState;
    private float stopY;

    private void Awake()
    {
        enemyCharacter = GetComponent<EnemyCharacter>();
        enemyNpc = GetComponent<EnemyNpc>();
    }

    private void OnEnable()
    {
        // 1. 데이터베이스에서 속도 실시간 쿼리
        if (enemyCharacter != null)
        {
            speed = enemyCharacter.GetMoveSpeed();
        }

        // 💡 2. [핵심] 스폰되는 순간 50%의 확률로 패턴을 결정합니다.
        chosenPattern = (Random.value > 0.5f) ? AIPattern.DiagonalFormation : AIPattern.HitAndRun;

        // 3. 각 패턴별 초기 세팅
        if (chosenPattern == AIPattern.DiagonalFormation)
        {
            // [패턴 1] 중앙 기준 왼쪽에 있으면 우하향, 오른쪽에 있으면 좌하향 대각선 돌격
            if (transform.position.x < 0) moveDirection = new Vector3(0.6f, -1f, 0f).normalized;
            else moveDirection = new Vector3(-0.6f, -1f, 0f).normalized;
        }
        else
        {
            // [패턴 2] 급강하 후 멈출 위치를 Y 2.5 ~ 4.5 사이로 랜덤 지정
            stopY = Random.Range(2.5f, 4.5f);
            currentHRState = HitAndRunState.MoveDown;
        }
    }

    private void Update()
    {
        // 💡 실시간 선택된 패턴에 따라 다르게 행동합니다.
        if (chosenPattern == AIPattern.DiagonalFormation)
        {
            UpdateDiagonalFormation();
        }
        else
        {
            UpdateHitAndRun();
        }
    }

    // 🔽 패턴 1: 대각선 교차 편대 비행
    private void UpdateDiagonalFormation()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // 화면 밖(좌우로 완전히 탈출 시 포함)으로 나가면 풀 반환
        if (transform.position.y < -6f || Mathf.Abs(transform.position.x) > 6f)
        {
            ReturnToPool();
        }
    }

    // 🔽 패턴 2: 라이덴 스타일 급강하 정지 사격 후 퇴각
    private void UpdateHitAndRun()
    {
        switch (currentHRState)
        {
            case HitAndRunState.MoveDown:
                // 지정된 Y까지 빠르게 하강
                transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
                if (transform.position.y <= stopY)
                {
                    transform.position = new Vector3(transform.position.x, stopY, 0f);
                    currentHRState = HitAndRunState.Shooting;
                    StartCoroutine(AttackAndRetreatRoutine());
                }
                break;

            case HitAndRunState.Retreat:
                // 공격 끝났으니 화면 위쪽으로 급상승 퇴각 (속도 1.5배 보너스)
                transform.Translate(Vector3.up * (speed * 1.5f) * Time.deltaTime, Space.World);

                // 화면 위로 도망치면 풀 반환
                if (transform.position.y > 8f)
                {
                    ReturnToPool();
                }
                break;
        }
    }

    private IEnumerator AttackAndRetreatRoutine()
    {
        // 제자리 정지 상태에서 총알 발사 연출 시전
        if (enemyNpc != null)
        {
            // 기존 EnemyNpc의 단발 사격이나 점사를 취향껏 호출합니다.
            enemyNpc.FireBurstPattern();
        }

        // 탄알 난사하는 연출 시간 동안 제자리 대기 (약 1.2초)
        yield return new WaitForSeconds(1.2f);

        // 퇴각 상태로 전환하여 위로 질주
        currentHRState = HitAndRunState.Retreat;
    }

    private void ReturnToPool()
    {
        StopAllCoroutines();

        if (enemyCharacter != null)
        {
            enemyCharacter.ExecuteDeath();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
