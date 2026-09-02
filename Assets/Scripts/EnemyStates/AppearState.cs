using UnityEngine;

public class AppearState : INpcState
{
    private float moveSpeed = 2f; // 천천히 등장하는 속도
    private float targetY = 3.5f; // 화면 내 안착할 목표 Y 좌표

    public void Enter(EnemyNpc npc)
    {
        Debug.Log("적: 화면 위에서 천천히 등장 시작");
    }

    public void Update(EnemyNpc npc)
    {
        // Y축만 아래로 천천히 이동
        Vector3 currentPos = npc.transform.position;
        currentPos.y -= moveSpeed * Time.deltaTime;
        npc.transform.position = currentPos;

        // 목표 지점(또는 그 이하)에 도달했는지 체크
        if (npc.transform.position.y <= targetY)
        {
            // 목표 좌표에 정확히 고정 후, 전투(공격) 상태로 전환
            Vector3 finalPos = npc.transform.position;
            finalPos.y = targetY;
            npc.transform.position = finalPos;

            npc.ChangeState(npc.CombatStateObj); // 💡 전투 상태로 자동 전환
        }
    }

    public void Exit(EnemyNpc npc)
    {
        Debug.Log("적: 지정된 위치 안착 완료, 등장 상태 종료");
    }
}
