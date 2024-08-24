using UnityEngine;

public class UICenterOnClick_Ellipse : MonoBehaviour
{
    void OnClick()
    {
        UICenterOnChild_Ellipse center = NGUITools.FindInParents<UICenterOnChild_Ellipse>(gameObject);
        //UIPanel panel = NGUITools.FindInParents<UIPanel>(gameObject);

        if (center != null)
        {
            if (center.enabled)
                center.CenterOn(transform, UIScrollView_Ellipse.EM_RecenterEvent.OnCustomClick);
        }
        //else if (panel != null && panel.clipping != UIDrawCall.Clipping.None)
        //{
        //    UIScrollView sv = panel.GetComponent<UIScrollView>();
        //    Vector3 offset = -panel.cachedTransform.InverseTransformPoint(transform.position);
        //    if (!sv.canMoveHorizontally) offset.x = panel.cachedTransform.localPosition.x;
        //    if (!sv.canMoveVertically) offset.y = panel.cachedTransform.localPosition.y;
        //    SpringPanel.Begin(panel.cachedGameObject, offset, 6f);
        //}
    }
}