using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int whoTurn; // 0 =x , 1 = o
    public int turnCount; // counts the number of turn
    public GameObject[] turnIcons; // displays whos turn
    public Sprite[] playIcons; // 0 = x, 1 = y icon
    public Button[] tictactorSpaces; // playable space

    // Start is called before the first frame update
    void Start()
    {
        GameSetup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameSetup()
    {
        whoTurn = 0;
        turnCount = 0;
        turnIcons[0].SetActive(true);
        turnIcons[1].SetActive(false);
        for (int i = 0; i< tictactorSpaces.Length; i++)
        {
            tictactorSpaces[i].interactable = true;
            tictactorSpaces[i].GetComponent<Image>().sprite = null;
        }
    }
}
