using UnityEngine;

public class RabbitController : MonoBehaviour
{
    public float Speed = 10.0f;
    public float RotationSpeed = 10.0f;
    public float ConsumeDistance = 2.0f;

    void Update()
    {
        // Move toward the closest food
        Transform target = FoodManager.GetClosestFood(this.transform);
        if (target != null)
        {
            Vector2 ourPos = realPosToSimulationPos(this.transform.position);
            Vector2 targetPos = realPosToSimulationPos(target.position);

            // Eat the food if we're close enough
            if (Vector2.Distance(targetPos, ourPos) < this.ConsumeDistance)
            {
                GlobalEvents.Notifier.SendEvent(new FoodDestroyedEvent(target));
                Destroy(target.gameObject);
            }
            else
            {

                this.transform.position = applySimulationPosToRealPos(Vector2.MoveTowards(ourPos, targetPos, this.Speed * Time.deltaTime), this.transform.position);

                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetPos.x, this.transform.position.y, targetPos.y) - this.transform.position);
                float str = Mathf.Min(this.RotationSpeed * Time.deltaTime, 1);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, str);
            }
        }
    }

    private Vector2 realPosToSimulationPos(Vector3 realPos)
    {
        return new Vector2(realPos.x, realPos.z);
    }

    private Vector3 applySimulationPosToRealPos(Vector2 simulationPos, Vector3 prevRealPos)
    {
        return new Vector3(simulationPos.x, prevRealPos.y, simulationPos.y);
    }
}
