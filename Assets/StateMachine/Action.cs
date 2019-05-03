using UnityEngine;

//Abstract class for an action that the state machine can cause the agent to take.
public abstract class Action : ScriptableObject
{
    public abstract void Act(StateController controller);
}
