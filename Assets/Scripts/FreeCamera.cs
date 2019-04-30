using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class FreeCamera : MonoBehaviour
{
    public Transform target; //The transform of the object we are following.

    public float radius = 3f; //Radius of the sphere the camera is traversing.
    public float maxPitch = 60f; //How much the camera can pitch.
    public float focusDistance = 50.0f; //How far forward to track past the target.

    public Vector2 offset = Vector2.one; //How offset we want the target to appear.
    public Vector2 mouseSensitivity = new Vector2(3, 2); //How fast the camera turns.

    [SerializeField] private float collisionPadding = 0.1f; //How much clearance the camera has when colliding with walls.

    private float targetYaw;
    private float targetPitch;

    private Camera cameraComponent; //Camera component of object.

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        //Don't move the camera, if the game is paused.
        if(GameManager.IsPaused)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity.x;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity.y;

        targetYaw = (targetYaw + mouseX) % 360;
        targetPitch = Mathf.Clamp(targetPitch + mouseY, -maxPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(targetPitch, targetYaw, 0);

        //Using transform.right seems to cause camera stutter, so I recreate the vector with the rotation quaternion instead.
        Vector3 desiredPosition = target.position + rotation * -Vector3.forward * radius + rotation * Vector3.right * offset.x + Vector3.up * offset.y;

        transform.position = desiredPosition;
        transform.LookAt(target.position + rotation * Vector3.forward * focusDistance);

        Vector3[] nearCorners = GetNearPlaneCorners();

        float distance = -1f;
        Vector3 difference = new Vector3();

        foreach(Vector3 corner in nearCorners)
        {
            if(Physics.Linecast(target.position, corner, out RaycastHit hit, LayerMask.GetMask("Geometry")))
            {
                if(distance == -1f || hit.distance < distance)
                {
                    distance = hit.distance;
                    difference = corner - target.position;
                }
            }
        }

        if(distance != -1f)
        {
            transform.position -= difference.normalized * (difference.magnitude - distance + collisionPadding);
        }
    }

    private void Update()
    {
        //Set desired position to "neutral" position behind the target.
        if(Input.GetButtonDown("Reset Camera"))
        {
            targetPitch = 0;
            targetYaw = target.eulerAngles.y;
        }
    }

    private Vector3[] GetNearPlaneCorners()
    {
        Vector3[] nearCorners = new Vector3[4];
        cameraComponent.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cameraComponent.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearCorners);

        for(int i = 0; i < nearCorners.Length; i++)
        {
            nearCorners[i] = transform.TransformVector(nearCorners[i]) + transform.position;
        }

        return nearCorners;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += FindTarget;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= FindTarget;
    }

    private void FindTarget(Scene scene, LoadSceneMode mode)
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
