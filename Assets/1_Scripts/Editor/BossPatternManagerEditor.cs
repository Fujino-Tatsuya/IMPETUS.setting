#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossPatternManager))]
public class BossPatternManagerEditor : Editor
{
    const int WIDTH = 7;
    const int HEIGHT = 7;

    public override void OnInspectorGUI()
    {
        // �⺻ �ʵ� ����
        DrawDefaultInspector();
        BossPatternManager bpm = (BossPatternManager)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("=== ���� ���� ===", EditorStyles.boldLabel);

        // ���� ����Ʈ
        for (int p = 0; p < bpm.patterns.Count; ++p)
        {
            var pattern = bpm.patterns[p];
            pattern.name = EditorGUILayout.TextField("Name", pattern.name);

            // 7��7 ���
            for (int y = HEIGHT - 1; y >= 0; --y)        // ������ y=6
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < WIDTH; ++x)
                {
                    Vector2Int cell = new(x, y);
                    bool on = pattern.nodes.Contains(cell);
                    bool next = GUILayout.Toggle(on, "", "Button", GUILayout.Width(22), GUILayout.Height(22));

                    if (next != on)
                    {
                        if (next) pattern.nodes.Add(cell);
                        else pattern.nodes.RemoveAll(v => v == cell);
                        EditorUtility.SetDirty(bpm);      // Undo ����
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space(6);
        }

        // �� ���� �߰� ��ư
        if (GUILayout.Button("Add New Pattern"))
            bpm.patterns.Add(new BossPattern());
    }
}
#endif
