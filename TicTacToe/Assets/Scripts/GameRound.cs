using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRound : MonoBehaviour
{
    Text myText;
    int Round;
    void Start()
    {
        myText = GetComponent<Text>();
    }

    // Auto update round number when game number in EmulateGrab change
    void Update()
    {
        Round = GameObject.Find("Hand").GetComponent<EmulateGrab>().GameRound;
        if (Round == 10)
        {
            myText.text = "Round_" + Round.ToString();
        }
        else
        {
            myText.text = "Round_0" + Round.ToString();
        }

    }
}
