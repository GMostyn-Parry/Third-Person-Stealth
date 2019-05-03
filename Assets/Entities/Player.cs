using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class for the object moved by the player; affected by gravity, and can be caught.
 * 
 * Movement expects the floor to always be Vector3.down.
 */
 [RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public delegate void Caught();
    public event Caught OnCaught; //Event emitted when the player is caught.

    public delegate void EnterInteract(string toDisplay);
    public event EnterInteract OnEnterInteract; //Event emitted when the player enters trigger of an interactable object, when it could not interact before.

    public delegate void LeftInteract();
    public event LeftInteract OnLeftInteract; //Event emitted when the player leaves trigger of the last interactable object.

    public float moveSpeed = 10.0f; //How fast the player moves.
    public float gravity = 5.0f; //Magnitude of gravity acting on player.

    public bool IsCaught { get; private set; } //Whether the player has been caught.

    private CharacterController controller; //Movement controller for the player.
    private Transform cameraTransform; //Transform of the camera that was being used when the level started.

    private List<IInteractable> nearInteractables = new List<IInteractable>(); //List of interactables the player is in-range to use.

    //Called when the player is caught; prevents the player moving anymore, and signals the OnCaught event.
    public void Catch()
    {
        IsCaught = true;
        
        OnCaught();
    }

    //Adds interactable to the list of objects the player can interact with.
    public void AddInteractable(IInteractable interactable)
    {
        if(nearInteractables.Count == 0) OnEnterInteract("Press F to Interact");

        nearInteractables.Add(interactable);
    }

    //Removes interactable from the list of objects the player can interact with.
    public void RemoveInteractable(IInteractable interactable)
    {
        if(nearInteractables.Count == 1) OnLeftInteract();

        nearInteractables.Remove(interactable);
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        //Don't move the player while the game is paused, or they are caught.
        if(GameManager.IsPaused || IsCaught) return;


        //Only allow the player to control movement when they're touching the ground.
        if(controller.isGrounded)
        {
            //Use the main camera's rotation to dictate direction of travel, but only around the y-axis.
            Quaternion rotation = Quaternion.Euler(0, cameraTransform.transform.eulerAngles.y, 0);

            //Direction the player will move.
            Vector3 direction = rotation * Vector3.forward * Input.GetAxis("Vertical") + rotation * Vector3.right * Input.GetAxis("Horizontal");
            controller.Move(direction.normalized * moveSpeed * Time.deltaTime);

            //Look at direction of travel.
            transform.LookAt(transform.position + direction);
        }
        //Pull the player towards the ground when in the air.
        else
        {
            controller.Move(Vector3.down * gravity * Time.deltaTime);
        }

        //Interact with all nearby interactables when the player presses the interact key.
        if(Input.GetButtonUp("Interact"))
        {
            foreach (IInteractable interactable in nearInteractables)
            {
                interactable.Interact();
            }
        }
    }
}