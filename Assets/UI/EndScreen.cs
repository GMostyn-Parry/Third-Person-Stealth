using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private Text title = null; //Display whether the player succeeded, or not.
    [SerializeField] private Text timeDisplay = null; //Displays how much time the level took.

    [SerializeField] private Canvas winControls = null; //Controls that are shown when the player succeeds.
    
    public void SetFinishState(bool wasCaught, float timeBeforeEnd)
    {
        title.text = wasCaught ? "Failed To Escape" : "Successfully Escaped";
        //Display the time to finish to two decimal places.
        timeDisplay.text = timeBeforeEnd.ToString("f2") + "s";

        winControls.gameObject.SetActive(!wasCaught);
    }
}
