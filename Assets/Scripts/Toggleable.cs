using UnityEngine;

//Abstract class for objects that have a toggleable state, i.e. buttons, locks, lights, etc.
//Not an interface, so it can be used to type objects for the inspector.
public abstract class Toggleable : MonoBehaviour
{
    public delegate void Activated();
    public event Activated OnActivated; //Called when the toggle changes to the active state.

    public delegate void Deactivated();
    public event Deactivated OnDeactivated; //Called when the toggle changes to the inactive state.

    private bool _isActive;
    public bool IsActive //The state of the toggle; emits an event when activated, or deactivated.
    {
        get
        {
            return _isActive;
        }

        set
        {
            //Don't emit the events if we were already in this state.
            if(_isActive == value) return;

            _isActive = value;

            //Emit the event depending on the new state.
            if(value)
            {
                OnActivated?.Invoke();
            }
            else
            {
                OnDeactivated?.Invoke();
            }
        }
    }

    //Called when the object is activated.
    protected abstract void OnSelfActivated();
    //Called when the object is deactivated.
    protected abstract void OnSelfDeactivated();

    protected virtual void OnEnable()
    {
        OnActivated += OnSelfActivated;
        OnDeactivated += OnSelfDeactivated;
    }

    protected virtual void OnDisable()
    {
        OnActivated -= OnSelfActivated;
        OnDeactivated -= OnSelfDeactivated;
    }
}