using UnityEngine;

public abstract class Condition : ScriptableObject
{
    public abstract bool Evaluate(StateController controller);
}
