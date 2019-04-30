using UnityEngine;

public class Door : Toggleable
{
    public Toggleable[] requiredActivated; //The toggles that must be activated for the door to be open.
    public Toggleable[] requiredDeactivated; //The toggles that must be deactivated for the door to be open.

    public float moveDamping = 5f; //How fast the door interpolates between positions.

    private Vector3 closedPosition; //The position of the door when it is closed.
    private Vector3 openPosition; //The position of the door when it is open.

    private void Start()
    {
        //The starting position is the door's closed position.
        closedPosition = transform.position;
        //The door moved over by its width, with a slight lip so it can still be seen, is the open position.
        openPosition = transform.position + new Vector3(transform.localScale.x - 0.01f, 0, 0);
    }

    private void Update()
    {
        //Move the door via interpolation between the closed position and open position; only has an effect when the toggle is changed.
        transform.position = Vector3.Lerp(transform.position, IsActive ? openPosition : closedPosition, moveDamping * Time.deltaTime);
    }

    //Add functions to events of linked toggles to control when the door is open, and closed.
    private void OnEnable()
    {
        foreach(Toggleable toggleable in requiredActivated)
        {
            toggleable.OnActivated += CheckAndOpen;
            toggleable.OnDeactivated += CheckAndClose;
        }

        foreach(Toggleable toggleable in requiredDeactivated)
        {
            toggleable.OnActivated += CheckAndClose;
            toggleable.OnDeactivated += CheckAndOpen;
        }
    }

    //Removed functions from events of linked toggles when the door is disabled.
    private void OnDisable()
    {
        foreach(Toggleable toggleable in requiredActivated)
        {
            toggleable.OnActivated -= CheckAndOpen;
            toggleable.OnDeactivated -= CheckAndClose;
        }

        foreach(Toggleable toggleable in requiredDeactivated)
        {
            toggleable.OnActivated -= CheckAndClose;
            toggleable.OnDeactivated -= CheckAndOpen;
        }
    }

    //Opens the door, if the current state determines it should be open.
    private void CheckAndOpen()
    {
        //Doesn't open the door if any of the toggles that must be active aren't active.
        foreach(Toggleable toggleable in requiredActivated)
        {
            if(!toggleable.IsActive) return;
        }

        //Doesn't open the door if any of the toggles that shouldn't be active are active.
        foreach(Toggleable toggleable in requiredDeactivated)
        {
            if(toggleable.IsActive) return;
        }

        Activate();
    }

    //Closes the door, if is is already open.
    //All linked toggles must be in the correct state for the door to open; thus if an event calls this function, then the door should be closed.
    private void CheckAndClose()
    {
        //We check if it is active first to stop the OnDeactivated event being called unnecessarily for this door.
        if(IsActive)
        {
            Deactivate();
        }
    }
}
