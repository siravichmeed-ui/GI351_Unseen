using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverSFX : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip hoverSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX(hoverSound);
    }
}