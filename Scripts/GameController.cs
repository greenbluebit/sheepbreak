using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameController : MonoBehaviour {

    public static GameController Instance;
    public bool isGameOver = true;
    private bool canStart = true;

    public float restartDelay = 2;

    public GameObject startSheep;

    public float score = 0;

    public GameObject sheepPrefab;

    public float maxWaveAmount;
    public Vector3 sheepSpawnPosition;
    public int defaultInstancedSheepAmount;
    public List<GameObject> instancedSheep = new List<GameObject>();
    public List<GameObject> activeSheep = new List<GameObject>();

    public MenuController startGame;
    public MenuController endGame;

    public LayerMask clickTargets;

    public AudioSource jump;
    public AudioSource hitFence;

    public bool isPaused = false;

    private string scoreLength = "000000000";
    public Text scoreText;
    // Use this for initialization
    void Awake () {
		if(Instance != null)
        {
            Destroy(Instance);
        }
        /*
        int width = 620; // or something else
        int height = 620; // or something else
        bool isFullScreen = false; // should be windowed to run in arbitrary resolution
        int desiredFPS = 60; // or something else

        Screen.SetResolution(width, height, isFullScreen, desiredFPS);
        */
        Instance = this;

        SetupSheep();
	}

    void SetupSheep()
    {
        for (var i = 0; i < defaultInstancedSheepAmount; i++)
        {
            CreateSheep();
        }
    }

    void CreateSheep()
    {
        GameObject newSheep = Instantiate(sheepPrefab, sheepSpawnPosition, Quaternion.identity, this.transform);
        newSheep.SetActive(false);
        instancedSheep.Add(newSheep);
    }

    public void StartWave()
    {
        print("START WAVE");
        ActivateSheep(sheepSpawnPosition, Random.Range(1, maxWaveAmount));
    }

    private void ActivateSheep(Vector2 position, float amount)
    {
        GameObject sheep = null;
        if (instancedSheep.Count > 0)
        {
            sheep = instancedSheep.PopAt(0);
        } else
        {
            CreateSheep();
            sheep = instancedSheep.PopAt(0);
        }

        sheep.transform.position = new Vector2(position.x + Random.Range(1.1f, 3.5f), position.y);
        activeSheep.Add(sheep);
        sheep.SetActive(true);
        amount--;
        if(amount > 0)
        {
            ActivateSheep(sheep.transform.position, amount);
        }
    }

    public void SleepAllSheep()
    {
        while(activeSheep.Count > 0)
        {
            GameObject sheep = activeSheep.PopAt(activeSheep.Count - 1);
            sheep.SetActive(false);
            instancedSheep.Add(sheep);
        }
    }

    public void SleepSheep(GameObject sheep)
    {
        sheep.SetActive(false);
        activeSheep.PopAt(activeSheep.IndexOf(sheep));
        instancedSheep.Add(sheep);
        if(activeSheep.Count <= 0)
        {
            AddScore();
            StartCoroutine(StartWaveDelayed());
        }
    }

    public void AddScore()
    {
        score++;
        if(scoreText != null)
        {
            int neededZeros = scoreLength.Length - score.ToString().Length;
            string tempScore = "";
            for(var i = 0; i < neededZeros; i++)
            {
                tempScore += "0";
            }
            scoreText.text = "SCORE: " + tempScore + score.ToString();
        }
    }

    public void GameOver()
    {
        hitFence.Play();
        isGameOver = true;
        StartCoroutine(AllowRestart());
        endGame.ShowUi();
    }

    private IEnumerator AllowRestart()
    {
        yield return new WaitForSeconds(.5f);
        canStart = true;
    }

    public void StartGame()
    {
        endGame.hideUI();
        canStart = false;
        StartCoroutine("StartGameDelayed");
    }

    public void RestartGame()
    {
        endGame.hideUI();
        canStart = false;
        score = 0;
        if (scoreText != null)
        {
            string tempScore = "";
            for (var i = 0; i < scoreLength.Length-1; i++)
            {
                tempScore += "0";
            }
            scoreText.text = "SCORE: " + tempScore + score.ToString();
        }
        SleepAllSheep();
        if (startSheep != null)
        {
            startSheep.GetComponent<Sheep>().Reset();
        }
        
        StartCoroutine(StartGameDelayed());
    }

    private IEnumerator StartWaveDelayed()
    {
        yield return new WaitForSeconds(.4f);
        StartWave();
    }

    private IEnumerator StartGameDelayed()
    {
        print("START GAME DELAYED");
        yield return new WaitForSeconds(restartDelay);
        isGameOver = false;
        StartWave();
    }

    private void StartGameNoDelay()
    {
        isGameOver = false;
        StartWave();
    }

    private void Update()
    {
        checkClick();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                isPaused = false;
                Time.timeScale = 1;
            } else
            {
                isPaused = true;
                Time.timeScale = 0;
            }
            
        }
        if(canStart && Input.anyKeyDown)
        {
            print ("RESTARTING GAME");
            RestartGame();
        }
    }

    public void ResetStartSheep()
    {
        AddScore();
        Sheep startSheepScript = startSheep.GetComponent<Sheep>();
        startSheepScript.startedSheep = false;
        startSheep.transform.position = startSheepScript.startPosition;
        startSheep.transform.rotation = startSheepScript.startRotation;
        StartGameNoDelay();
    }

    void checkClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, (int) clickTargets);
            if (hit && hit.collider.gameObject.tag.ToLower() == "sheep")
            {
                Sheep sheep = hit.collider.gameObject.GetComponent<Sheep>();

                Sheep startSheepScript = null;
                if(startSheep != null)
                {
                    startSheep.GetComponent<Sheep>();
                }
                
                if (startSheep != null && sheep == startSheepScript && startSheepScript.startSheep)
                {
                    jump.Play();
                    sheep.ChargeAndStart();
                    isGameOver = false;
                    return;
                }
                if(sheep != null && isGameOver == false)
                {
                    jump.Play();
                    sheep.Jump();
                }
            }
        }
    }
}
