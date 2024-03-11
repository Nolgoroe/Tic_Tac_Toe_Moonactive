using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModel : MonoBehaviour //remove mono here // temp flag???
{
    //do we need a constructor here.


    public GameModeSO gameModeSO;
    public PlayerBase[] players;


    public Image tempPreafb;
    public Transform tempParent;

    //I was thinking of removing the monobehavior here and make this a stricly data script.
    //I changed my mind since this script being a monobehavior allows me for easier debugging and more rapid word in the unity inspscor.
    public void InitGameModel(GameModeSO _gameModeSO, PlayerBase[] _players)
    {
        gameModeSO = _gameModeSO;
        players = _players;
    }

    public void Test()
    {
        foreach (var item in players)
        {
            Debug.Log(item.publicPlyerData.playerName);

            //Image testImage = Instantiate(tempPreafb, tempParent);
            //testImage.sprite = item.publicPlyerData.playerIconSprite;

        }

        Debug.Log("Bla");
    }
}
