using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    //allowing me to set the inputs to enable pause
    [SerializeField] private InputAction pauseInput;

    [Header("UI ")] 
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Timer")] 
    [SerializeField] private int gameTime; //maximum gametime before game over
    private float _timer; //float to run by delta time, acting as a timer
    private int _score;
    private bool _isPaused;
    private int _health;

    [Header("Scene Management")] 
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private string lossSceneName = "LossScene";

    private bool _gameEnded; //flag to stop Update from triggering GameOver repeatedly
    
    void OnEnable()
    {
        pauseInput.Enable();
        pauseInput.performed += Pause;
        MeleeEnemyController.EnemyDestroyed += IncreaseScore;
        RangedEnemyController.EnemyDestroyed += IncreaseScore;
        ChestInteractable.ChestDestroyed += IncreaseScore;
        PlayerController.OnPlayerDied += GameOver;
        PlayerController.OnHealthChanged += UpdateHealth; 
    }

    void OnDisable()
    {
        pauseInput.performed -= Pause;
        MeleeEnemyController.EnemyDestroyed -= IncreaseScore;
        RangedEnemyController.EnemyDestroyed -= IncreaseScore;
        ChestInteractable.ChestDestroyed -= IncreaseScore;
        PlayerController.OnPlayerDied -= GameOver;
        PlayerController.OnHealthChanged -= UpdateHealth;
    }
   
    
    public void Start()
    {
        Time.timeScale = 1;
        _timer = 0;
        _score = 0; //ensure score is set to 0 at start
        _gameEnded = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        if (_gameEnded) return; //if game is over, stop Updates to prevent errors with scene loading
        
         _timer += Time.deltaTime; //ensuring timer runs consistently
        UpdateUI();
        if (_timer > gameTime) //if the timer goes past the maximum allotted game time, call the game over method
        {
            GameOver();
        }
    }
 
    void IncreaseScore(int score)
    {
        _score += score; //increase existing score by adding new _score points upon chest or enemy destruction
        
        if (_score >= 30)
            WinGame();
    }

    void UpdateHealth(int health)
    {
        _health = health;
    }

    //update the timer and score text on the canvas while game is playing 
    void UpdateUI()
    {
        timerText.text = "Time: " + (gameTime - _timer).ToString("F0");
        healthText.text = "Health: " + _health;
        scoreText.text = "Score: " + _score;
    }

    private void Pause(InputAction.CallbackContext context)
    {
        if (_gameEnded) return;
        
        if(context.performed)
        {
            _isPaused = !_isPaused; //copied from AI as noted in assignment 2 documentation
            Time.timeScale = _isPaused ? 0 : 1; //copied from AI as noted in assignment 2 documentation
            //toggle _isPaused bool when pause button is pressed
            //when bool is toggled, so is timescale so the game actually freezes/unfreezes

            pauseCanvas.gameObject.SetActive(_isPaused); // setting the pause panel visibility
            //unfreeze the cursor on pause to allow players to use the UI buttons
            Cursor.lockState = _isPaused ? CursorLockMode.None : CursorLockMode.Locked; 
        }
    }

    //public method to allow the pause screen's resume button to work
    public void OnResumePressed()
    {
        _isPaused = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        pauseCanvas.gameObject.SetActive(false);
    }

    //pause the game and switch to the game over/loss screen
    void GameOver()
    {
        _gameEnded = true;
        Time.timeScale = 0;
        SceneManager.LoadScene(lossSceneName);
    }
    
   //pause and switch to the win screen
    void WinGame()
    {
        _gameEnded = true;
        Time.timeScale = 0; 
        SceneManager.LoadScene(winSceneName);

    }
    
    
    //quitting the game -> made public so it can be accessed in the inspector for the "Quit" button on UI panels
    public void QuitGame()
    {
        Application.Quit(); 
    }
}
