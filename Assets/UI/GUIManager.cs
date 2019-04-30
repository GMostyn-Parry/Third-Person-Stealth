using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private EndScreen endScreen = null; //The screen that is shown when the level ends.
    [SerializeField] private GameObject pauseScreen = null; //The screen that is shown when the game is paused.
    [SerializeField] private GameObject contextDialog = null; //A pop-up that tells the player they can interact with objects.

    private Text contextDialogText; //Reference to the text component of the context dialog.
    
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
