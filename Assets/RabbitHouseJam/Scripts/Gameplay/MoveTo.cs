using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public Transform goal;
    public bool MoveOnStart = false;
    public NavMeshAgent Agent;

	void Start () {

        if (this.MoveOnStart)
            this.MoveToward(goal);
	}

    public void MoveToward(Transform target)
    {
        goal = target;
        this.Agent.destination = goal.position;
    }
}
