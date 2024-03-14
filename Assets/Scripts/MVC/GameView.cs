using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;


public class GameView : MonoBehaviour
{
    #region hidden data
    [HideInInspector]
    [SerializeField] GameController gameController;
    #endregion

    [Header("Main Menu")]
    [SerializeField] RectTransform mainMenuScreen;
    [SerializeField] RectTransform mainMenuParentObject;
    [SerializeField] float mainMenuTweenSpeed;

    [Header("AI Difficulties")]
    [SerializeField] Button[] difficultyButtons;

    [Header("Undo System")]
    [SerializeField] RectTransform pvcButtonsParent;

    [Header("End Screen")]
    [SerializeField] RectTransform endScreenCanvas;
    [SerializeField] RectTransform endScreenPanel;
    [SerializeField] TMP_Text endScreenText;
    [SerializeField] float endScreenTextTweenSpeed;

    [Header("In Game")]
    [SerializeField] TMP_Text currentPlayerName;
    [SerializeField] TMP_Text timerText;

    [Header("Settings")]
    [SerializeField] RectTransform settingsScreen;
    [SerializeField] Slider volumeControlSlider;
    [SerializeField] TMP_InputField turnTimeInput;
    [SerializeField] TMP_Text systemMessages;

    private void Awake()
    {
        //I know this is generally frowned upon.. but it is called once on awake for the rest of the game.
        // the memory and CPU footprint of this is negligible in my opinion.
        Button[] allSceneButtons = FindObjectsOfType<Button>();
        foreach (Button button in allSceneButtons)
        {
            button.onClick.AddListener(() => SoundManager.Instance.PlaySoundOneShot(Sounds.ButtonClick));
        }

        ToggleScreen(true, mainMenuScreen);
        ToggleScreen(false, endScreenCanvas);
        ToggleScreen(false, settingsScreen);
    }

    private void Start()
    {
        SoundManager.Instance.PlaySoundFade(Sounds.MenuMusic);

        SetDifficultyButtonsCallbacks();
    }



    #region Public Actions
    public void InitGameView(GameController _gameController)
    {
        gameController = _gameController;
    }
    public void GameViewStartup()
    {
        LeanTween.moveX(mainMenuParentObject, Screen.width, mainMenuTweenSpeed).setEaseInCirc().
            setOnComplete(() => ToggleScreen(false, mainMenuScreen));

        SoundManager.Instance.StopSoundFade(Sounds.MenuMusic);
        SoundManager.Instance.PlaySoundFade(Sounds.GameMusic);
    }
    public void UpdatePlayerView(PlayerBase _currentPlayer)
    {
        currentPlayerName.text = _currentPlayer.publicPlyerData.playerName;
    }

    public void AnimateCellMark(Cell cell)
    {
        cell.AnimateMark();
    }

    #endregion


    #region Screen Management
    public void TogglePVCButtons(bool enable)
    {
        pvcButtonsParent.gameObject.SetActive(enable);
    }
    private void ToggleScreen(bool _isOn, RectTransform screen)
    {
        screen.gameObject.SetActive(_isOn);
    }
    public void SetEndScreenText(EndConditions endCondition, PlayerBase player)
    {
        //I know there are some better ways to manage strings - is this enough?? temp flag.
        string text = "";

        switch (endCondition)
        {
            case EndConditions.End:
                text = player.publicPlyerData.playerName + " " + "Wins!";
                SoundManager.Instance.PlaySoundOneShot(Sounds.Win);
                break;
            case EndConditions.Draw:
                text = "DRAW";
                break;
            case EndConditions.Timeout:
                text = player.publicPlyerData.playerName + " " + "Timeout... :(";
                SoundManager.Instance.PlaySoundOneShot(Sounds.Timeout);
                break;
            case EndConditions.None:
                break;
            default:
                break;
        }

        endScreenText.text = text;

        ToggleScreen(true, endScreenCanvas);

        LeanTween.move(endScreenPanel, Vector3.zero, endScreenTextTweenSpeed).setEaseOutBounce();

    }
    public void UpdateTurnTimer(float time)
    {
        timerText.text = "Time: " + Mathf.Ceil(time).ToString();
    }

    #endregion


    #region Button Related
    public void SetDifficultyButtonsCallbacks()
    {
        //connected To Buttons
        for (int i = 0; i < System.Enum.GetValues(typeof(AILevel)).Length; i++)
        {
            int index = i;

            difficultyButtons[index].onClick.AddListener(() => gameController.SetAILevel((AILevel)index));
        }
    }

    public void OnChangeSoundModifier()
    {
        SoundManager.Instance.OnChangeSoundModifier(volumeControlSlider.value);
    }
    public void ToggleSettings()
    {
        if(settingsScreen.gameObject.activeInHierarchy)
        {
            settingsScreen.gameObject.SetActive(false);
        }
        else
        {
            settingsScreen.gameObject.SetActive(true);
        }
    }

    public void SetControllerTimeForTurn()
    {
        if(int.TryParse(turnTimeInput.text, out int num) && num != 0)
        {
            gameController.SetTimeForTurn(num);
        }
        else
        {
            StartCoroutine(DisplaySettingsSystemMessage("Error in input field - must be a number larger than 0"));
        }
    }

    #endregion


    private IEnumerator DisplaySettingsSystemMessage(string message)
    {
        systemMessages.text = message;
        yield return new WaitForSeconds(3);
        systemMessages.text = " ";
    }
}
