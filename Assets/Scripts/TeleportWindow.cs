using UnityEngine;

public class TeleportWindow : MonoBehaviour
{
    public int teleportID;
    public float teleportOffsetY = 0.5f;

    public void Teleport(GameObject player)
{
    // prevent teleporting while any tile is being dragged
    DraggableGround[] allTiles = FindObjectsOfType<DraggableGround>();

    foreach (DraggableGround tile in allTiles)
    {
        if (tile.IsDragging())
            return;
    }

    TeleportWindow target = GetLinkedTeleport();

    if (target == null || target == this) return;

    Vector3 newPos = target.transform.position;
    newPos.y += teleportOffsetY;

    player.transform.position = newPos;
}

    public TeleportWindow GetLinkedTeleport()
    {
        TeleportWindow[] all = FindObjectsOfType<TeleportWindow>();

        foreach (TeleportWindow t in all)
        {
            if (t != this && t.teleportID == teleportID)
            {
                return t;
            }
        }

        return null;
    }
}