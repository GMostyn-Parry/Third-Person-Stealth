using UnityEngine;

/*
 * Movement expects the floor to always be Vector3.down.
 */
 [RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public delegate void Caught();
    public event Caught OnCaught;

    public float moveSpeed = 10.0f; //How fast the player moves.
    public float turnSpeed = 60.0f; //How fast the player turns.
    public float gravity = 5.0f; //Force of gravity.

    public bool IsCaught { get; private set; }

    private CharacterController controller; //Movement controller for the player.
    private Transform cameraTransform; //Transform of the camera used by the screen.

    public void Catch()
    {
        IsCaught = true;
        Destroy(this);
        
        OnCaught();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        //Don't move the player while the game is paused.
        if(GameManager.IsPaused) return;


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
    }
}