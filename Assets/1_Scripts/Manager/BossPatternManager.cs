using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 한 개 패턴
[System.Serializable]
public class BossPattern
{
    public string name = "Pattern";
    public List<Vector2Int> nodes = new();   // 7×7 좌표
}

public class BossPatternManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] BoardManager board;      // 체스판
    [SerializeField, Min(0.1f)] float warningTime = 1.5f;

    public List<BossPattern> patterns = new();

    /* -------------- API -------------- */

    /// 패턴 인덱스로 경고 표시
    public void Play(int index)
    {
        if (index < 0 || index >= patterns.Count) return;

        // 1) 노드 목록 생성
        List<Node> nodeList = new();
        foreach (var cell in patterns[index].nodes)
        {
            Node n = board.GetNode(cell);     // 좌표 → Node
            if (n != null) nodeList.Add(n);
        }

        // 2) 이펙트 매니저 호출
        EffectManager.instance.ShowPlaceEffects(nodeList);

        // 3) 지정 시간 뒤 지우기
        StopAllCoroutines();                  // 중복 호출 대비
        StartCoroutine(ClearAfterDelay());
    }

    public float WarningTime => warningTime;
    public int PatternCount => patterns.Count;

    /* -------------- 내부 -------------- */
    IEnumerator ClearAfterDelay()
    {
        yield return new WaitForSeconds(warningTime);
        EffectManager.instance.ClearEffects();
    }
}
