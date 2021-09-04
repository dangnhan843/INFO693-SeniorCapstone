using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool playerTurn = true;
    public bool gameEnded = false;
    public bool playerWon = false;
    public bool drawGame = false;
    public GameObject[] AnswerList;

    public GameObject[] Os;
    public GameObject[] Xs;

    // Start is called before the first frame update
    void Start()
    {
        GameSetup();
        /*
        foreach (GameObject O in Os)
        {
            O.SetActive(false);
        }
        foreach (GameObject X in Xs)
        {
            X.SetActive(false);
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            foreach (GameObject X in Xs)
            {
                X.SetActive(false);
            }
            foreach (GameObject O in Os)
            {
                O.SetActive(false);
            }
        }

            /*
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (playerTurn)
                {
                    playerTurn = false;
                    foreach (GameObject X in Xs)
                    {
                        X.SetActive(false);
                    }
                    foreach (GameObject O in Os)
                    {
                        O.SetActive(true);
                    }
                }
                else if (!playerTurn)
                {
                    playerTurn = true;
                    foreach (GameObject X in Xs)
                    {
                        X.SetActive(true);
                    }
                    foreach (GameObject O in Os)
                    {
                        O.SetActive(false);
                    }
                }
                Debug.Log(playerTurn);

            }
            */
        }

    void GameSetup()
    {

        /*
         * turnIcons[0].SetActive(true);
        turnIcons[1].SetActive(false);
        for (int i = 0; i< tictactorSpaces.Length; i++)
        {
            tictactorSpaces[i].interactable = true;
            tictactorSpaces[i].GetComponent<Image>().sprite = null;
        }
        */
    }
}
