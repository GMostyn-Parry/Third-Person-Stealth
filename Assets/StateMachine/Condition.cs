using UnityEngine;

//Abstract class for a condition for the state machine to evaluate if it should transition between states.
public abstract class Condition : ScriptableObject
{
    public abstract bool Evaluate(StateController controller);
}
