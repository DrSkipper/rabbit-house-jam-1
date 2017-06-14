using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.iOS;

public class FoodManager : MonoBehaviour
{
    public List<GameObject> FoodPrefabs;
    public float FoodHeight;
    public Vector3 AROffset;
    public Vector3 FoodScale;

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
                Debug.Log("Touch detected");
                var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                ARPoint point = new ARPoint
                {
                    x = screenPosition.x,
                    y = screenPosition.y
                };

                // prioritize reults types
                ARHitTestResultType[] resultTypes = {
                ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
                // if you want to use infinite planes use this:
                //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                ARHitTestResultType.ARHitTestResultTypeHorizontalPlane,
                //ARHitTestResultType.ARHitTestResultTypeFeaturePoint
                };

                foreach (ARHitTestResultType resultType in resultTypes)
                {
                    List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultType);
                    if (hitResults.Count > 0)
                    {
                        var hitResult = hitResults[0];
                        if (attemptCreateFood(UnityARMatrixOps.GetPosition(hitResult.worldTransform) + this.AROffset))
                            return;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
            float distance = 0;
            if (_foodPlane.Raycast(ray, out distance))
            {
                attemptCreateFood(ray.GetPoint(distance));
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

    private bool attemptCreateFood(Vector3 position)
    {
        // Make sure we clicked in a reachable position
        //UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        //bool check = UnityEngine.AI.NavMesh.CalculatePath(this.transform.position, position, int.MaxValue, path);
        //{
        //Debug.Log("Creating Food, check: " + check);
        GameObject food = Instantiate<GameObject>(this.FoodPrefabs[Random.Range(0, this.FoodPrefabs.Count - 1)]);
        food.AddComponent<Food>();
        food.transform.position = position;
        food.transform.localScale = this.FoodScale;
        return true;
        /*}
        return false;*/
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
