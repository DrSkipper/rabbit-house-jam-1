using UnityEngine;
using UnityEngine.AI;

public class RabbitController : MonoBehaviour
{
    public bool UseNavMesh = false;
    public float Speed = 10.0f;
    public float RotationSpeed = 10.0f;
    public float ConsumeDistance = 2.0f;
    public MoveTo MoveTo;
    public NavMeshAgent Agent;
    public Animation AnimBehavior;
    public AnimationClip RunClip;
    public AnimationClip IdleClip;

    private Transform _target;
    private delegate void MovementMethodDelegate(Transform target);
    private MovementMethodDelegate _moveUpdate;

    void Start()
    {
        if (this.UseNavMesh)
            _moveUpdate = moveWithNavMeshUpdate;
        else
            _moveUpdate = moveSimpleUpdate;
    }

    void Update()
    {
        // Move toward the closest food
        Transform target = FoodManager.GetClosestFood(this.transform);
        if (target != null)
        {
            if (this.AnimBehavior.clip != this.RunClip)
            {
                this.AnimBehavior.Stop();
                this.AnimBehavior.clip = this.RunClip;
                this.AnimBehavior.Play();
            }

            _moveUpdate(target);
        }
        else if (this.AnimBehavior.clip != this.IdleClip)
        {
            this.AnimBehavior.Stop();
            this.AnimBehavior.clip = this.IdleClip;
            this.AnimBehavior.Play();
        }
    }

    private void moveWithNavMeshUpdate(Transform target)
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

    private void moveSimpleUpdate(Transform target)
    {
        Vector2 ourPos = realPosToSimulationPos(this.transform.position);
        Vector2 targetPos = realPosToSimulationPos(target.position);
        this.transform.position = applySimulationPosToRealPos(Vector2.MoveTowards(ourPos, targetPos, this.Speed * Time.deltaTime), this.transform.position);

        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetPos.x, this.transform.position.y, targetPos.y) - this.transform.position);
        float str = Mathf.Min(this.RotationSpeed * Time.deltaTime, 1);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, str);

        if (Vector2.Distance(targetPos, ourPos) < this.ConsumeDistance)
        {
            target.GetComponent<Food>().Consume();
        }
        else
        {
            _target = target;
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
