using System.Collections.Generic;
using UnityEngine;

// 엑셀의 '한 줄(Row)'에 해당하는 적의 수치 데이터 규격
[System.Serializable]
public struct EnemyDataRow
{
    [Tooltip("적의 이름이나 종류 (예: Normal, Fast, Boss)")]
    public string enemyName;

    [Tooltip("적 NPC의 최대 체력")]
    public int maxHP;

    [Tooltip("베지에 곡선 이동 속도")]
    public float moveSpeed;

    [Tooltip("처치 시 플레이어가 획득할 점수")]
    public int score;
}

// 유니티 내부에서 에셋 파일로 만들 수 있게 해주는 마법의 문장
[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Scriptable Object/Enemy Database")]
public class EnemyDatabaseSO : ScriptableObject
{
    [Header("적 NPC 엑셀 데이터 시트 대체")]
    public List<EnemyDataRow> enemySheet = new List<EnemyDataRow>();
}
