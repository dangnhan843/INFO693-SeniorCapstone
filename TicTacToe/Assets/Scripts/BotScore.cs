using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotScore : MonoBehaviour
{
    Text myText;
    int Score;
    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<Text>();
    }

    // Auto update bot score when BotScore in EmulateGrab change
    void Update()
    {
        Score = GameObject.Find("Hand").GetComponent<EmulateGrab>().BotScore;
        if (Score == 10)
        {
            myText.text = Score.ToString();
        }
        else
        {
            myText.text = "0" + Score.ToString();
        }

    }
}
