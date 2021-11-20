using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRound : MonoBehaviour
{
    Text myText;
    int Round;
    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<Text>();
    }

    // Update is called once per frame
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
