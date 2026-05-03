using UnityEngine;

public class DragAxisConstraint : MonoBehaviour
{
    public enum AxisMode
    {
        Free,
        XOnly,
        YOnly
    }

    public AxisMode axisMode = AxisMode.Free;

    private Vector3 startDragPosition;
    private bool hasStartedDrag = false;

    public void OnStartDrag(Vector3 startPos)
    {
        startDragPosition = startPos;
        hasStartedDrag = true;
    }

    public Vector3 ApplyConstraint(Vector3 targetPosition)
    {
        if (!hasStartedDrag) return targetPosition;

        switch (axisMode)
        {
            case AxisMode.XOnly:
                targetPosition.y = startDragPosition.y;
                break;

            case AxisMode.YOnly:
                targetPosition.x = startDragPosition.x;
                break;
        }

        return targetPosition;
    }

    public void OnEndDrag()
    {
        hasStartedDrag = false;
    }
}