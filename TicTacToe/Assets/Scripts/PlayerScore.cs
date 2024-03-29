﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    Text myText;
    int Score;
    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<Text>();
    }

    // Auto update player score when PlayerScore in EmulateGrab change
    void Update()
    {
        Score = GameObject.Find("Hand").GetComponent<EmulateGrab>().PlayerScore;
        if (Score == 10)
        {
            myText.text = Score.ToString();
        }
        else
        {
            myText.text = "0"+Score.ToString();
        }
        
    }
}
