using UnityEngine;

public class PlayerTargetAttack : IAttackPattern
{
    public void ExecuteAttack(EnemyNpc npc)
    {
        if (npc == null || npc.firePoint == null) return;

        GameObject bullet = ObjectPoolManager.Instance.GetObject("EnemyBullet");
        if (bullet == null) return;

        bullet.transform.position = npc.firePoint.position;
        Vector3 shootDirection = Vector3.down;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            shootDirection = (player.transform.position - npc.firePoint.position).normalized;
        }

        // 💡 [핵심 연동] 총알의 Kinematic 세팅을 완벽히 무력화하고 
        // 조준 방향과 고속 스피드(8.5f)를 다이렉트로 주입합니다!
        if (bullet.TryGetComponent<EnemyProjectile>(out var projectile))
        {
            projectile.SetTargetingDirection(shootDirection, 8.5f);
        }

        // 각도 회전 정렬
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle + 270f);
    }
}
