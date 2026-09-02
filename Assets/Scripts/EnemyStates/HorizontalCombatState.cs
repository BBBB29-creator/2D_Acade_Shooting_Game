using UnityEngine;

public class HorizontalCombatState : INpcState
{
    private float moveSpeed = 3f;      // 좌우 이동 속도
    private float moveRange = 4f;      // 정중앙으로부터 한쪽으로 움직일 최대 반경

    private float startX;              // 안착 지점의 중심 X 좌표
    private int moveDirection = 1;     // 이동 방향 (1: 우측, -1: 좌측)

    private float attackCooldown = 0.5f;
    private float attackTimer = 0f;

    public void Enter(EnemyNpc npc)
    {
        Debug.Log("적: 패턴 돌입 (방향 전환 기반 등속 무빙)");

        // 안착한 그 순간의 X 좌표를 중심점으로 완벽히 저장
        startX = npc.transform.position.x;

        // 시작 시 우측(1) 방향으로 설정, 타이머 초기화
        moveDirection = 1;
        attackTimer = 0f;
    }

    public void Update(EnemyNpc npc)
    {
        // 1. [순간이동 해결] 방향에 따라 정속으로 매 프레임 좌표를 이동시킵니다.
        Vector3 currentPos = npc.transform.position;
        currentPos.x += moveDirection * moveSpeed * Time.deltaTime;
        npc.transform.position = currentPos;

        // 2. 중심점(startX)으로부터 반경(moveRange)을 벗어나면 즉시 방향을 꺾습니다.
        if (npc.transform.position.x >= startX + moveRange)
        {
            // 우측 한계선에 도달하면 좌측으로 방향 전환
            moveDirection = -1;

            // 위치가 한계선을 초과하지 않도록 보정
            Vector3 clampedPos = npc.transform.position;
            clampedPos.x = startX + moveRange;
            npc.transform.position = clampedPos;
        }
        else if (npc.transform.position.x <= startX - moveRange)
        {
            // 좌측 한계선에 도달하면 우측으로 방향 전환
            moveDirection = 1;

            // 위치가 한계선을 초과하지 않도록 보정
            Vector3 clampedPos = npc.transform.position;
            clampedPos.x = startX - moveRange;
            npc.transform.position = clampedPos;
        }

        // 3. 탄 발사 로직 (기존 유지)
        attackTimer += Time.deltaTime;
        
        while (attackTimer >= attackCooldown) // if 대신 while을 쓰고 차감형태로 정밀 제어
        {
            npc.CurrentAttackPattern.ExecuteAttack(npc);
            
            attackTimer -= attackCooldown; // [수정] 0f 대신 정확히 쿨타임만큼만 차감합니다.
        }
    }

    public void Exit(EnemyNpc npc)
    {
        Debug.Log("적: 전투 패턴 종료");
    }
}
