//A class for storing data on when, and how, a transition should occur.
[System.Serializable]
public class Transition
{
    public bool transitionOnTrue = true; //Whether the transition occurs on the condition being true.
    public Condition condition; //The condition that causes the transition.
    public State nextState; //The state after the transition.
    public Action[] actionsOnTransition; //The actions to perform before performing the transition.
}