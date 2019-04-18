using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public Transform target; //The transform of the object we are following.
   
    public float radius = 3f; //Radius of the sphere the camera is traversing.
    public float maxPitch = 60f; //How much the camera can pitch.
    public float focusDistance = 50.0f; //How far forward to track past the target.

    public Vector2 offset = Vector2.one; //How offset we want the target to appear.
    public Vector2 mouseSensitivity = new Vector2(3, 2); //How fast the camera turns. 

    private float targetYaw;
    private float targetPitch;

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
        transform.position = target.position + rotation * -Vector3.forward * radius + rotation * Vector3.right * offset.x + Vector3.up * offset.y;

        transform.LookAt(target.position + rotation * Vector3.forward * focusDistance);
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
}
