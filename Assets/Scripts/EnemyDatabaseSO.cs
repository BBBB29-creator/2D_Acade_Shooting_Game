using System.Collections.Generic;
using UnityEngine;

// 엑셀의 '한 줄(Row)'에 해당하는 적의 수치 데이터 규격 (과제 핵심 구조)
[System.Serializable]
public struct EnemyDataRow
{
    [Tooltip("적의 이름이나 종류 (예: Normal, Medium, Heavy)")]
    public string enemyName;

    [Tooltip("적 NPC의 최대 체력")]
    public int maxHP;

    [Tooltip("이동 속도")]
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

    // 빠른 검색을 위해 내부적으로 저장할 딕셔너리 변수 (자동 처리)
    private Dictionary<string, EnemyDataRow> dataCache;

    /// <summary>
    /// 데이터를 빠르게 찾을 수 있도록 준비 구역(캐시)을 만듭니다.
    /// </summary>
    public void InitializeCache()
    {
        dataCache = new Dictionary<string, EnemyDataRow>();

        foreach (var row in enemySheet)
        {
            // 중복된 이름이 들어오는 것을 방지하며 안전하게 등록
            if (!string.IsNullOrEmpty(row.enemyName) && !dataCache.ContainsKey(row.enemyName))
            {
                dataCache.Add(row.enemyName, row);
            }
        }
    }

    /// <summary>
    /// EnemyCharacter가 요청한 이름(Normal 등)을 바탕으로 체력과 점수 행을 찾아 반환합니다.
    /// </summary>
    public EnemyDataRow GetEnemyData(string name)
    {
        // 만약 준비 구역이 비어있다면 자동으로 먼저 생성합니다.
        if (dataCache == null)
        {
            InitializeCache();
        }

        // 이름으로 매칭되는 데이터 행을 찾아 반환합니다.
        if (dataCache.TryGetValue(name, out EnemyDataRow row))
        {
            return row;
        }

        // [안전장치] 만약 데이터베이스 시트에 없는 이름을 적었다면 기본값 반환
        Debug.LogError($"[EnemyDatabaseSO] 데이터베이스 시트에서 '{name}'에 해당하는 적을 찾을 수 없습니다!");
        return default;
    }
}
