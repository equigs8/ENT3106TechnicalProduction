using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    
    [Header("Game Settings")]
    public List<Fish> fishList = new List<Fish>();
    public int timer;


    public enum GameState { 
        Pre,
        In,
        Post 
    };
    [Header("Game State")]
    public GameState gameState;
    public bool isWinner;
    private bool displayedWinScreen;
    private bool displayedLoseScreen;

    [Header("Game Objects References")]
    [Header("Fish Meter")]
    public FishMeterHandler fishMeter;
    private float fishMeterValue;
    private float fishMeterWinMax;
    private float fishMeterWinMin;

    public TensionMeterHandler tensionMeter;
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("Fish")]
    public GameObject fishPrefab;
    public List<Sprite> fishTypes = new List<Sprite>();
    public Transform fishSpawnPoint;
    private Fish currentFish;
    public int score;
    public TextMeshProUGUI scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScoreText();
        
        //make sure there is only 1 game manager instance in scene
        if (FindObjectsByType<gameManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            gameState = GameState.Pre;
        }
        isWinner = false;

        // winScreen.SetActive(false);
        // loseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.Pre:
                scoreText.text = "Score: " + score;
                    if (Input.GetMouseButtonDown(0))
                    {
                        gameState = GameState.In;
                        fishMeterWinMax = Random.Range(-50f, 100f);
                        fishMeterWinMin = fishMeterWinMax - Random.Range(10, 100);
                        fishMeter.StartMeter(fishMeterWinMax,  fishMeterWinMin);


                    }
                break;
            case GameState.In:
                if (fishMeter.meterStarted)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        fishMeterValue = fishMeter.GetFishMeterValue();
                        fishMeter.meterStarted = false;
                        if (fishMeterValue <= fishMeterWinMax && fishMeterValue >= fishMeterWinMin)
                        {
                            score++;
                            UpdateScoreText();
                            Debug.Log("you hit");
                            currentFish = SpawnFish().GetComponent<Fish>();
                            currentFish.SetFishType(GetRandomFishType());

                            gameState = GameState.Post;

                        }
                        else
                        {
                            gameState = GameState.Post;
                            Debug.Log("you missed");
                        }
                    }
                }
                // if(fishMeter.GetStatus() == "win" )
                // {
                //     gameState = GameState.Post;
                //     isWinner = true;
                //     break;
                // }
                // else if (fishMeter.GetStatus() == "lose")
                // {
                //     gameState = GameState.Post;
                //     break;
                // }
                // fishMeter.UpdateFishMeter();

                break;
            case GameState.Post:
                Debug.Log("Post");
                if(isWinner && !displayedWinScreen)
                {
                    score++;
                    DisplayWinScreen();
                }else if (!isWinner && !displayedLoseScreen)
                {
                    DisplayLoseScreen();
                }
                break;
        }
    }


    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    public GameObject SpawnFish()
    {
        return Instantiate(fishPrefab, fishSpawnPoint.position, fishSpawnPoint.rotation);
    }

    public void DisplayWinScreen()
    {
        displayedWinScreen = true;
    }
    public void DisplayLoseScreen()
    {
        displayedLoseScreen = true;
    }

    public void ResetGame()
    {
        Debug.Log("Resetting game");
        scoreText.text = "Score: " + score;
        Destroy(currentFish.gameObject);
        gameState = GameState.Pre;
        displayedWinScreen = false;
        displayedLoseScreen = false;
    }


    public Sprite GetRandomFishType()
    {
        return fishTypes[Random.Range(0, fishTypes.Count)];
    }

}
