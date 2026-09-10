using System.Collections;
using UnityEngine;

public class EnemyNpc : MonoBehaviour
{
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

    [SerializeField]
    private Transform shadow;

    void Start()
    {
        burstWaitObj = new WaitForSeconds(0.12f);

        AppearStateObj = new AppearState();
        CombatStateObj = new HorizontalCombatState();

        SingleAttackComponent = new SingleAttack();
        RandomBurstAttackComponent = new RandomBurstAttack();

        SetAttackPattern(RandomBurstAttackComponent);

        transform.position = new Vector3(0f, 8f, 0f);
        ChangeState(AppearStateObj);
    }

    private void OnEnable()
    {
        if (AppearStateObj != null)
        {
            currentState = null;
            transform.position = new Vector3(0f, 8f, 0f);
            ChangeState(AppearStateObj);
        }

        if (shadow != null)
        {
            shadow.gameObject.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        if (shadow != null)
        {
            shadow.position = transform.position + new Vector3(0f, -3f, 0f);
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
                    bullet.transform.position = new Vector3(
                        firePoint.position.x,
                        firePoint.position.y,
                        0f
                    );

                    bullet.transform.rotation = firePoint.rotation;
                }
            }

            yield return burstWaitObj;
        }

        IsBurstFiring = false;
    }
}