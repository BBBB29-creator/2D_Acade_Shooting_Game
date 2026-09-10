using System.Collections;
using UnityEngine;

public class NormalEnemyController : MonoBehaviour
{
    private EnemyCharacter enemyCharacter;
    private EnemyNpc enemyNpc;

    [Header("=== 이동 및 패턴 설정 ===")]
    private float moveSpeed = 3f;

    private enum AIPattern { DiagonalFormation, HitAndRun }
    private AIPattern chosenPattern;

    private Vector3 moveDirection;

    private enum HitAndRunState { MoveDown, Shooting, Retreat }
    private HitAndRunState currentHRState;
    private float stopY;

    [Header("=== 화면 외 삭제 경계선 ===")]
    private const float DESTROY_Y = -6.5f;

    private void Awake()
    {
        enemyCharacter = GetComponent<EnemyCharacter>();
        enemyNpc = GetComponent<EnemyNpc>();
    }

    private void OnEnable()
    {
        if (enemyCharacter != null)
        {
            moveSpeed = enemyCharacter.GetMoveSpeed();
        }

        chosenPattern = (Random.value > 0.5f) ? AIPattern.DiagonalFormation : AIPattern.HitAndRun;

        if (chosenPattern == AIPattern.DiagonalFormation)
        {
            // [기획적 보완] 화면 끝에서 태어나 바깥으로 증발하는 현상 완벽 차단!
            // 스폰된 X 좌표가 왼쪽에 있다면, 무조건 우하향 대각선으로 화면 중심을 통과하게 만듭니다.
            if (transform.position.x < 0)
            {
                // 오른쪽 아래 대각선 (X값을 0.8f로 늘려 더 과감하게 안쪽으로 파고들게 함)
                moveDirection = new Vector3(0.8f, -1f, 0f).normalized;
            }
            // 스폰된 X 좌표가 오른쪽에 있다면, 무조건 좌하향 대각선으로 화면 중심을 통과하게 만듭니다.
            else
            {
                // 왼쪽 아래 대각선
                moveDirection = new Vector3(-0.8f, -1f, 0f).normalized;
            }
        }
        else
        {
            stopY = Random.Range(2.5f, 4.5f);
            currentHRState = HitAndRunState.MoveDown;
        }

        // 안전 지연 사격 루틴 시작
        StartCoroutine(SafeInitAttackRoutine());
    }

    // 💡 OnEnable 바로 밑에 새롭게 추가해 줄 안전지연 코루틴 함수입니다.
    private IEnumerator SafeInitAttackRoutine()
    {
        // 유니티 시스템 내부에서 모든 매니저(오브젝트 풀 포함)가 완벽히 셋업될 때까지 딱 1프레임 쉼호흡합니다.
        yield return null;

        // 공장이 잘 켜졌는지 2차 검사
        if (ObjectPoolManager.Instance == null) yield break;

        // 1. 노말 적에게 새로 만든 플레이어 정밀 조준 패턴을 안전하게 장착합니다.
        if (enemyNpc != null)
        {
            enemyNpc.SetAttackPattern(new PlayerTargetAttack());
        }

        // 2. 대각선 돌격 패턴일 경우, 이제 정상 가동하는 풀 매니저를 통해 조준탄 1발 사격!
        if (chosenPattern == AIPattern.DiagonalFormation)
        {
            if (enemyNpc != null && enemyNpc.CurrentAttackPattern != null)
            {
                enemyNpc.CurrentAttackPattern.ExecuteAttack(enemyNpc);
            }
        }
    }


    void Update()
    {
        if (chosenPattern == AIPattern.DiagonalFormation)
        {
            UpdateDiagonalFormation();
        }
        else
        {
            UpdateHitAndRun();
        }
    }

    private void UpdateDiagonalFormation()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        if (transform.position.y <= DESTROY_Y)
        {
            ReturnToPool();
        }
    }

    private void UpdateHitAndRun()
    {
        switch (currentHRState)
        {
            case HitAndRunState.MoveDown:
                transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);
                if (transform.position.y <= stopY)
                {
                    transform.position = new Vector3(transform.position.x, stopY, 0f);
                    currentHRState = HitAndRunState.Shooting;
                    StartCoroutine(AttackAndRetreatRoutine());
                }
                break;

            case HitAndRunState.Retreat:
                transform.Translate(Vector3.up * (moveSpeed * 1.5f) * Time.deltaTime, Space.World);
                if (transform.position.y > 8f)
                {
                    ReturnToPool();
                }
                break;
        }
    }

    private IEnumerator AttackAndRetreatRoutine()
    {
        // 정지 상태에서도 새로 주입된 조준 패턴으로 딱 1발 정밀 사격!
        if (enemyNpc != null && enemyNpc.CurrentAttackPattern != null)
        {
            enemyNpc.CurrentAttackPattern.ExecuteAttack(enemyNpc);
        }

        yield return new WaitForSeconds(1.2f);
        currentHRState = HitAndRunState.Retreat;
    }

    private void ReturnToPool()
    {
        StopAllCoroutines();

        if (enemyCharacter != null)
        {
            enemyCharacter.ExecuteDeath();
        }
        else if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReleaseObject(gameObject, "NormalEnemy");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
