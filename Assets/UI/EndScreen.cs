using UnityEngine;
using UnityEngine.UI;

//Class for controlling the screen shown the level ends, for whatever reason.
public class EndScreen : MonoBehaviour
{
    [SerializeField] private Text title = null; //Display whether the player succeeded, or not.
    [SerializeField] private Text timeDisplay = null; //Displays how much time the level took.

    [SerializeField] private Canvas winControls = null; //Controls that are shown when the player succeeds.
    
    //Set what is displayed on the end screen.
    public void SetFinishState(bool wasCaught, float timeBeforeEnd)
    {
        title.text = wasCaught ? "Failed To Escape" : "Successfully Escaped";
        //Display the time to finish to two decimal places.
        timeDisplay.text = timeBeforeEnd.ToString("f2") + "s";

        //Only show the win controls if they weren't caught.
        winControls.gameObject.SetActive(!wasCaught);
    }
}
