using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AFSlider : Slider
{
    [Header("Segments")]
    [Min(1)]
    public int segments = 1;

    public override float value
    {
        get => base.value;
        set => base.value = SnapToSegment(value);
    }

    public override void SetValueWithoutNotify(float value)
    {
        base.value = SnapToSegment(value);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        base.value = SnapToSegment(value);
    }

    public override void OnMove(AxisEventData eventData)
    {
        base.OnMove(eventData);
        base.value = SnapToSegment(value);
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
        base.value = SnapToSegment(value);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        base.value = SnapToSegment(value);
    }

    private float SnapToSegment(float rawValue)
    {
        return Mathf.RoundToInt(rawValue * segments) / (float)segments;
    }
}
