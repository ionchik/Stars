using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class TouchHandler : EventTrigger
{
    public UnityEvent TouchDown = new UnityEvent();
    public UnityEvent TouchUp = new UnityEvent();
    public UnityEvent<Modificator> Choose = new UnityEvent<Modificator>();

    public override void OnPointerDown(PointerEventData data)
    {
        TouchDown?.Invoke();
    }

    public override void OnPointerUp(PointerEventData data)
    {
        TouchUp?.Invoke();
    }

    public override void OnDrop(PointerEventData data)
    {
        RaycastResult raycastResult = data.pointerCurrentRaycast;
        if (raycastResult.gameObject.TryGetComponent<ModificationVariant>(out ModificationVariant variant))
        {
            Choose?.Invoke(variant.GetModificator);
        }
    }
}
