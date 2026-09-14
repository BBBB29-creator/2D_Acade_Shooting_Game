using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    private void Start()
    {
        // 🎯 이 오브젝트가 세상에 태어나자마자(소환되자마자) 0.4초 뒤에 스스로를 파괴(삭제)합니다.
        Destroy(gameObject, 0.4f);
    }
}
