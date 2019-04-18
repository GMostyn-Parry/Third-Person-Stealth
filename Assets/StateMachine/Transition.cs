using UnityEngine;

[System.Serializable]
public class Transition
{
    public Condition condition;
    public State nextState;
    public bool transitionOnTrue = true;
}