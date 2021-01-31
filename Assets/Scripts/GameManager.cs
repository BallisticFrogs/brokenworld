using System.Collections.Generic;
using DG.Tweening;
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

    private List<Island> islands = new List<Island>();

    [SceneObjectsOnly] public ParticleSystem mergeParticleSystem;
    [SceneObjectsOnly] public TMP_Text fieldFollowers;
    [SceneObjectsOnly] public TMP_Text fieldFood;

    [SceneObjectsOnly] public GameObject pausePanel;
    [SceneObjectsOnly] public GameObject victoryPanel;
    [SceneObjectsOnly] public GameObject gameOverPanel;
    [SceneObjectsOnly] public TMP_Text gameOverPanelText;

    private void Awake()
    {
        INSTANCE = this;
        DOTween.Init();
    }

    private void Start()
    {
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);

        var objs = GameObject.FindGameObjectsWithTag("island");
        foreach (var obj in objs)
        {
            islands.Add(obj.GetComponent<Island>());
        }
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

        if (!islands.Exists(i => !i.connected))
        {
            Victory();
        }

        if (gameOver)
        {
            return;
        }

        if (!isStarted && Input.GetMouseButton((int) MouseButton.LeftMouse))
        {
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
        fieldFood.text = PlayerController.INSTANCE.food + "";
    }

    public void OnIslandMerge(Vector3 contactPoint)
    {
        mergeParticleSystem.transform.position = contactPoint;
        mergeParticleSystem.Play();

        SoundManager.INSTANCE.Play(SoundManager.INSTANCE.aggregateIsland, 1);
        SoundManager.INSTANCE.Play(SoundManager.INSTANCE.peopleCheering, 1, 0.8f, 1.5f);
        SoundManager.INSTANCE.Play(SoundManager.INSTANCE.peopleCheering, 1, 0.5f, 0.7f);
        
        TutoManager.INSTANCE.islandsConnected++;
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
        SoundManager.INSTANCE.StopAllSounds();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        gameOver = true;
        gameOverPanel.SetActive(true);
        gameOverPanelText.text = text;
    }

    public void Victory()
    {
        SoundManager.INSTANCE.StopAllSounds();

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        gameOver = true;
        victoryPanel.SetActive(true);
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
        SoundManager.INSTANCE.StopAllSounds();

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