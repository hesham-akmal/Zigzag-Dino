using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIScaleAnim : MonoBehaviour
{
    [SerializeField] private float BounceTime;

    [SerializeField] private float BounceScalePercentage;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void StartAnim()
    {
        transform.localScale = originalScale;
        LeanTween.scale(GetComponent<RectTransform>(), originalScale * BounceScalePercentage, BounceTime).setLoopPingPong().setEaseInSine();
    }

    private void OnEnable()
    {
        StartAnim();
    }
}