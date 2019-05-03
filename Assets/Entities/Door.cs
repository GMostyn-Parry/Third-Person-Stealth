using System.Collections;
using UnityEngine;

//Class that moves the game object out of the way, when all linked toggles are in the desired state.
public class Door : Toggleable
{
    public Toggleable[] requiredActivated; //The toggles that must be activated for the door to be open.
    public Toggleable[] requiredDeactivated; //The toggles that must be deactivated for the door to be open.

    public float moveDamping = 5f; //How fast the door interpolates between positions.

    private Vector3 closedPosition; //The position of the door when it is closed.
    private Vector3 openPosition; //The position of the door when it is open.

    private Coroutine slideCoroutine; //Reference to the coroutine that moves the door.
    
    //Move the door to the open position.
    protected override void OnSelfActivated()
    {
        //End the coroutine if it is running, before restarting it with the new value.
        if(slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }

        slideCoroutine = StartCoroutine(SlideToPosition(openPosition));
    }

    //Move the door to the closed position.
    protected override void OnSelfDeactivated()
    {
        //End the coroutine if it is running, before restarting it with the new value.
        if(slideCoroutine != null)
        {
            //Restart the coroutine with the new desired value.
            StopCoroutine(slideCoroutine);
        }

        slideCoroutine = StartCoroutine(SlideToPosition(closedPosition));
    }

    private void Start()
    {
        //The starting position is the door's closed position.
        closedPosition = transform.position;
        //The door moved over by its width, with a slight lip so it can still be seen, is the open position.
        openPosition = transform.position + new Vector3(transform.localScale.x - 0.01f, 0, 0);
    }

    //Add functions to events of linked toggles to control when the door is open, and closed.
    protected override void OnEnable()
    {
        base.OnEnable();

        foreach(Toggleable toggleable in requiredActivated)
        {
            toggleable.OnActivated += OnToggleEnteredDesiredState;
            toggleable.OnDeactivated += OnToggleEnteredWrongState;
        }

        foreach(Toggleable toggleable in requiredDeactivated)
        {
            toggleable.OnActivated += OnToggleEnteredWrongState;
            toggleable.OnDeactivated += OnToggleEnteredDesiredState;
        }
    }

    //Removed functions from events of linked toggles when the door is disabled.
    protected override void OnDisable()
    {
        base.OnDisable();

        foreach(Toggleable toggleable in requiredActivated)
        {
            toggleable.OnActivated -= OnToggleEnteredDesiredState;
            toggleable.OnDeactivated -= OnToggleEnteredWrongState;
        }

        foreach(Toggleable toggleable in requiredDeactivated)
        {
            toggleable.OnActivated -= OnToggleEnteredWrongState;
            toggleable.OnDeactivated -= OnToggleEnteredDesiredState;
        }
    }

    //Move the door via interpolation to the new position.
    private IEnumerator SlideToPosition(Vector3 newPosition)
    {
        //Continue moving the door while it has not reached the position.
        while(Vector3.Distance(transform.position, newPosition) > 0.01)
        {
            yield return transform.position = Vector3.Lerp(transform.position, newPosition, moveDamping * Time.deltaTime);
        }

        //Ensure the door ends up in the correct position.
        transform.position = newPosition;
    }

    //Checks if all toggles have entered the desired state, and opens the door, if they have done so.
    private void OnToggleEnteredDesiredState()
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
        
        IsActive = true;
    }

    //Closes the door, if is is already open.
    //All linked toggles must be in the correct state for the door to open; thus if an event calls this function, then the door should be closed.
    private void OnToggleEnteredWrongState()
    {
        IsActive = false;
    }
}