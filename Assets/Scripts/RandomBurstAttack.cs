using UnityEngine;

public class RandomBurstAttack : IAttackPattern
{
    public void ExecuteAttack(EnemyNpc npc)
    {
        // 0부터 100 사이의 무작위 주사위를 굴립니다.
        float randomChance = Random.Range(0f, 100f);

        // [매칭 확인] 30% 이하일 때 'FireBurstPattern'이 정확히 매칭되어야 합니다.
        if (randomChance <= 30f)
        {
            Debug.Log("부품 작동: 30% 확률 당첨! 5연발 사격");
            npc.FireBurstPattern(); // 5연발 함수 호출
        }
        else
        {
            Debug.Log("부품 작동: 일반 1발 사격");
            npc.FireProjectile();  // 1발 함수 호출
        }
    }
}
