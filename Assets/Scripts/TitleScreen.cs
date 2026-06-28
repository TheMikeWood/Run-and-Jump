using UnityEngine;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    [Header("Title Text")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI taglineText;

    [Header("Title Screen Objects")]
    public GameObject titleObject;
    public GameObject taglineObject;
    public GameObject startButtonObject;
    public GameObject optionsButtonObject;

    [Header("Options Menu")]
    public GameObject optionsPanel;

    [Header("Sub Menus")]
    public GameObject controlsPanel;
    public GameObject creditsPanel;

    private bool starting = false;

    private void Start()
    {
        titleText.text = "Streetlights";
        taglineText.text = "Get home before dark.";

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
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

        if (titleObject != null)
            titleObject.SetActive(false);

        if (taglineObject != null)
            taglineObject.SetActive(false);

        if (startButtonObject != null)
            startButtonObject.SetActive(false);

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

        if (titleObject != null)
            titleObject.SetActive(true);

        if (taglineObject != null)
            taglineObject.SetActive(true);

        if (startButtonObject != null)
            startButtonObject.SetActive(true);

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

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    public void BackToOptions()
    {

        if (optionsPanel != null)
            optionsPanel.SetActive(true);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }
}