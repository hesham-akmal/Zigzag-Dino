using System.Collections;
using UnityEngine;

public class ScreenGroup : MonoBehaviour
{
    [SerializeField] private float AnimTimeIn;
    [SerializeField] private float AnimTimeOut;

    private Vector3[] originalScales;

    private void Awake()
    {
        StoreOriginalScales();
    }

    private void StoreOriginalScales()
    {
        // Store original scales in array, to return to them.
        originalScales = new Vector3[transform.childCount];
        int i = 0;
        foreach (RectTransform rt in transform)
            originalScales[i++] = rt.localScale;
    }

    public void AnimateMyChildrenIn()
    {
        StartCoroutine(AnimateMyChildrenInIE());
    }

    private IEnumerator AnimateMyChildrenInIE()
    {
        foreach (RectTransform rt in transform)
            rt.localScale = Vector3.zero;

        yield return null;

        int i = 0;
        foreach (RectTransform rt in transform)
            LeanTween.scale(rt, originalScales[i++], AnimTimeIn).setEaseOutElastic();
    }

    public void AnimateMyChildrenOut()
    {
        foreach (RectTransform rt in transform)
            LeanTween.scale(rt, Vector3.zero, AnimTimeOut).setEaseLinear().setOnComplete(DisableGameObject);
    }

    private void DisableGameObject()
    {
        gameObject.SetActive(false);
    }
}