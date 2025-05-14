using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// �� �� ����
[System.Serializable]
public class BossPattern
{
    public string name = "Pattern";
    public List<Vector2Int> nodes = new();   // 7��7 ��ǥ
}

public class BossPatternManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] BoardManager board;      // ü����
    [SerializeField, Min(0.1f)] float warningTime = 1.5f;

    public List<BossPattern> patterns = new();

    /* -------------- API -------------- */

    /// ���� �ε����� ��� ǥ��
    public void Play(int index)
    {
        if (index < 0 || index >= patterns.Count) return;

        // 1) ��� ��� ����
        List<Node> nodeList = new();
        foreach (var cell in patterns[index].nodes)
        {
            Node n = board.GetNode(cell);     // ��ǥ �� Node
            if (n != null) nodeList.Add(n);
        }

        // 2) ����Ʈ �Ŵ��� ȣ��
        EffectManager.instance.ShowPlaceEffects(nodeList);

        // 3) ���� �ð� �� �����
        StopAllCoroutines();                  // �ߺ� ȣ�� ���
        StartCoroutine(ClearAfterDelay());
    }

    public float WarningTime => warningTime;
    public int PatternCount => patterns.Count;

    /* -------------- ���� -------------- */
    IEnumerator ClearAfterDelay()
    {
        yield return new WaitForSeconds(warningTime);
        EffectManager.instance.ClearEffects();
    }
}
