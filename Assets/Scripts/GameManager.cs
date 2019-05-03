using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Class that manages the high-level game state; changing level, pausing the game, etc.
public class GameManager : MonoBehaviour
{
    public delegate void Pause();
    public static event Pause OnPause; //Event emitted when the game is paused.

    public delegate void Unpause();
    public static event Unpause OnUnpause; //Event emitted when the game is unpaused.

    private static bool _isPaused;
    public static bool IsPaused  //Whether the game is paused; should be used to prevent scripts updating while paused.
    {
        get
        {
            return _isPaused;
        }

        private set
        {
            //Don't emit a signal if we aren't changing state.
            if(_isPaused == value) return;

            _isPaused = value;

            //Signal when the game is paused, and unpaused.
            if(_isPaused)
            {
                OnPause?.Invoke();
            }
            else
            {
                OnUnpause?.Invoke();
            }
        }
    }

    public List<string> levelOrder; //List of names of the levels to be loaded, and the order they are in.

    [SerializeField] private GUIManager guiManager = null; //Reference to the mono behaviour that controls the GUI.

    private Player player; //Instance the user is controlling.

    private bool isLevelFinished = false; //Whether the level has finished.
    private float levelTime = 0f; //Time level has been running.
    private int levelOrderIndex = 0; //Index of the level we are on from the levelOrder array.

    //Flag the level as finished, and show the end screen.
    public void EndLevel()
    {
        isLevelFinished = true;

        IsPaused = true;
        SetCursorEnabled(true);
        guiManager.ShowEndScreen(player.IsCaught, levelTime);
    }

    //Reload the level that is currently loaded.
    public void RestartLevel()
    {
        StartCoroutine(LoadLevel(levelOrder[levelOrderIndex]));
    }

    //Loads the next level in the level order array; wrapping round to the start of the array.
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

    //Sets whether to show the pause screen, while pausing the game.
    public void SetInPauseState(bool setToPausedState)
    {
        IsPaused = setToPausedState;
        SetCursorEnabled(setToPausedState);

        if(setToPausedState)
        {
            guiManager.ShowPauseScreen();
        }
        else
        {
            guiManager.HidePauseScreen();
        }
    }

    private void Start()
    {
        //Load the level, if we only have one scene loaded; i.e. the game scene.
        if(SceneManager.sceneCount == 1)
        {
            StartCoroutine(LoadLevel(levelOrder[levelOrderIndex]));
        }
        //Implicitly only called while using the editor.
        else
        {
            string sceneName = SceneManager.GetActiveScene().name;

            //Editor needs to be changed to set the level as the active scene if the Game scene is active.
            if(sceneName == "Game")
            {
                Debug.LogError("Scene 'Game' is active scene, set active scene to level.");
            }
            //Otherwise, set the levelOrderIndex to the index of the level that is currently loaded.
            else
            {
                levelOrderIndex = levelOrder.IndexOf(sceneName);
            }
        }

        //Fill the reference to the player.
        FindPlayer();

        SetCursorEnabled(false);
        IsPaused = false;
    }

    private void Update()
    {
        //Toggle the pause screen, if we are not in the end state and the pause button was pressed.
        if(!isLevelFinished && Input.GetButtonUp("Pause"))
        {
            SetInPauseState(!IsPaused);
        }

        //Increment level time, if the game is not paused.
        if(!IsPaused)
        {
            levelTime += Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        //The level ends if the player reaches a finish area.
        FinishArea.OnEnteredFinishArea += EndLevel;

        //Only link the player events if we have a player.
        if(player)
        {
            player.OnCaught += EndLevel;
            player.OnEnterInteract += guiManager.ShowContextDialog;
            player.OnLeftInteract += guiManager.HideContextDialog;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        FinishArea.OnEnteredFinishArea -= EndLevel;

        if(player)
        {
            player.OnCaught -= EndLevel;
            player.OnEnterInteract -= guiManager.ShowContextDialog;
            player.OnLeftInteract -= guiManager.HideContextDialog;
        }
    }

    //Hides and locks the cursor when it is disabled; otherwise it is free and visible.
    private void SetCursorEnabled(bool isEnabled)
    {
        Cursor.lockState = isEnabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isEnabled;
    }

    //Loads the level with the passed name.
    private IEnumerator LoadLevel(string levelName)
    {
        //No screen should be shown while, or after, the level is loaded.
        guiManager.HideEndScreen();
        guiManager.HidePauseScreen();
        guiManager.HideContextDialog();

        //Store the index, so we aren't constantly requesting the active scene.
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;

        //Don't unload the scene if the active scene is the Game scene.
        if(currentBuildIndex != 0)
        {
            yield return SceneManager.UnloadSceneAsync(currentBuildIndex);
        }

        yield return SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));

        //Wait a frame to allow the level to start before unpausing.
        yield return null;

        //Reset the manager's state for the new level.
        IsPaused = false;
        SetCursorEnabled(false);
        isLevelFinished = false;
        levelTime = 0;
    }

    //Finds the player in the loaded level, and assigns it to the player variable.
    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        player.OnCaught += EndLevel;
        player.OnEnterInteract += guiManager.ShowContextDialog;
        player.OnLeftInteract += guiManager.HideContextDialog;
    }

    //Find the player in the scene when a new level is loaded.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlayer();
    }
}
