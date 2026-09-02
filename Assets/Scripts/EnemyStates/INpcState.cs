using UnityEngine;

public interface INpcState
{
    public void Enter(EnemyNpc npc);  // 상태 진입 시 실행
    public void Update(EnemyNpc npc); // 상태 유지 중 실행 (매 프레임)
    public void Exit(EnemyNpc npc);   // 상태 탈출 시 실행
}