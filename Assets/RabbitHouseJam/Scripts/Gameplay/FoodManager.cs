using UnityEngine;
using System.Collections.Generic;

public class FoodManager : MonoBehaviour
{
    public List<Transform> Food;
    private static FoodManager _instance;

    void Awake()
    {
        _instance = this;
        this.Food = new List<Transform>();
        GlobalEvents.Notifier.Listen(FoodSpawnEvent.NAME, this, onFoodSpawn);
        GlobalEvents.Notifier.Listen(FoodDestroyedEvent.NAME, this, onFoodDestroyed);
    }

    public static Transform GetClosestFood(Transform source)
    {
        float dist = float.MaxValue;
        Transform retVal = null;

        for (int i = 0; i < _instance.Food.Count; ++i)
        {
            float d = Vector3.Distance(_instance.Food[i].position, source.position);
            if (d < dist)
            {
                dist = d;
                retVal = _instance.Food[i];
            }
        }
        return retVal;
    }

    private void onFoodSpawn(LocalEventNotifier.Event e)
    {
        this.Food.Add((e as FoodSpawnEvent).Food);
    }

    private void onFoodDestroyed(LocalEventNotifier.Event e)
    {
        this.Food.Remove((e as FoodDestroyedEvent).Food);
    }
}

public class FoodSpawnEvent : LocalEventNotifier.Event
{
    public const string NAME = "FOOD_SPAWN";
    public Transform Food;

    public FoodSpawnEvent(Transform food)
    {
        this.Name = NAME;
        this.Food = food;
    }
}

public class FoodDestroyedEvent : LocalEventNotifier.Event
{
    public const string NAME = "FOOD_DESTROY";
    public Transform Food;

    public FoodDestroyedEvent(Transform food)
    {
        this.Name = NAME;
        this.Food = food;
    }
}
