using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    [SerializeField] private GameObject glowEffectPrefab;

    private List<GameObject> activeEffects = new();

    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    public void ShowMoveEffects(List<Node> nodes)
    {
        ClearEffects();  // ±âÁ¸ ÀÌÆåÆ® Á¦°Å

        foreach (var node in nodes)
        {
            GameObject effect = Instantiate(glowEffectPrefab);
            effect.transform.position = node.transform.position + new Vector3(0, 0.1f, 0);  // »ìÂ¦ ¶ç¿ò
            activeEffects.Add(effect);
        }
    }

    public void ShowPlaceEffects(List<Node> nodes)
    {
        ClearEffects();  // ±âÁ¸ ÀÌÆåÆ® Á¦°Å

        foreach (var node in nodes)
        {
            GameObject effect = Instantiate(glowEffectPrefab);
            effect.transform.position = node.transform.position + new Vector3(0, 0.1f, 0);  // »ìÂ¦ ¶ç¿ò
            activeEffects.Add(effect);
        }
    }

    public void ClearEffects()
    {
        foreach (var effect in activeEffects)
        {
            Destroy(effect);
        }
        activeEffects.Clear();
    }
}
