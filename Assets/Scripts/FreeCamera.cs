using UnityEngine;
using UnityEngine.SceneManagement;

//Camera that follows just behind the target, and only changes angle when input is passed by the user; or it has avoid shearing geometry, or the target.
[RequireComponent(typeof(Camera))]
public class FreeCamera : MonoBehaviour
{
    public Transform target; //The transform of the object we are tracking/following.

    public float radius = 3f; //Radius of the sphere the camera is traversing.
    public float maxPitch = 60f; //How much the camera can pitch.
    public float focusDistance = 50.0f; //How far forward to track past the target.

    public Vector2 offset = new Vector2(1, 1); //How much we want to offset the target from the centre of the view.
    public Vector2 mouseSensitivity = new Vector2(3, 2); //How fast the camera turns on mouse input.

    [SerializeField] private readonly float collisionPadding = 0.1f; //How much clearance is added to avoid shearing geometry.
    [SerializeField] private readonly float avoidHeight = 1.5f; //Height above the target's transform we move to, so we can avoid shearing the target when next to a wall.

    private float targetYaw; //The yaw of the camera the user has requested, and we are trying to match.
    private float targetPitch; //The pitch of the camera the user has requested, and we are trying to match.

    private Camera cameraComponent; //Camera component of object.
    private LayerMask collisionMask; //Layer mask of what the camera can collide with.

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();

        //Set camera to collide with everything, but the player.
        collisionMask = ~0;
        collisionMask = ~LayerMask.GetMask("Ignore Raycast", "Player");
    }

    //Update the camera's position, and rotation, after all of the other game objects have moved.
    private void LateUpdate()
    {
        //Don't move the camera, if the game is paused.
        if(GameManager.IsPaused) return;

        Quaternion targetRotation = Quaternion.Euler(targetPitch, targetYaw, 0);

        //Using transform.right will cause camera stutter, as the camera's transform.right changes at the end of this update, so I recreate the vector with the rotation quaternion instead.
        Vector3 desiredPosition = target.position + targetRotation * -Vector3.forward * radius + targetRotation * Vector3.right * offset.x + Vector3.up * offset.y;

        Vector3[] nearFrustrumCorners = GetNearPlaneCorners(desiredPosition, targetRotation);

        //What we will subtract from the camera position to avoid shearing geometry.
        Vector3 avoidDirection = Vector3.zero;
        //The smallest collision distance from target to any of the frustrum corners.
        float minCollisionDistance = radius + 1;

        //Find the mininimum collision distance,
        //and set the travel direction to a vector made from the largest components out of all avoidance vectors for the corners.
        foreach(Vector3 corner in nearFrustrumCorners)
        {
            //Only perform calculation if a collision occured; we travel from the target to the corner, rather that vice versa,
            //as there won't be a collision if the starting point is inside the offending geometry.
            if(Physics.Linecast(target.position, corner, out RaycastHit hit, collisionMask))
            {
                //Vector difference between corner and the target's position.
                Vector3 cornerDifference = corner - target.position;
                //How much we need to move the camera along the vector difference to avoid shearing geometry.
                Vector3 collisionAvoid = cornerDifference.normalized * (cornerDifference.magnitude - hit.distance + collisionPadding);

                //Replace any component in avoidDirection that is smaller than the current avoidance vector.
                if(Mathf.Abs(collisionAvoid.x) > Mathf.Abs(avoidDirection.x)) avoidDirection.x = collisionAvoid.x;
                if(Mathf.Abs(collisionAvoid.y) > Mathf.Abs(avoidDirection.y)) avoidDirection.y = collisionAvoid.y;
                if(Mathf.Abs(collisionAvoid.z) > Mathf.Abs(avoidDirection.z)) avoidDirection.z = collisionAvoid.z;

                //Update minCollisionDistance if the new hit distance was smaller than the current value.
                if(hit.distance < minCollisionDistance) minCollisionDistance = hit.distance;
            }
        }

        //Move the camera to avoid the collision, if there was a collision.
        if(avoidDirection != Vector3.zero)
        {
            //We subtract because the linecasts were from the target to the camera corners, rather than vice versa.
            desiredPosition -= avoidDirection;

            //We interpolate between the current height and the height to avoid shearing the target, based on how close we are to the colliding geometry.
            Vector3 squeezePosition = new Vector3(desiredPosition.x, target.position.y + avoidHeight, desiredPosition.z);
            desiredPosition = Vector3.Lerp(desiredPosition, squeezePosition, avoidDirection.magnitude / minCollisionDistance);
        }

        //Set the camera's transform with the calculated values.
        transform.position = desiredPosition;
        transform.LookAt(target.position + targetRotation * Vector3.forward * focusDistance);
    }

    //Take input, and update the camera's state.
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

    //Get the near plane corners of the camera, as if the camera's transform was set to the passed values.
    private Vector3[] GetNearPlaneCorners(Vector3 cameraPosition, Quaternion rotation)
    {
        //Get an array of the near corners of the view frustrum.
        Vector3[] nearCorners = new Vector3[4];
        cameraComponent.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cameraComponent.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearCorners);

        //Transform the corners relative to the camera's position, and rotation.
        for(int i = 0; i < nearCorners.Length; i++)
        {
            nearCorners[i] = rotation * nearCorners[i];
            nearCorners[i] += cameraPosition;
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