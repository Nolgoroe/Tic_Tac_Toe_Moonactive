using UnityEngine;

[CreateAssetMenu(fileName = "GameMode", menuName = "ScriptableObjects/Game Modes")]
public class GameModeSO : ScriptableObject
{
    [SerializeField] int requiredComboToWin = 3;
    [SerializeField] int width = 3;
    [SerializeField] int height = 3;
    [SerializeField] float timeDelayAITurn = 1;
    [SerializeField] bool allowUndo = false;
    [SerializeField] PlayerData[] modePlayers;

    public PlayerData[] modePublicModePlayers => modePlayers;
    public Vector2 modeBoardWidthAndHeight => new Vector2(width, height);
    public bool modeAllowUndo => allowUndo;
    public int modeRequiredComboToWin => requiredComboToWin;
    public float modeTimeDelayAITurn => timeDelayAITurn;

}
