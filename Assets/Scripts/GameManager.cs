using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } //Whether the game is paused; should be used to prevent scripts updating while paused.

    public List<string> levelOrder; //List of names of the levels to be loaded, and the order they are in.

    [SerializeField] private Player player = null; //Instance the user is controlling.
    [SerializeField] private GUIManager guiManager = null; //Reference to the mono behaviour that controls the GUI.

    private bool isLevelFinished = false; //Whether the level has finished.
    private float levelTime = 0f; //Time level has been running.
    private int levelOrderIndex; //Index of the level we are on from the levelOrder array.

    public void SetGamePaused(bool isPaused)
    {
        IsPaused = isPaused;
        SetCursorEnabled(isPaused);

        if(isPaused)
        {
            guiManager.ShowPauseScreen();
        }
        else
        {
            guiManager.HidePauseScreen();
        }
    }

    public void EndLevel()
    {
        IsPaused = true;
        SetCursorEnabled(true);

        guiManager.ShowEndScreen(player.IsCaught, levelTime);
    }

    public void RestartLevel()
    {
        StartCoroutine(LoadLevel(levelOrder[levelOrderIndex]));
    }

    public void NextLevel()
    {
        //If we are at the last level, then we go to the first level.
        levelOrderIndex = (levelOrderIndex + 1) % levelOrder.Count;

        //Go to the next level in the level order array.
        StartCoroutine(LoadLevel(levelOrder[levelOrderIndex]));
    }    

    public void QuitGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        SetCursorEnabled(false);
        SetGamePaused(false);

        if(SceneManager.sceneCount == 1)
        {
            LoadLevel(levelOrder[levelOrderIndex]);
        }
        else
        {
            string sceneName = SceneManager.GetActiveScene().name;

            if(sceneName == "Game")
            {
                Debug.LogError("Scene 'Game' is active scene, set active scene to level.");
            }
            else
            {
                levelOrderIndex = levelOrder.IndexOf(sceneName);
            }
        }

        FindPlayer(new Scene(), new LoadSceneMode());
    }

    private void Update()
    {
        //Toggle the pause state state, if we are not in the end state and the pause button was pressed.
        if(!isLevelFinished && Input.GetButtonUp("Pause"))
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
        SceneManager.sceneLoaded += FindPlayer;

        //Enable the end screen on the player reaching the finish area, or being caught.
        FinishArea.OnPlayerFinished += EndLevel;

        if(player)
        {
            player.OnCaught += EndLevel;
            player.OnEnterInteract += guiManager.ShowContextDialog;
            player.OnLeftInteract += guiManager.HideContextDialog;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= FindPlayer;

        FinishArea.OnPlayerFinished -= EndLevel;

        if(player)
        {
            player.OnCaught -= EndLevel;
            player.OnEnterInteract -= guiManager.ShowContextDialog;
            player.OnLeftInteract -= guiManager.HideContextDialog;
        }
    }

    private void SetCursorEnabled(bool isEnabled)
    {
        Cursor.lockState = isEnabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isEnabled;
    }

    private IEnumerator LoadLevel(string levelName)
    {
        guiManager.HideEndScreen();

        //Store the index, so we aren't constantly requesting the active scene.
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        yield return SceneManager.UnloadSceneAsync(currentBuildIndex);

        yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));

        //Reset the time in level, as we have spent zero time in the new level.
        levelTime = 0;

        SetGamePaused(false);
    }

    private void FindPlayer(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        player.OnCaught += EndLevel;
        player.OnEnterInteract += guiManager.ShowContextDialog;
        player.OnLeftInteract += guiManager.HideContextDialog;
    }
}
