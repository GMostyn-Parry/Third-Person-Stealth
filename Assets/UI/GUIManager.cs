using UnityEngine;
using UnityEngine.UI;

//Manager for the GUI that can be used to control when elements are shown on the screen.
public class GUIManager : MonoBehaviour
{
    [SerializeField] private EndScreen endScreen = null; //The screen that is shown when the level ends.
    [SerializeField] private GameObject pauseScreen = null; //The screen that is shown when the game is paused.
    [SerializeField] private GameObject contextDialog = null; //A pop-up that gives the player context for the environment; i.e. they can interact with a nearby object.

    private Text contextDialogText; //Reference to the text component of the context dialog.
    
    //Show the end screen, and pass what information should be displayed on it.
    public void ShowEndScreen(bool wasCaught, float timeBeforeEnd)
    {
        endScreen.SetFinishState(wasCaught, timeBeforeEnd);
        endScreen.gameObject.SetActive(true);
    }

    public void HideEndScreen()
    {
        endScreen.gameObject.SetActive(false);
    }

    public void ShowPauseScreen()
    {
        pauseScreen.SetActive(true);
    }

    public void HidePauseScreen()
    {
        pauseScreen.SetActive(false);
    }

    //Shows a context dialog on the screen, and allows passing what text it should display.
    public void ShowContextDialog(string toDisplay)
    {
        contextDialogText.text = toDisplay;
        contextDialog.SetActive(true);
    }

    public void HideContextDialog()
    {
        contextDialog.SetActive(false);
    }

    private void Start()
    {
        contextDialogText = contextDialog.GetComponentInChildren<Text>();
    }
}
