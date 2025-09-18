using DG.Tweening;
using UnityEngine;

public class UITweens : MonoBehaviour
{
    public float goalX;
    public float goalY;

    private float originX;
    private float originY;

    private void Start()
    {
        originX = GetComponent<RectTransform>().anchoredPosition.x;
        originY = GetComponent<RectTransform>().anchoredPosition.y;
    }

    public void ActivateTween()
    {
        Move2GoalPos();
    }

    public void ActivateReverseTween()
    {
        Move2OriginPos();
    }

    private void Move2GoalPos()
    {
        Vector2 goalPos = new Vector2(goalX, goalY);

        GetComponent<RectTransform>().DOAnchorPos(goalPos, 1.5f).SetEase(Ease.OutElastic);
    }

    private void Move2OriginPos()
    {
        Vector2 goalPos = new Vector2(originX, originY);

        GetComponent<RectTransform>().DOAnchorPos(goalPos, 0.5f)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
