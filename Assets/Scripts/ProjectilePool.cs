using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    // 어디서나 접근 가능하도록 싱글톤 설정
    public static ProjectilePool Instance { get; private set; }

    [Header("설정")]
    [SerializeField] private GameObject projectilePrefab; // 탄환 프리팹
    [SerializeField] private int poolSize = 30;           // 미리 생성할 탄환 개수

    // 탄환들을 담아둘 풀(List)
    private List<GameObject> pooledProjectiles = new List<GameObject>();

    void Awake()
    {
        Instance = this;

        // 게임 시작 시 지정된 개수만큼 탄환을 미리 생성하여 꺼둡니다.
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false); // 💡 미리 비활성화 상태로 보관
            
            pooledProjectiles.Add(obj);
        }
    }

    // 💡 [핵심] 밖에서 탄환이 필요할 때 호출하는 함수
    public GameObject GetProjectile()
    {
        // 1. 꺼져 있는(비활성화된) 탄환이 있다면 재사용하기 위해 가져옵니다.
        for (int i = 0; i < pooledProjectiles.Count; i++)
        {
            if (!pooledProjectiles[i].activeInHierarchy)
            {
                return pooledProjectiles[i];
            }
        }

        // 2. 만약 탄환이 부족하면(화면에 탄이 너무 많음) 실시간으로 하나 더 만들어 풀에 보충합니다.
        GameObject obj = Instantiate(projectilePrefab);
        obj.SetActive(false);
        
        pooledProjectiles.Add(obj);
        
        return obj;
    }
}
