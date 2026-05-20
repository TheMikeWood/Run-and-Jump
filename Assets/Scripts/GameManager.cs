using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float initialGameSpeed = 5f;
    public float gamesSpeedIncrease = 0.1f;
    public float gameSpeed { get; private set; }

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;
    public TextMeshProUGUI narrativeText;
    public Button retryButton;

    public AudioSource musicSource;

    private Player player;
    private Spawner spawner;
    private float score;
    private float hiscore;
    private int lastMilestone = -1;

    private string[] narrativeLines = {
        "Just a few more blocks...",
        "The street lights are starting to flicker...",
        "It's getting dark. Run.",
        "Something's not right...",
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        spawner = FindObjectOfType<Spawner>();
        NewGame();
    }

    public void NewGame()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        foreach (var obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        gameSpeed = initialGameSpeed;
        score = 0f;
        enabled = true;
        lastMilestone = -1;

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        narrativeText.gameObject.SetActive(false);

        RestartMusic();
        UpdateHiscore();
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);

        StopMusic();
        UpdateHiscore();
    }

    private void Update()
    {
        gameSpeed += gamesSpeedIncrease * Time.deltaTime;
        score += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");

        CheckNarrativeMilestones();
    }

    private void CheckNarrativeMilestones()
    {
        int milestone = Mathf.FloorToInt(gameSpeed / 5f) - 1;
        if (milestone != lastMilestone && milestone >= 0 && milestone < narrativeLines.Length)
        {
            lastMilestone = milestone;
            ShowNarrativeText(narrativeLines[milestone]);
        }
    }

    private void ShowNarrativeText(string line)
    {
        narrativeText.text = line;
        narrativeText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideNarrativeText));
        Invoke(nameof(HideNarrativeText), 3f);
    }

    private void HideNarrativeText()
    {
        narrativeText.gameObject.SetActive(false);
    }

    private void UpdateHiscore()
    {
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);
        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }

        hiscoreText.text = Mathf.FloorToInt(hiscore).ToString("D5");
    }

    private void RestartMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.time = 0f;
            musicSource.pitch = 1f;
            musicSource.Play();
        }
    }

    private void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}