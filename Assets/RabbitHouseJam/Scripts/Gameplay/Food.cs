using UnityEngine;

public class Food : MonoBehaviour
{
    void Start()
    {
        GlobalEvents.Notifier.SendEvent(new FoodSpawnEvent(this.transform));
    }
}
