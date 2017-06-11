using UnityEngine;
using UnityEngine.AI;

public class RabbitController : MonoBehaviour
{
    public float Speed = 10.0f;
    public float RotationSpeed = 10.0f;
    public float ConsumeDistance = 2.0f;
    public MoveTo MoveTo;
    public NavMeshAgent Agent;
    private Transform _target;

    void Update()
    {
        // Move toward the closest food
        Transform target = FoodManager.GetClosestFood(this.transform);
        if (target != null)
        {
            Vector2 ourPos = realPosToSimulationPos(this.transform.position);
            Vector2 targetPos = realPosToSimulationPos(target == _target && this.Agent.hasPath ? this.Agent.pathEndPosition : target.position);

            // Eat the food if we're close enough
            if (Vector2.Distance(targetPos, ourPos) < this.ConsumeDistance)
            {
                target.GetComponent<Food>().Consume();
            }
            else
            {
                _target = target;
                this.MoveTo.MoveToward(target);
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
