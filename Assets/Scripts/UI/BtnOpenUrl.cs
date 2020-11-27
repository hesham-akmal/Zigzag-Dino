using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BtnOpenUrl : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private string URL;

    public void OnPointerDown(PointerEventData eventData)
    {
        Application.OpenURL(URL);
    }
}