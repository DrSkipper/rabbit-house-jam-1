using UnityEngine;
using System.Collections.Generic;

public class FoodManager : MonoBehaviour
{
    public List<GameObject> FoodPrefabs;
    public float FoodHeight;

    [HideInInspector]
    public List<Transform> Food;

    private static FoodManager _instance;
    private Plane _foodPlane;

    void Awake()
    {
        _instance = this;
        this.Food = new List<Transform>();
        _foodPlane = new Plane(Vector3.up, new Vector3(0, this.FoodHeight, 0));
        GlobalEvents.Notifier.Listen(FoodSpawnEvent.NAME, this, onFoodSpawn);
        GlobalEvents.Notifier.Listen(FoodDestroyedEvent.NAME, this, onFoodDestroyed);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                //TODO: AR stuff
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
            float distance = 0;
            if (_foodPlane.Raycast(ray, out distance))
            {
                Vector3 position = ray.GetPoint(distance);

                // Make sure we clicked in a reachable position
                UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
                if (UnityEngine.AI.NavMesh.CalculatePath(this.transform.position, position, int.MaxValue, path))
                {
                    GameObject food = Instantiate<GameObject>(this.FoodPrefabs[Random.Range(0, this.FoodPrefabs.Count - 1)]);
                    food.AddComponent<Food>();
                    food.transform.position = position;
                }
            }
        }
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
