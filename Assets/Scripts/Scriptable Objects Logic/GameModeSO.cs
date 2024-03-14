using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameMode", menuName = "ScriptableObjects/Game Modes")]
public class GameModeSO : ScriptableObject
{
    [SerializeField] int requiredComboToWin = 3;
    [SerializeField] int width = 3;
    [SerializeField] int height = 3;
    [SerializeField] bool allowUndo = false;
    [SerializeField] PlayerData[] modePlayers;

    public PlayerData[] publicModePlayers => modePlayers;
    public Vector2 boardWidthAndHeight => new Vector2(width, height);
    public bool modelAllowUndo => allowUndo;
    public int modelRequiredComboToWin => requiredComboToWin;

}
