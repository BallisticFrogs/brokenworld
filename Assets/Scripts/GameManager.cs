using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;

    public bool isStarted;
    public bool gamePaused;
    public bool gameOver;


    [SceneObjectsOnly] public ParticleSystem mergeParticleSystem;
    [SceneObjectsOnly] public TMP_Text fieldFollowers;

    [SceneObjectsOnly] public GameObject tutoPanel;
    [SceneObjectsOnly] public GameObject pausePanel;
    [SceneObjectsOnly] public GameObject gameOverPanel;
    [SceneObjectsOnly] public TMP_Text gameOverPanelText;

    private void Awake()
    {
        INSTANCE = this;
    }

    private void Start()
    {
        tutoPanel.SetActive(true);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (gameOver)
        {
            return;
        }

        if (PlayerController.INSTANCE.energy <= 0)
        {
            GameOver("You died in the great void...\nfar from your followers and their faith in you");
        }

        if (PlayerController.INSTANCE.followers <= 0)
        {
            GameOver("All your followers died of hunger...\nand the idea of you with them");
        }

        if (gameOver)
        {
            return;
        }

        if (!isStarted && Input.GetMouseButton((int) MouseButton.LeftMouse))
        {
            tutoPanel.SetActive(false);
            isStarted = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            gamePaused = !gamePaused;
            if (gamePaused)
            {
                PauseGame();
            }
            else
            {
                UnpauseGame();
            }
        }
    }

    public void UpdateInfoPanel()
    {
        fieldFollowers.text = PlayerController.INSTANCE.followers + "";
    }

    public void PlayIslandMergeVFX(Vector3 contactPoint)
    {
        mergeParticleSystem.transform.position = contactPoint;
        mergeParticleSystem.Play();
    }

    public void QuitGame()
    {
#if UNITY_STANDALONE
        // if we are running in a standalone build of the game
        Application.Quit();
#endif

#if UNITY_EDITOR
        // if we are running in the editor
        EditorApplication.isPlaying = false;
#endif
    }

    public void GameOver(string text)
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        gameOver = true;
        gameOverPanel.SetActive(true);
        gameOverPanelText.text = text;

        // TODO
    }

    public void RestartGame()
    {
        gameOver = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Time.timeScale = 0;
        AudioListener.pause = true;
        pausePanel.SetActive(true);
    }

    public void UnpauseGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1;
        AudioListener.pause = false;
        pausePanel.SetActive(false);
    }
}