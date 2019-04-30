using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateController : MonoBehaviour
{
    public GameObject target; //What we are chasing.
    public GuardStats stats; //Information on the agent.
    public PatrolPoint currentPoint; //The waypoint the agent is currently moving to.

    [HideInInspector] public NavMeshAgent navMeshAgent; //NavMeshAgent of the agent the state machine controls.
    [HideInInspector] public Vector3 desiredPosition; //A position we want the AI to move to.

    public bool ShouldMoveToPosition { get; private set; } //Whether the agent should move to the desired position.
    public float TimeInState { get; private set; } //How long we have been in the current state.

    [SerializeField] private State currentState; //The state the controller is currently in; also the initial state of the controller.

    private PatrolPoint previousPoint; //The patrol point the agent was last at.

    private bool wasPaused; //Whether the agent has been paused.
    private Vector3 velocityBeforePause; //Agent's velocity before the game was paused, used for restoring state.

    public void ChangeState(State nextState)
    {
        currentState = nextState;
        TimeInState = 0;
    }

    public Vector3 NextPatrolPosition()
    {
        //Select a connected point to patrol to.
        PatrolPoint newPoint = currentPoint.ChooseConnectedPoint(previousPoint);

        //Update references.
        previousPoint = currentPoint;
        currentPoint = newPoint;

        //Return position to patrol to.
        return currentPoint.transform.position;
    }

    public void EnableMoveToPosition(Vector3 position = new Vector3())
    {
        if(position != Vector3.zero)
        {
            desiredPosition = position;
        }

        ShouldMoveToPosition = true;
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");

        //Print an error, if there is no starting waypoint for patrolling.
        Debug.Assert(currentPoint, "No initial patrol point set for: " + name);
    }

    private void Update()
    {
        //NavMeshAgent should not move while the game is paused.
        navMeshAgent.isStopped = GameManager.IsPaused;

        if(GameManager.IsPaused)
        {
            //Store the velocity, and set the current velocity to zero, when pausing the agent.
            if(!wasPaused)
            {
                velocityBeforePause = navMeshAgent.velocity;
                navMeshAgent.velocity = Vector3.zero;

                wasPaused = true;
            }
        }
        else
        {
            //Restore the agent's velocity, when unpausing the agent.
            if(wasPaused)
            {
                navMeshAgent.velocity = velocityBeforePause;

                wasPaused = false;
            }

            TimeInState += Time.deltaTime;
            currentState.UpdateState(this);
            
        }
    }
}