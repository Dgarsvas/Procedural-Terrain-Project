using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public ButtonSocketHandler webSocket;

    public List<FancyButton> buttons;

    public ObjectSpawner spawner;

    public bool canEvaluate = false;

    private List<Color> currentColors;

    public int buttonPressCount = 0;

    public int rounds = 1;

    public GameObject textObject;

    public GameObject _startText;

    public GameObject _startButton;

    public Text _currentRound;

    public GameObject _spinningFrame;

    public float vignetteSpeed = 0.1f;

    public AudioClip goodBoop;

    public AudioClip badBeep;

    public AudioClip startSound;

    private Vignette vignette;

    private AudioSource audioSource;

    private bool nextButtonPressed;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        buttons = new List<FancyButton>();
        MockButtons();

        PostProcessVolume activeVolume = FindObjectOfType<PostProcessVolume>();
        if (activeVolume != null)
        {
            activeVolume.profile.TryGetSettings(out vignette);
        }
    }

    private void OnEnable()
    {
        ButtonSocketHandler.OnBDown += buttonPressedDown;
        ButtonSocketHandler.OnBUp += buttonPressedUp;
    }

    private void OnDisable()
    {
        ButtonSocketHandler.OnBDown -= buttonPressedDown;
        ButtonSocketHandler.OnBUp -= buttonPressedUp;
    }

    private void buttonPressedDown(AButton b)
    {
        if (canEvaluate)
        {
            ColorPressed(b.color1);
        }
    }

    private void buttonPressedUp(AButton b)
    {
        throw new NotImplementedException();
        //handle button release
    }



    private void MockButtons()
    {
        buttons.Add(new FancyButton(Color.red, KeyCode.R));
        buttons.Add(new FancyButton(Color.green, KeyCode.G));
        buttons.Add(new FancyButton(Color.blue, KeyCode.B));
    }


    #region mocked system
    void Update()
    {
        if (canEvaluate)
        {
            CheckForButtonPress();
        }
    }

    public void EvaluateButtonsByColors(List<Color> colors)
    {
        StartCoroutine(StartEvaluation(colors));
    }

    private void CheckForButtonPress()
    {
        foreach (FancyButton button in buttons)
        {
            if (Input.GetKeyDown(button.KeyCode))
            {
                ColorPressed(button.Color);
                break;
            }
        }
    }

    #endregion

    private void ColorPressed(Color color)
    {
        VisualizePressedButton(color);

        Debug.Log("Comparing object color of: " + currentColors[buttonPressCount] + " with Button pressed of color: " + color);
        if (currentColors[buttonPressCount] == color)
        {
            Debug.Log("Correct Button pressed!");
            buttonPressCount++;
            if (buttonPressCount == currentColors.Count)
            {
                StopEvaluation();
                StartCoroutine(StartNextWave());
            }
            audioSource.PlayOneShot(goodBoop);
        }
        else
        {
            Debug.Log("Incorrent Button pressed!");
            StopEvaluation();
            StartCoroutine(StopGame());
            audioSource.PlayOneShot(badBeep);
        }
    }

    private IEnumerator VignetteAnim()
    {
        nextButtonPressed = false;
        vignette.intensity.value = 0.7f;
        while (!nextButtonPressed || vignette.intensity.value <= 0)
        {
            vignette.intensity.value -= Time.deltaTime * vignetteSpeed;
            yield return new WaitForSeconds(0.05f);
        }
    }

   
    private void VisualizePressedButton(Color color)
    {
        if (vignette != null)
        {
            vignette.color.value = color;
            nextButtonPressed = true;
            StartCoroutine(VignetteAnim());
        }
    }


    #region game states
    private IEnumerator StartEvaluation(List<Color> colors)
    {
        currentColors = colors;
        buttonPressCount = 0;
        ShowMessage(true, "START PRESSING!");
        audioSource.PlayOneShot(startSound);
        canEvaluate = true;
        yield return new WaitForSeconds(1f);
        ShowMessage(false);

    }

    private IEnumerator StartNextWave()
    {
        rounds++;
        ShowMessage(true, "NICE");
        yield return new WaitForSeconds(1.4f);
        ShowMessage(true, "STARTING NEXT WAVE");
        yield return new WaitForSeconds(1.4f);
        ShowMessage(false);
        spawner.PrepareObjectSpawning(rounds);
        _currentRound.text = "ROUND " + rounds;
    }

    private IEnumerator StopGame()
    {
        ShowMessage(true, "TOO BAD");
        yield return new WaitForSeconds(1.4f);
        ShowMessage(true, "YOU LOST");
        yield return new WaitForSeconds(1.4f);
        ShowMessage(false);
        ShowStartMenu();
        _spinningFrame.SetActive(false);
        _currentRound.gameObject.SetActive(false);
        rounds = 1;
    }

    private void ShowStartMenu()
    {
        _startText.SetActive(true);
        _startButton.SetActive(true);
    }

    private void ShowMessage(bool show, string text = "")
    {
        textObject.SetActive(show);
        textObject.GetComponent<Text>().text = text;
    }

    private void StopEvaluation()
    {
        canEvaluate = false;
        buttonPressCount = 0;
    }

    #endregion
}



public class FancyButton
{
    public Color Color { get; set; }
    public KeyCode KeyCode { get; set; }

    public FancyButton(Color color, KeyCode buttonReference)
    {
        Color = color;
        KeyCode = buttonReference;
    }
}