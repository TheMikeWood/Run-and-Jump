using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Speed")]
    public float initialGameSpeed = 5f;
    public float gamesSpeedIncrease = 0.1f;
    public float maxStageSpeed = 25f;
    public float gameSpeed { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;
    public TextMeshProUGUI narrativeText;
    public Button retryButton;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioSource titleMusicSource;

    [Header("Stage Music")]
    [SerializeField]
    private AudioClip dayMusic;

    [SerializeField]
    private AudioClip sunsetMusic;

    [SerializeField]
    private AudioClip nightMusic;

    [SerializeField]
    private AudioClip twistedMusic;

    [Header("Death")]
    [SerializeField]
    private float gameOverDelay = 1f;

    [Header("Stage Lengths")]
    [SerializeField]
    private float dayStageLength = 75f;

    [SerializeField]
    private float sunsetStageLength = 75f;

    [SerializeField]
    private float nightStageLength = 105f;

    [SerializeField]
    private float twistedStageLength = 105f;

    [Header("Stage Screens")]
    [SerializeField]
    private GameObject stagePanel;

    [SerializeField]
    private TextMeshProUGUI stageText;

    [SerializeField]
    private Image stageClearImage;

    [Header("Stage Clear Images")]
    [SerializeField]
    private Sprite dayClearImage;

    [SerializeField]
    private Sprite sunsetClearImage;

    [SerializeField]
    private Sprite nightClearImage;

    [SerializeField]
    private Sprite twistedClearImage;

    [SerializeField]
    private float stageEndScreenDuration = 2f;

    [SerializeField]
    private float stageStartScreenDuration = 2f;

    private bool stageScreenActive = false;
    private Coroutine stageRoutine;
    private Player player;
    private Spawner spawner;
    private float score;
    private float hiscore;

    private enum GameStage
    {
        Day,
        Sunset,
        Night,
        Twisted,
    }

    private GameStage currentStage;
    private float stageTimer = 0f;
    private float nextMessageTime = 0f;
    private float currentStageLength = 0f;

    private int dayIndex = 0;
    private int sunsetIndex = 0;
    private int nightIndex = 0;
    private int twistedIndex = 0;

    // You know your father would be so proud of you.
    // I can't believe they gave you a full ride to Prairie View.
    // I made you my special spaghetti to celebrate.
    // Get home soon sweetie.
    // Grace Marie Jones, you're out entirely too late.
    // Call me right now?
    // Grace you're scaring me. Where are you?
    // Is that you at the door?
    // Are you seeing all this red mist?
    // Girl! If you don't your ass home right now.

    private string[] dayLines =
    {
        "Mom: Grace, your brother came home with a hole in his backpack. He has dropped things all over the neighborhood. Can you please get them back on your way home?",
        "Mom: Be home before dark, okay?",
        "Mom: I love you. Stay on the main road.",
        "Mom: Text me when you're close.",
        "Mom: Your brother keeps asking when you'll be back.",
        "Mom: I made your favorite tonight.",
    };

    private string[] sunsetLines =
    {
        "Mom: Grace, the streetlights are coming on.",
        "Mom: Honey, it's getting late.",
        "Mom: Where are you? It's almost dark.",
        "Mom: Grace, please answer me.",
        "Mom: I'm starting to worry.",
        "Mom: You know the rule. Be home by dark.",
        "Mom: Grace? Did your phone die?",
    };

    private string[] nightLines =
    {
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

    private string[] twistedLinesPool =
    {
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
        player = FindObjectOfType<Player>();
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
        CancelInvoke(nameof(ShowGameOverScreen));
        CancelInvoke(nameof(HideNarrativeText));

        if (stageRoutine != null)
            StopCoroutine(stageRoutine);

        stageScreenActive = false;

        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        foreach (var obstacle in obstacles)
            Destroy(obstacle.gameObject);

        score = 0f;
        enabled = true;

        dayIndex = 0;
        sunsetIndex = 0;
        nightIndex = 0;
        twistedIndex = 0;

        activeTwistedLines = GetRandomSubset(twistedLinesPool, 8);

        player.gameObject.SetActive(true);
        spawner.gameObject.SetActive(false);

        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        narrativeText.gameObject.SetActive(false);

        if (stagePanel != null)
            stagePanel.SetActive(false);

        FindObjectOfType<DayNightCycle>()?.ResetCycle();

        RestartMusic();
        UpdateHiscore();

        stageRoutine = StartCoroutine(BeginStageRoutine(GameStage.Day, false));
    }

    private void StartStage(GameStage stage)
    {
        currentStage = stage;
        stageTimer = 0f;
        gameSpeed = initialGameSpeed;

        DayNightCycle dayNightCycle = FindObjectOfType<DayNightCycle>();

        switch (stage)
        {
            case GameStage.Day:
                currentStageLength = dayStageLength;
                nextMessageTime = GetMessageSpacing(dayStageLength, dayLines.Length);
                dayNightCycle?.SetDay();
                PlayStageMusic(dayMusic);
                break;

            case GameStage.Sunset:
                currentStageLength = sunsetStageLength;
                nextMessageTime = GetMessageSpacing(sunsetStageLength, sunsetLines.Length);
                dayNightCycle?.SetSunset();
                PlayStageMusic(sunsetMusic);
                break;

            case GameStage.Night:
                currentStageLength = nightStageLength;
                nextMessageTime = GetMessageSpacing(nightStageLength, nightLines.Length);
                dayNightCycle?.SetNight();
                PlayStageMusic(nightMusic);
                break;

            case GameStage.Twisted:
                currentStageLength = twistedStageLength;
                nextMessageTime = GetMessageSpacing(twistedStageLength, activeTwistedLines.Length);
                dayNightCycle?.SetTwisted();
                PlayStageMusic(twistedMusic);
                break;
        }
    }

    private float GetMessageSpacing(float stageLength, int messageCount)
    {
        if (messageCount <= 0)
            return stageLength;

        // This spaces messages across the stage with a little breathing room.
        return stageLength / (messageCount + 1);
    }

    public void GameOver()
    {
        if (stageRoutine != null)
            StopCoroutine(stageRoutine);

        stageScreenActive = false;

        if (stagePanel != null)
            stagePanel.SetActive(false);

        gameSpeed = 0f;
        enabled = false;

        if (spawner != null)
            spawner.gameObject.SetActive(false);

        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);

        StopMusic();
        UpdateHiscore();

        if (player != null)
        {
            player.PlayDeath();
        }

        Invoke(nameof(ShowGameOverScreen), gameOverDelay);
    }

    private void ShowGameOverScreen()
    {
        if (player != null)
            player.gameObject.SetActive(false);

        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (stageScreenActive)
            return;

        gameSpeed += gamesSpeedIncrease * Time.deltaTime;
        gameSpeed = Mathf.Min(gameSpeed, maxStageSpeed);

        score += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");

        UpdateStage();
    }

    private void UpdateStage()
    {
        stageTimer += Time.deltaTime;

        CheckNarrativeMilestones();

        if (stageTimer >= currentStageLength)
        {
            AdvanceStage();
        }
    }

    private void AdvanceStage()
    {
        if (stageScreenActive)
            return;

        switch (currentStage)
        {
            case GameStage.Day:
                stageRoutine = StartCoroutine(BeginStageRoutine(GameStage.Sunset, true));
                break;

            case GameStage.Sunset:
                stageRoutine = StartCoroutine(BeginStageRoutine(GameStage.Night, true));
                break;

            case GameStage.Night:
                stageRoutine = StartCoroutine(BeginStageRoutine(GameStage.Twisted, true));
                break;

            case GameStage.Twisted:
                // Stay in Twisted forever after this.
                currentStageLength = Mathf.Infinity;
                break;
        }
    }

    private IEnumerator BeginStageRoutine(GameStage nextStage, bool showEndScreen)
    {
        stageScreenActive = true;
        gameSpeed = 0f;

        if (spawner != null)
            spawner.gameObject.SetActive(false);

        ClearObstacles();

        if (showEndScreen)
        {
            ShowStageScreen(GetStageCompleteText(currentStage), GetStageClearImage(currentStage));

            yield return new WaitForSecondsRealtime(stageEndScreenDuration);
        }

        StartStage(nextStage);

        ShowStageScreen(GetStageStartText(nextStage), null);
        yield return new WaitForSecondsRealtime(stageStartScreenDuration);

        HideStageScreen();

        if (spawner != null)
            spawner.gameObject.SetActive(true);

        stageScreenActive = false;
    }

    private void CheckNarrativeMilestones()
    {
        if (stageTimer < nextMessageTime)
            return;

        string line = null;

        switch (currentStage)
        {
            case GameStage.Day:
                if (dayIndex < dayLines.Length)
                {
                    line = dayLines[dayIndex++];
                    nextMessageTime =
                        GetMessageSpacing(dayStageLength, dayLines.Length) * (dayIndex + 1);
                }
                break;

            case GameStage.Sunset:
                if (sunsetIndex < sunsetLines.Length)
                {
                    line = sunsetLines[sunsetIndex++];
                    nextMessageTime =
                        GetMessageSpacing(sunsetStageLength, sunsetLines.Length)
                        * (sunsetIndex + 1);
                }
                break;

            case GameStage.Night:
                if (nightIndex < nightLines.Length)
                {
                    line = nightLines[nightIndex++];
                    nextMessageTime =
                        GetMessageSpacing(nightStageLength, nightLines.Length) * (nightIndex + 1);
                }
                break;

            case GameStage.Twisted:
                if (twistedIndex < activeTwistedLines.Length)
                {
                    line = activeTwistedLines[twistedIndex++];
                    nextMessageTime =
                        GetMessageSpacing(twistedStageLength, activeTwistedLines.Length)
                        * (twistedIndex + 1);
                }
                break;
        }

        if (line != null)
            ShowNarrativeText(line);
    }

    private void ShowStageScreen(string text, Sprite image = null)
    {
        if (stageText != null)
            stageText.text = text;

        if (stageClearImage != null)
        {
            stageClearImage.sprite = image;
            stageClearImage.gameObject.SetActive(image != null);
        }

        if (stagePanel != null)
            stagePanel.SetActive(true);
    }

    private void HideStageScreen()
    {
        if (stageClearImage != null)
        {
            stageClearImage.sprite = null;
            stageClearImage.gameObject.SetActive(false);
        }

        if (stagePanel != null)
            stagePanel.SetActive(false);
    }

    private void ClearObstacles()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();

        foreach (var obstacle in obstacles)
            Destroy(obstacle.gameObject);
    }

    private string GetStageCompleteText(GameStage stage)
    {
        switch (stage)
        {
            case GameStage.Day:
                return "DAY COMPLETE";

            case GameStage.Sunset:
                return "SUNSET COMPLETE";

            case GameStage.Night:
                return "NIGHT COMPLETE";

            case GameStage.Twisted:
                return "TWISTED COMPLETE";

            default:
                return "STAGE COMPLETE";
        }
    }

    private string GetStageStartText(GameStage stage)
    {
        switch (stage)
        {
            case GameStage.Day:
                return "DAY\n\nGet home before dark.";

            case GameStage.Sunset:
                return "SUNSET\n\nThe streetlights are coming on.";

            case GameStage.Night:
                return "NIGHT\n\nStay in the light.";

            case GameStage.Twisted:
                return "TWISTED\n\nDo not look back.";

            default:
                return "STAGE START";
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
        if (titleMusicSource != null)
            titleMusicSource.Stop();
    }

    private void PlayStageMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.time = 0f;
        musicSource.pitch = 1f;
        musicSource.Play();
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
