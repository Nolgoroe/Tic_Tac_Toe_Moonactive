using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditorInternal.VersionControl;
using TMPro;


public class GameView : MonoBehaviour
{

    [Header("Main Menu")]
    [SerializeField] RectTransform mainMenuScreen;
    [SerializeField] RectTransform mainMenuParentObject;
    [SerializeField] float mainMenuTweenSpeed;
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

    private void Awake()
    {
        ToggleScreen(true, mainMenuScreen);
        ToggleScreen(false, endScreenCanvas);

        //I know this is generally frowned upon.. but it is called once on awake for the rest of the game.
        // the memory and CPU footprint of this is negligible in my opinion.
        Button[] allSceneButtons = FindObjectsOfType<Button>();
        foreach (Button button in allSceneButtons)
        {
            button.onClick.AddListener(() => SoundManager.Instance.PlaySoundOneShot(Sounds.ButtonClick));
        }

    }

    private void Start()
    {
        SoundManager.Instance.PlaySoundFade(Sounds.MenuMusic);
    }



    #region Public Actions
    public void InitGameView()
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
            case EndConditions.Win:
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
}
