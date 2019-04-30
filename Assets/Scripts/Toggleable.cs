using UnityEngine;

//For objects that have a toggleable state, i.e. buttons, locks, lights, etc.
//Not an interface, so it can be used to type objects for the inspector.
public abstract class Toggleable : MonoBehaviour
{
    public delegate void Activated();
    public event Activated OnActivated;

    public delegate void Deactivated();
    public event Deactivated OnDeactivated;

    public bool IsActive { get; protected set; } = false;

    public virtual void Toggle()
    {
        //Deactivate the toggle, if it was active.
        if(IsActive)
        {
            Deactivate();
        }
        else
        {
            Activate();
        }
    }

    public virtual void Activate()
    {
        IsActive = true;
        OnActivated?.Invoke();
    }

    public virtual void Deactivate()
    {
        IsActive = false;
        OnDeactivated?.Invoke();
    }
}