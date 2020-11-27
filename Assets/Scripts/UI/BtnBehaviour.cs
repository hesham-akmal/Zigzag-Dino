using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BtnBehaviour : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private bool SfxOnClick = true;

    [SerializeField] private bool BounceOnClick = true;

    [SerializeField] private float BounceTime = 0.15f;

    [SerializeField] private float BounceScalePercentage = 0.7f;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (SfxOnClick)
            AudioManager.Instance.PlayBtnClickSfx();

        if (BounceOnClick)
            Bounce();
    }

    public void Bounce()
    {
        if (!LeanTween.isTweening(rectTransform))
            LeanTween.scale(rectTransform
                , new Vector3(transform.localScale.x * BounceScalePercentage, transform.localScale.y * BounceScalePercentage, 1)
                , BounceTime).setLoopPingPong(1).setEaseInSine();
    }
}