using UnityEngine;

public class FadeInAndOut : MonoBehaviour
{
    public float minZ = -2f;
    public float maxZ = 2f;
    public float speed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float zOffset = Mathf.PingPong(Time.time * speed, maxZ - minZ) + minZ;

        transform.localPosition = new Vector3(
            startPos.x,
            startPos.y,
            zOffset
        );
    }
}