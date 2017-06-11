using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Tracker;
    public Vector2 Offset;

    void Update()
    {
        if (this.Tracker != null)
        {
            this.transform.SetPosition(this.Tracker.transform.position.x + this.Offset.x, this.transform.position.y, this.Tracker.transform.position.z + this.Offset.y);
        }
    }
}
