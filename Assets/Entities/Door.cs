using System.Collections;
using UnityEngine;

//Class that moves the game object out of the way, when its active state is true.
public class Door : Toggle
{
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

    //Move the door via interpolation to the new position.
    private IEnumerator SlideToPosition(Vector3 newPosition)
    {
        //Continue moving the door while it has not reached the position.
        while(Vector3.Distance(transform.position, newPosition) > 0.001)
        {
            yield return transform.position = Vector3.Lerp(transform.position, newPosition, moveDamping * Time.deltaTime);
        }

        //Set the door to the desired position, as we are close enough for the player to not notice.
        transform.position = newPosition;
    }
}