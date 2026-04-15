using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIScenes : MonoBehaviour
{
    [Header("Start Scene")] 
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    [Header("Main Game Scene")] [SerializeField]
    private string mainGameScene = "GameScene";
    
    [Header("Win Scene")]
    [SerializeField] private Button winRestartButton;
    [SerializeField] private Button winQuitButton;
    
    [Header("Loss Scene")]
    [SerializeField] private Button lossRestartButton;
    [SerializeField] private Button lossQuitButton;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // unlocking the cursor for any UI scenes
        
        startButton?.onClick.AddListener(StartGame);
        quitButton?.onClick.AddListener(QuitGame);
        
        winRestartButton?.onClick.AddListener(StartGame);
        winQuitButton?.onClick.AddListener(QuitGame);
        
        lossRestartButton?.onClick.AddListener(StartGame);
        lossQuitButton?.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainGameScene);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
