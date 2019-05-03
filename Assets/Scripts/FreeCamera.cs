using UnityEngine;
using UnityEngine.SceneManagement;

//Camera that follows just behind the target; only changing position and angle when input is passed by the user, or there is geometry blocking the way.
[RequireComponent(typeof(Camera))]
public class FreeCamera : MonoBehaviour
{
    public Transform target; //The transform of the object we are tracking/following.

    public float radius = 3f; //Radius of the sphere the camera is traversing.
    public float maxPitch = 60f; //How much the camera can pitch.
    public float focusDistance = 50.0f; //How far forward to track past the target.

    public Vector2 offset = Vector2.one; //How offset we want the target to appear.
    public Vector2 mouseSensitivity = new Vector2(3, 2); //How fast the camera turns.

    [SerializeField] private readonly float collisionPadding = 0.1f; //How much clearance the camera has when colliding with walls.

    private float targetYaw; //The yaw of the camera the user has requested, and we are trying to match.
    private float targetPitch; //The pitch of the camera the user has requested, and we are trying to match.

    private Camera cameraComponent; //Camera component of object.
    private LayerMask collisionMask; //Layer mask of what the camera can collide with.

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();

        //Set camera to collide with everything, but the player.
        collisionMask = ~0;
        collisionMask = ~LayerMask.GetMask("Player");
    }

    private void LateUpdate()
    {
        //Don't move the camera, if the game is paused.
        if(GameManager.IsPaused) return;

        Quaternion targetRotation = Quaternion.Euler(targetPitch, targetYaw, 0);

        //Using transform.right seems to cause camera stutter, so I recreate the vector with the rotation quaternion instead.
        Vector3 desiredPosition = target.position + targetRotation * -Vector3.forward * radius + targetRotation * Vector3.right * offset.x + Vector3.up * offset.y;

        //Set the camera to the position before collision.
        transform.position = desiredPosition;
        transform.LookAt(target.position + targetRotation * Vector3.forward * focusDistance);

        Vector3[] nearFrustrumCorners = GetNearPlaneCorners();

        float minCollisionDistance = -1f; //Smallest distance that the collision occured.
        Vector3 cornerDifference = new Vector3(); //Vector difference between corner collision was on, and the target.

        //Find the corner with the shortest distance before collision, out of all of the near view frustrum corners.
        foreach(Vector3 corner in nearFrustrumCorners)
        {
            if(Physics.Linecast(target.position, corner, out RaycastHit hit, collisionMask))
            {
                if(minCollisionDistance == -1f || hit.distance < minCollisionDistance)
                {
                    minCollisionDistance = hit.distance;
                    cornerDifference = corner - target.position;
                }
            }
        }

        //Move the camera, along the direction between the target and the colliding corner, to avoid the collision, if there was a collision.
        if(minCollisionDistance != -1f)
        {
            transform.position -= cornerDifference.normalized * (cornerDifference.magnitude - minCollisionDistance + collisionPadding);
        }
    }

    private void Update()
    {
        //Don't take input, if the game is paused.
        if(GameManager.IsPaused) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity.x;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity.y;

        //Update the target yaw and pitch based on player input.
        targetYaw = (targetYaw + mouseX) % 360;
        targetPitch = Mathf.Clamp(targetPitch + mouseY, -maxPitch, maxPitch);

        //Set desired position to "neutral" position behind the target, when the correct button is released.
        if(Input.GetButtonUp("Reset Camera"))
        {
            targetPitch = 0;
            targetYaw = target.eulerAngles.y;
        }        
    }

    //Get the near plane corners of the camera, relative to the camera's current transform.
    private Vector3[] GetNearPlaneCorners()
    {
        //Get an array of the near corners of the view frustrum.
        Vector3[] nearCorners = new Vector3[4];
        cameraComponent.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cameraComponent.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearCorners);

        //Transform the corners relative to the camera's current transform.
        for(int i = 0; i < nearCorners.Length; i++)
        {
            nearCorners[i] = transform.TransformVector(nearCorners[i]) + transform.position;
        }

        return nearCorners;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Locate the player, and set them as the tracking target for the camera.
    private void FindTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    //Find the target (player), when a new level is loaded.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindTarget();
    }
}
