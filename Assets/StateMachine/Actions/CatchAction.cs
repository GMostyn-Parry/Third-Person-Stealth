using UnityEngine;

//An action that catches the player if they are close enough to the agent.
[CreateAssetMenu(menuName = "StateMachine/Actions/Catch")]
public class CatchAction : Action
{
    public override void Act(StateController controller)
    {
        //Catch the player if we are close enough; avoiding a square root by using square magnitude.
        if((controller.target.transform.position - controller.transform.position).sqrMagnitude <= controller.stats.distanceToCatch * controller.stats.distanceToCatch)
        {
            //Catch the target, checking if they still have a player component, and stop the agent from moving.
            controller.target.GetComponent<Player>()?.Catch();
            controller.navMeshAgent.isStopped = true;
        }
    }
}
