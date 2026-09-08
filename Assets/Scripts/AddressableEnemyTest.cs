using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableEnemyTest : MonoBehaviour
{
    private GameObject spawnedEnemy;

    private void Start()
    {
        LoadEnemy();
    }

    public void LoadEnemy()
    {
        Addressables.InstantiateAsync("enemy010anim").Completed += OnEnemyLoaded;
    }

    private void OnEnemyLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            spawnedEnemy = handle.Result;
            spawnedEnemy.transform.position = Vector3.zero; // 출현 위치

            Debug.Log("Addressable Enemy Loaded!");
        }
        else
        {
            Debug.LogError("Addressable Enemy Load Failed!");
        }
    }

    private void OnDestroy()
    {
        if (spawnedEnemy != null)
        {
            Addressables.ReleaseInstance(spawnedEnemy);
        }
    }
}