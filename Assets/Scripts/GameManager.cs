using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } //Whether the game is paused; should be used to prevent scripts updating while paused.

    public string nextLevelName; //Name of the level to be loaded after this level.

    [SerializeField] private Player player = null; //Instance the user is controlling.
    [SerializeField] private EndScreen endScreen = null; //The screen that is shown when the level ends.
    [SerializeField] private Canvas pauseScreen = null; //The screen that is shown when the game is paused.

    private float levelTime = 0f; //Time level has been running.

    public void SetGamePaused(bool isPaused)
    {
        SetGameDisabled(isPaused);
        SetCursorEnabled(isPaused);
        pauseScreen.gameObject.SetActive(isPaused);
    }

    public void EndLevel()
    {
        SetGameDisabled(true);
        SetCursorEnabled(true);

        endScreen.SetFinishState(player.IsCaught, levelTime);
        endScreen.gameObject.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //Reset the time in level, as we have spent zero time in the restarted level.
        levelTime = 0;

        SetGamePaused(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }

    private void Start()
    {
        SetCursorEnabled(false);
        SetGamePaused(false);
    }

    private void Update()
    {
        //Toggle the pause state state, if we are not in the end state and the pause button was pressed.
        if(!endScreen.isActiveAndEnabled && Input.GetButtonUp("Pause"))
        {
            SetGamePaused(!IsPaused);
        }

        //Increment level time, if the game is not paused.
        if(!IsPaused)
        {
            levelTime += Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        //Enable the end screen on the player reaching the finish area, or being caught.
        FinishArea.OnPlayerFinished += EndLevel;
        player.OnCaught += EndLevel;
    }

    private void OnDisable()
    {
        FinishArea.OnPlayerFinished -= EndLevel;
        player.OnCaught -= EndLevel;
    }

    private void SetCursorEnabled(bool isEnabled)
    {
        Cursor.lockState = isEnabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isEnabled;
    }

    private void SetGameDisabled(bool isDisabled)
    {
        IsPaused = isDisabled;
    }
}
