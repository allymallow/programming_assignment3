using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    //allowing me to set the inputs to enable pause
    [SerializeField] private InputAction pauseInput;

    [Header("UI Canvases")] [SerializeField]
    private Canvas pauseCanvas;

    [SerializeField] private Canvas startCanvas;
    [SerializeField] private Canvas lossCanvas;
    [SerializeField] private Canvas winCanvas;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Timer")] [SerializeField] private int gameTime; //maximum gametime before game over
    private float _timer; //integer to run by delta time, acting as a timer
    private int _score;
    private bool _isPaused;


    void OnEnable()
    {
        pauseInput.Enable();
        pauseInput.performed += Pause;
        EnemyController.EnemyDestroyed += IncreaseScore;
        ChestInteractable.ChestDestroyed += IncreaseScore;
    }

    void OnDisable()
    {
        pauseInput.performed -= Pause;
        EnemyController.EnemyDestroyed -= IncreaseScore;
        ChestInteractable.ChestDestroyed -= IncreaseScore;
    }

   
    
    public void Start()
    {
        _timer = 0;
        startCanvas.gameObject.SetActive(true); //show the start canvas
        Time.timeScale = 0; //freeze game while start canvas on screen
        _score = 0; //ensure score is set to 0 at start
    }
    
    void Update()
    {
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

        //setting the win state
        if (_score == 7)
        {
            WinGame();
        }
    }

    //update the timer and score text on the canvas while game is playing 
    void UpdateUI()
    {
        timerText.text = "Time: " + _timer;
        scoreText.text = "Score: " + _score;
    }

    private void Pause(InputAction.CallbackContext context)
    {
        //lines 71 and 72 copied from AI, I do understand the general process but just had some 
        //difficulty getting the proper syntax, and have included a copy of the AI chat in my documentation for transparency.
        if (context.performed)
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0 : 1;
            //toggle _isPaused bool when pause button is pressed
            //when bool is toggled, so is timescale so the game actually freezes/unfreezes

            pauseCanvas.gameObject.SetActive(_isPaused); // setting the pause panel visibility
        }
    }

    void GameOver()
    {
        Debug.Log("GameOver");
        Cursor.lockState = CursorLockMode.None; //allowing the cursor to move freely 
        lossCanvas.gameObject.SetActive(true); //activating the loss/GameOver screen
        Time.timeScale = 0; //freezing/pausing the game
    }

    void WinGame()
    {
        Cursor.lockState = CursorLockMode.None; // allowing the cursor to move freely when UI panel is on screen
        winCanvas.gameObject.SetActive(true); // activating the win menu
        Time.timeScale = 0; //freezing the game 
    }

    //method is public to allow it to be selected by UI buttons in the inspector
    public void Playing()
    {
        //ensuring the UI panels aren't visible in play mode
        startCanvas.gameObject.SetActive(false);
        lossCanvas.gameObject.SetActive(false);
        winCanvas.gameObject.SetActive(false);
        Time.timeScale = 1; //allowing the game to play normally (not paused)
        Cursor.lockState = CursorLockMode.Locked; // cursor locked for play mode

    }

    //quitting the game -> made public so it can be accessed in the inspector for the "Quit" button on UI panels
    public void QuitGame()
    {
        Application.Quit(); 
    }
}
