using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Class that controls the state machine for an agent that chases the player; attempting to catch them.
[RequireComponent(typeof(NavMeshAgent))]
public class StateController : MonoBehaviour
{
    public float TimeInState { get; private set; } //How long we have been in the current state.

    public Dictionary<string, object> blackboard = new Dictionary<string, object>(); //For capturing information for later use by the agent; i.e. last known position of target, etc.

    public GameObject target; //What we are chasing.
    public GuardStats stats; //Information on the agent.
    public PatrolPoint currentPoint; //The waypoint the agent is currently moving to.

    [HideInInspector] public NavMeshAgent navMeshAgent; //NavMeshAgent of the agent the state machine controls.

    [SerializeField] private State currentState; //The state the controller is currently in; also the initial state of the controller.

    //Change the agent to the state passed.
    public void ChangeState(State nextState)
    {
        currentState = nextState;
        TimeInState = 0;
    }

    //Find a patrol point connected to the current patrol point, use it as the new current patrol point, and return its location.
    public Vector3 NextPatrolPosition()
    {
        //Select a connected point to patrol to.
        PatrolPoint newPoint = currentPoint.ChooseConnectedPoint((PatrolPoint)blackboard["PreviousPatrolPoint"]);

        //Update references.
        blackboard["PreviousPatrolPoint"] = currentPoint;
        currentPoint = newPoint;

        //Return position to patrol to.
        return currentPoint.transform.position;
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");

        //Setup a few required items in the blackboard.
        blackboard["TargetController"] = target.GetComponent<CharacterController>();
        blackboard["PreviousPatrolPoint"] = currentPoint;
        blackboard["VelocityBeforePause"] = Vector3.zero;

        //Print an error, if there is no starting waypoint for patrolling.
        Debug.Assert(currentPoint, "No initial patrol point set for: " + name);
    }

    private void Update()
    {
        //Don't update the agent, while the game is paused.
        if(GameManager.IsPaused) return;

        TimeInState += Time.deltaTime;
        currentState.UpdateState(this);
    }

    private void OnEnable()
    {
        GameManager.OnPause += OnPause;
        GameManager.OnUnpause += OnUnpause;

        /* Dictionaries are not serialisable, and as such will lose all their values when the code is hotswapped on change.
         * Thus to allow hotswapping, we load the default values back into the dictionary
         * when it is not the first time it is being enabled (it has a target) while using the Unity Editor.
         * A more maintainable technique would be to create a serialisable blackboard, but this works for this small project.
         */
        if(Application.isEditor && target)
        {
            blackboard["TargetController"] = target.GetComponent<CharacterController>();
            blackboard["PreviousPatrolPoint"] = currentPoint;
            blackboard["VelocityBeforePause"] = Vector3.zero;

            blackboard["LastTargetPosition"] = transform.position;
            blackboard["LastTargetVelocity"] = Vector3.zero;
        }
    }

    private void OnDisable()
    {
        GameManager.OnPause -= OnPause;
        GameManager.OnUnpause -= OnUnpause;
    }

    private void OnPause()
    {
        //Store the velocity, and set the current velocity to zero, when pausing the agent.
        blackboard["VelocityBeforePause"] = navMeshAgent.velocity;

        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.isStopped = true;
    }

    private void OnUnpause()
    {
        //Restore the agent's velocity, when unpausing the agent.
        navMeshAgent.velocity = blackboard.ContainsKey("VelocityBeforePause") ? (Vector3)blackboard["VelocityBeforePause"] : Vector3.zero;
        navMeshAgent.isStopped = false;
    }
}