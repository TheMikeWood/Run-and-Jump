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
    public AudioSource titleMusicSource;

    private Player player;
    private Spawner spawner;
    private float score;
    private float hiscore;

    // Phase thresholds — match DayNightCycle and StreetLamp
    private const float SunsetSpeed  = 10f;
    private const float NightSpeed   = 20f;
    private const float TwistedSpeed = 35f;

    // Lines fire every 5 speed units within each phase, issue: the days are too short to actually play all messages from mom
    private const float LineInterval = 5f;
    private float nextLineAt;

    private int dayIndex     = 0;
    private int sunsetIndex  = 0;
    private int nightIndex   = 0;
    private int twistedIndex = 0;

    private string[] dayLines = {
        "Mom: Grace, dinner's almost ready.",
        "Mom: Don't forget your jacket.",
        "Mom: Be home before dark, okay?",
        "Mom: I love you. Stay on the main road.",
        "Mom: Text me when you're close.",
        "Mom: Your brother keeps asking when you'll be back.",
        "Mom: I made your favorite tonight.",
    };

    private string[] sunsetLines = {
        "Mom: Grace, the streetlights are coming on.",
        "Mom: Honey, it's getting late.",
        "Mom: Where are you? It's almost dark.",
        "Mom: Grace, please answer me.",
        "Mom: I'm starting to worry.",
        "Mom: You know the rule. Be home by dark.",
        "Mom: Grace? Did your phone die?",
    };

    private string[] nightLines = {
        "Mom: Grace, call me right now.",
        "Mom: I don't care what happened. Just come home.",
        "Mom: Something feels wrong tonight. Please hurry.",
        "Mom: Grace? How far are you?",
        "Mom: Don't take the alley. Stay in the light.",
        "Mom: I heard something outside.",
        "Mom: The Hendersons' dog won't stop barking.",
        "Mom: Grace I need you to answer me RIGHT NOW.",
        "Mom: I'm coming to find you.",
        "Mom: No. Stay inside. I'm coming to you.",
    };

    private string[] twistedLinesPool = {
        "Mom: Is that you at the door?",
        "Mom: I can hear screaming outside.",
        "Mom: There's someone in the yard. It doesn't look right.",
        "Mom: Grace... your brother says he sees you. But you're not here.",
        "Mom: I opened the door. I shouldn't have opened the door.",
        "Mom: It's wearing my clothes.",
        "Mom: GRACE RUN DO NOT COME HOME",
        "Mom: I'm so proud of you baby. I love—",
        "Mom: We're okay. Just get home. Everything is fine.",
        "Mom: Don't stop running. Don't look at it.",
        "Mom: I can't remember what you look like.",
        "Mom: She's already here. She's been here.",
        "Mom: GRACE!",
        "Mom: Come home. We're waiting for you.",
        "Mom: The lights went out. All of them.",
        "Mom: I think I made a mistake.",
    };

    private string[] activeTwistedLines;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DestroyImmediate(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        player  = FindObjectOfType<Player>();
        spawner = FindObjectOfType<Spawner>();

        player.gameObject.SetActive(false);
        spawner.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        narrativeText.gameObject.SetActive(false);
        enabled = false;

        if (titleMusicSource != null)
            titleMusicSource.Play();
    }

    public void NewGame()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        foreach (var obstacle in obstacles)
            Destroy(obstacle.gameObject);

        gameSpeed    = initialGameSpeed;
        score        = 0f;
        enabled      = true;
        nextLineAt   = initialGameSpeed + LineInterval;
        dayIndex     = 0;
        sunsetIndex  = 0;
        nightIndex   = 0;
        twistedIndex = 0;

        activeTwistedLines = GetRandomSubset(twistedLinesPool, 8);

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        narrativeText.gameObject.SetActive(false);

        FindObjectOfType<DayNightCycle>()?.ResetCycle();

        RestartMusic();
        UpdateHiscore();
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled   = false;

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
        score     += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");

        CheckNarrativeMilestones();
    }

    private void CheckNarrativeMilestones()
    {
        if (gameSpeed < nextLineAt) return;

        nextLineAt += LineInterval;

        string line = null;

        if (gameSpeed < SunsetSpeed)
        {
            if (dayIndex < dayLines.Length)
                line = dayLines[dayIndex++];
        }
        else if (gameSpeed < NightSpeed)
        {
            if (sunsetIndex < sunsetLines.Length)
                line = sunsetLines[sunsetIndex++];
        }
        else if (gameSpeed < TwistedSpeed)
        {
            if (nightIndex < nightLines.Length)
                line = nightLines[nightIndex++];
        }
        else
        {
            if (twistedIndex < activeTwistedLines.Length)
                line = activeTwistedLines[twistedIndex++];
        }

        if (line != null)
            ShowNarrativeText(line);
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
        if (titleMusicSource != null)
            titleMusicSource.Stop();

        if (musicSource != null)
        {
            musicSource.Stop();
            musicSource.time  = 0f;
            musicSource.pitch = 1f;
            musicSource.Play();
        }
    }

    private void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    private string[] GetRandomSubset(string[] pool, int count)
    {
        string[] shuffled = (string[])pool.Clone();
        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        string[] result = new string[count];
        System.Array.Copy(shuffled, result, count);
        return result;
    }
}