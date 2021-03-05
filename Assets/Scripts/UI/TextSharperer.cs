using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TextSharpener : MonoBehaviour
{
    void rtValidate()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Rect parent_rect = ((RectTransform)rt.parent).rect;
        rt.sizeDelta = new Vector2(parent_rect.width / rt.localScale.x - parent_rect.width, parent_rect.height / rt.localScale.y - parent_rect.height);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        rtValidate();
    }
#endif
    void OnRectTransformDimensionsChange()
    {
        rtValidate();
    }
}
