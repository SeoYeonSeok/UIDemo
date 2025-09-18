using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class LogoTween : MonoBehaviour
{
    private Image logoImg;
    private RectTransform logoImgTrns;
    private float bounceDist = 15f;
    private float bounceDur = 0.25f;

    private Sequence sq;

    Vector2 originSize;
    Vector2 originPos;

    void Start()
    {
        logoImg = GetComponent<Image>();
        logoImgTrns = GetComponent<RectTransform>();
        originSize = logoImgTrns.sizeDelta;
        originPos = logoImgTrns.anchoredPosition;

        StartTween();
    }

    public void StartTween()
    {
        RectTransform rect = logoImg.rectTransform;
        Vector2 originPos = rect.anchoredPosition;

        sq = DOTween.Sequence(); // �������� Ʈ�� ������ ������ ���� ������ ���
        sq.AppendInterval(2f); // 2�� ���� �ð����� ����
        sq.Append(rect.DOAnchorPosY(originPos.y - (bounceDist / 2), bounceDur / 2).SetEase(Ease.InQuad));
        sq.Append(rect.DOAnchorPosY(originPos.y + bounceDist, bounceDur).SetEase(Ease.InQuad));
        Vector2 newSize = new Vector2(originSize.x * 1.1f, originSize.y);
        sq.Join(rect.DOSizeDelta(newSize, 0.25f));
        sq.Append(rect.DOAnchorPosY(originPos.y, bounceDur).SetEase(Ease.OutBounce));
        sq.Join(rect.DOSizeDelta(originSize, 0.25f)); // Position�� Scale�� ������ ������ ���� Ƣ�� ���� ȿ�� ����
        sq.SetLoops(-1, LoopType.Restart); // ���� �ݺ�
    }

    public void EndTween()
    {
        if (sq != null && sq.IsActive())
        {
            sq.Kill();   // Ʈ�� ����
            sq = null;   // ���� ����
        }

        logoImgTrns.anchoredPosition = originPos;
        logoImgTrns.sizeDelta = originSize;
    }

    public void RestartTween()
    {
        logoImgTrns.sizeDelta = Vector2.zero;
        logoImgTrns.DOSizeDelta(originSize, 0.75f).SetEase(Ease.OutElastic)
            .OnComplete(() => StartTween());
    }
}
