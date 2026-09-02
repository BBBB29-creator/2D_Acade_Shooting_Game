using UnityEngine;

public class SingleAttack : IAttackPattern
{
    public void ExecuteAttack(EnemyNpc npc)
    {
        npc.FireProjectile(); // 기존에 만든 1발 발사 호출
    }
}
