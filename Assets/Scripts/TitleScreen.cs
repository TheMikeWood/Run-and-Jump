using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [Header("Title Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI taglineText;
    public TextMeshProUGUI pressAnyKeyText;

    [Header("Title Screen Objects")]
    public GameObject titleObject;
    public GameObject taglineObject;
    public GameObject startObject;
    public GameObject optionsButtonObject;

    [Header("Options Menu")]
    public GameObject optionsPanel;
    public Slider masterVolumeSlider;

    [Header("Sub Menus")]
    public GameObject controlsPanel;
    public GameObject creditsPanel;

    private bool starting = false;
    private bool optionsOpen = false;

    private const string MasterVolumeKey = "MasterVolume";

    private void Start()
    {
        titleText.text = "Streetlights";
        taglineText.text = "Get home before dark.";
        pressAnyKeyText.text = "Press any key to start";

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        float savedVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        AudioListener.volume = savedVolume;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = savedVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
    }

    private void Update()
    {
        if (starting) return;
        if (optionsOpen) return;

        // ignore mouse clicks, only keyboard/gamepad.
        if (Input.anyKeyDown &&
            !Input.GetMouseButtonDown(0) &&
            !Input.GetMouseButtonDown(1))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (starting) return;

        starting = true;
        gameObject.SetActive(false);
        GameManager.Instance.NewGame();
    }

    public void ShowOptions()
    {
        optionsOpen = true;

        if (titleObject != null)
            titleObject.SetActive(false);

        if (taglineObject != null)
            taglineObject.SetActive(false);

        if (startObject != null)
            startObject.SetActive(false);

        if (optionsButtonObject != null)
            optionsButtonObject.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void HideOptions()
    {
        optionsOpen = false;

        if (titleObject != null)
            titleObject.SetActive(true);

        if (taglineObject != null)
            taglineObject.SetActive(true);

        if (startObject != null)
            startObject.SetActive(true);

        if (optionsButtonObject != null)
            optionsButtonObject.SetActive(true);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void ShowControls()
    {
        optionsOpen = true;

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        optionsOpen = true;

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void BackToOptions()
    {
        optionsOpen = true;

        if (optionsPanel != null)
            optionsPanel.SetActive(true);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(MasterVolumeKey, volume);
        PlayerPrefs.Save();
    }
}