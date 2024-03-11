using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameMode", menuName = "ScriptableObjects/Game Modes")]
public class GameModeSO : ScriptableObject
{
    [SerializeField] PlayerData[] modePlayers;

    public PlayerData[] publicModePlayers => modePlayers;

}
