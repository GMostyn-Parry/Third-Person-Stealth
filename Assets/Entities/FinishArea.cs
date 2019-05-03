using UnityEngine;

//Class that emits an event when the player enters the game object's trigger.
public class FinishArea : MonoBehaviour
{
    public delegate void EnterFinishArea();
    public static event EnterFinishArea OnEnteredFinishArea; //Emitted when the player enters the game object's trigger.

    //Emit signal when player enters trigger area.
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            OnEnteredFinishArea?.Invoke();
        }
    }
}
