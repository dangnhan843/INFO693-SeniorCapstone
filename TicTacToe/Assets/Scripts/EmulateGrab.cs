﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//********************************************************************************//
//******THIS SCRIPT EMULATES CONTROLLER ROTATION SIMILAR TO A VR CONTROLLER*******//
//********************************************************************************//

//Press the 'C' key (C for Controller) while the Game View is active and rotate the Controller with the left mouse button
//Grab objects by pointing and clicking down the left mouse button
//Scroll up and down the mouse wheel to move the grabbed object in z-axis (forward/backward)

public class EmulateGrab : MonoBehaviour
{
    // default emulateGrad variable
    float controllerSpeedHorizontal = 1.5f;
    float controllerSpeedVertical = 1.5f;
    float controllerYaw = 0.0f;
    float controllerPitch = 0.0f;
    float delayTime = 1.0f;
    private bool delayTimeCounter = false;

    string tileName ; // tile name for specific tile animation
    int tileNumberX ; // the chosen tile number

    private bool isGrabbing = false;             //A control variable that stores if the user is grabbing anything
    private Transform grabbedTransform;          //A variable to hold the grabbed object's transform
    private Transform clickTransform;
    public float zSpeed = 4.5f;                  //A variable to control the speed of movement in z-axis (forward/backward)
    public float rotationSpeedMultiplier = 2.0f; //A variable to increase the rotational speed for the controller
    private Transform hitTransform;              //A variable to hold the hit object's transform
    public int PlayerScore = 0;                  // Player Score
    public int BotScore = 0;                     // Bot score
    public int GameRound = 0;                    // Current round number
    public bool playerFirst = true;              // Player start First
    public AudioSource TTTSound;                 // sound for tile animation

    // boolean value for game condition
    bool playerTurn = true; 
    bool isGameEnd = false;
    bool botWin = false;
    bool playerWin = false;
    bool isDraw = false;
    bool BotTurn = false;
    bool PlayerTurnFirst = false;
    bool delayPlayerTurn = false;
    bool hardMode = true;

    float delayBotMoveTime = 1.1f;              // Delay bot tile due to bot animation
    float delayResetTime = 3.0f;                // Delay for reset game
    bool delayBotTime = false;
    private int tempMovePosition =10;

    public GameObject[] Os;                     // Bot tile holder
    public GameObject[] Xs;                     // Player tile holder
    public int[] playSpace;                     // available playing tile
    public GameObject[] TTTs;                   // list of tile for tile animation and selection
    public GameObject[] BotWinTile;             // list of tile for bot win animation tile
    public GameObject[] PlayerWinTile;          // list of tile for player win animation tile

    public Animator animFromBot;                // Animation for bot


    void Start()
    {
        // reset game at start
        DoDelayReset(0.0f);

        // Bot animation
        animFromBot = GameObject.FindGameObjectWithTag("Bot").GetComponent<Animator>();

        // find and set default color for all title
        TTTs = GameObject.FindGameObjectsWithTag("TTTSpace");
        foreach (GameObject TTT in TTTs)
        {
            TTT.transform.GetComponent<SpriteRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }

        // find and set default color for bot winning title
        BotWinTile = GameObject.FindGameObjectsWithTag("BotWinTile");
        foreach (GameObject tile in BotWinTile)
        {
            tile.transform.GetComponent<SpriteRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }

        // find and set default color for player winning title
        PlayerWinTile = GameObject.FindGameObjectsWithTag("PlayerWinTile");
        foreach (GameObject tile in PlayerWinTile)
        {
            tile.transform.GetComponent<SpriteRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }

    }

    void Update()
    {

        // Controller 
        if (Input.GetKey(KeyCode.C)) //When the C key is pressed, we'll rotate the controller with the mouse movements
        {
            controllerYaw += controllerSpeedHorizontal * Input.GetAxis("Mouse X") * rotationSpeedMultiplier;
            controllerPitch += controllerSpeedVertical * Input.GetAxis("Mouse Y") * -rotationSpeedMultiplier;
            transform.localRotation = Quaternion.Euler(controllerPitch, controllerYaw, 0.0f);
            
        }

        //We are casting a ray from the controller to check for hits with playing tiles
        RaycastHit hitInfo2;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo2))
        {
            if (hitInfo2.transform.tag == "TTTSpace")
            {
                if (hitTransform != null)
                    hitTransform.GetComponent<SpriteRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                hitTransform = hitInfo2.transform;
                // if hit, change color of the tile to light green
                hitTransform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }
            else
            {
                // change color of the tile back to normal when not hit
                hitTransform.GetComponent<SpriteRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }

        }

        else //If we are not hitting any object
        {
            if (hitTransform != null && !isGrabbing)
            {
                SetHighlight(hitTransform, false); //We dim the previously pointed object
            }
        }
        

        // If it is player turn
        if (playerTurn == true && BotTurn == false && isGameEnd == false)
        {
            if (delayPlayerTurn == false)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0)) //If we are pressing down the left mouse button
                {
                    delayPlayerTurn = true;
                    DoDelayPlayerTurn(3.5f);
                    RaycastHit hitInfo;
                    if (delayTimeCounter == false)
                    {
                        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo))
                        {

                            tileName = CheckHitName(hitInfo);
                            tileNumberX = CheckHitNumber(hitInfo);
                            if (hitInfo.transform.tag == "TTTSpace" && playSpace[tileNumberX - 1] == 0) 
                            {
                                playSpace[tileNumberX - 1] = 1;         // confirm player tile is 1 in playable tile list
                                Xs[tileNumberX - 1].SetActive(true);    // draw X to the tile
                                TTTSound.Play();                        // player sound effect
                                winConditionChecking();                 // check for winning condition
                                DoDelayBotThinking(1.5f);               // in care not winning, delay for bot animation
                            }
                        }
                    }
                }
            }
        }

        // flipping between hard mode and easy mode by pressing H
        if (Input.GetKeyDown(KeyCode.H))
        {
            switch (hardMode)
            {
                case true:
                    Debug.Log("Hard mode: off");
                    hardMode = false;
                    break;
                case false:
                    Debug.Log("Hard mode: on");
                    hardMode = true;
                    break;
            }
        }

        // bot AI using minimax algorithm
        //      Minimax using recursion function, allow bot to choose random playable tiles
        //      and simulate player random player tiles.  The return value will be the tile
        //      win highest value (bot win).

        // when hard mode is on, bot uses minimax to choose the best tile
        if (hardMode == true)
        {
            // if it is bot turn
            if (playerTurn == false && BotTurn == true && isGameEnd == false)
            {
                bool tempGameEnd = checkEndMove(playSpace); // if game is still playable
                BotTurn = true;
                if (delayBotTime == false)
                {
                    if (!tempGameEnd)
                    {
                        float bestScore = -Mathf.Infinity;    
                        int bestMove = 0;
                        for (int i = 0; i < playSpace.Length; i++)
                        {
                            if (playSpace[i] == 0)
                            {
                                playSpace[i] = 2;
                                float tempScore = Minimax(playSpace, 0, false);
                                playSpace[i] = 0;
                                if (tempScore > bestScore)
                                {
                                    bestScore = tempScore;
                                    bestMove = i;
                                }
                            }
                        }
                        int MovePosition = (int)bestMove;
                        tempMovePosition = MovePosition;
                        delayBotTime = true;
                        BotAnimation(MovePosition);
                        DoDelayBotMove(MovePosition, delayBotMoveTime);
                        BotTurn = false;
                    }
                }
                playerTurn = true;
            }
        }
        else // hard mode is off, bot chooses random tile
        {
            if (playerTurn == false && BotTurn == true && isGameEnd == false)
            {
                bool tempGameEnd = checkEndMove(playSpace);
                BotTurn = true;
                if (delayBotTime == false)
                {
                    if (!tempGameEnd)
                    {
                        while (BotTurn)
                        {
                            int movePosition = Random.Range(0, 8);

                            if (movePosition != (tileNumberX - 1))
                            {
                                if (playSpace[movePosition] == 0)
                                {
                                    tempMovePosition = movePosition;
                                    delayBotTime = true;
                                    BotAnimation(movePosition);
                                    DoDelayBotMove(movePosition, delayBotMoveTime);
                                    BotTurn = false;
                                }
                            }

                        }
                    }
                }
                playerTurn = true;
            }
        }



        // Default EmulateGrab Script for grabbing

        //If the left mouse button is released, we'll revert the changes to release the object
        if (isGrabbing && Input.GetKeyUp(KeyCode.Mouse0)) 
        {
            if (grabbedTransform != null)
            {
                //We are setting isKinematic as false for the object to be controlled via physics
                grabbedTransform.GetComponent<Rigidbody>().isKinematic = false;
                //We are setting useGravity as true for the object to be controlled via physics
                grabbedTransform.GetComponent<Rigidbody>().useGravity = true;
                //We are breaking the hierarchy so that the previously grabbed object has no parent and can act independently
                grabbedTransform.parent = null; 
            }
            isGrabbing = false; //We turn the control variable to false indicating that we are not grabbing anything
        }

        //We'll take care of moving the grabbed object in z-axis here
        if (isGrabbing)
        {
            float distance = Input.mouseScrollDelta.y; //We are storing mouse scroll delta in y-axis

            //We are moving the grabbed object in z-axis forward/backward
            grabbedTransform.position += distance * zSpeed * Time.deltaTime * transform.forward;

            //We want to restrict the movement between 0.4 and 7.0, hence we are clamping the z of the localPosition
            grabbedTransform.localPosition = new Vector3(grabbedTransform.localPosition.x, grabbedTransform.localPosition.y,
                                                         Mathf.Clamp(grabbedTransform.localPosition.z, 0.4f, 7.0f));
        }
    }

    //This method sets/dims highlight through the use of the Outline and IsHit_S scripts
    //(attached to grabbable objects in the scene)
    void SetHighlight(Transform t, bool highlight)
    {
        if (highlight)
        {
            //We are setting the material color of the highlighted object as cyan
            t.GetComponent<Renderer>().material.color = Color.cyan;
            //We are setting the outline mode as OutlineAll
            //t.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
            //We are setting the color of the linerenderer as fully opaque
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); 
        }
        else
        {
            //We are reverting the material color of the highlighted object to the original color
            t.GetComponent<Renderer>().material.color = t.GetComponent<IsHit_S>().originalColorVar;
            //We are setting the outline mode as OutlineHidden
            //t.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
            //We are setting the material color of the linerenderer as half transparent
            transform.GetComponent<LineRenderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 0.5f); 
        }
    }


    // function check for tile name when raycast hit
    string CheckHitName(RaycastHit hitInfo2)
    {
        string name = "";
        switch (hitInfo2.collider.name)
        {
            case "1":
                name = "X1";
                break;
            case "2":
                name = "X2";
                break;
            case "3":
                name = "X3";
                break;
            case "4":
                name = "X4";
                break;
            case "5":
                name = "X5";
                break;
            case "6":
                name = "X6";
                break;
            case "7":
                name = "X7";

                break;
            case "8":
                name = "X8";
                break;
            case "9":
                name = "X9";
                break;
        }
        return name;
    }

    // function check for tile number when raycast hit
    int CheckHitNumber(RaycastHit hitInfo2)
    {
        int number = 0;
        switch (hitInfo2.collider.name)
        {
            case "1":
                number = 1;
                break;
            case "2":
                number = 2;
                break;
            case "3":
                number = 3;
                break;
            case "4":
                number = 4;
                break;
            case "5":
                number = 5;
                break;
            case "6":
                number = 6;
                break;
            case "7":
                number = 7;
                break;
            case "8":
                number = 8;
                break;
            case "9":
                number = 9;
                break;
        }
        return number;
    } 

    // function check if there is no more playable tile (for draw case)
    bool checkEndMove(int[] spaceToPlay)
    {
        foreach (int i in spaceToPlay)
        {
            if ( i ==0 )
            {
                return false;
            }
        }
        return true;
    }

    // fucntion for checking winner
    void winConditionChecking()
    {
        string winner = GameWinner();
        GameWinnerEffect();             // start game winner Effect

        // if player win, add player score
        if (winner == "player")
        {
            isGameEnd = true;
            playerWin = true;
            PlayerScore++;
            DoDelayReset(delayResetTime);

        }

        // if bot win, add bot score
        if (winner == "bot")
        {
            isGameEnd = true;
            botWin = true;
            BotScore++;
            DoDelayReset(delayResetTime);
        }

        bool IsNotPlayble = PlayableSpace();
        // if draw, normally reset game
        if (IsNotPlayble == false && playerWin == false && botWin == false
            )
        {
            Debug.Log("Draw");
            
            isGameEnd = true;
            isDraw = true;
            DoDelayReset(delayResetTime);
        }
        
        // condition for who start the game first
        if (playerFirst == false)
        {
            playerFirst = true;
        }
        
    }

    // Delay for player turn while bot is moving
    void DoDelayPlayerTurn(float delayTime)
    {
        StartCoroutine(DelayPlayerTurn(delayTime));
    }

    IEnumerator DelayPlayerTurn(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        delayPlayerTurn = false;
    }

    // Delay for bot thinking time
    void DoDelayBotThinking(float delayTime)
    {
        StartCoroutine(DelayBotThinking(delayTime));
    }

    IEnumerator DelayBotThinking(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        playerTurn = false;
        BotTurn = true;
    }

    // delay for reset game after finish one round
    void DoDelayReset(float delayTime)
    {
        StartCoroutine(resetGame(delayTime));
    }
    IEnumerator resetGame(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        //turnCount = 0;
        
        foreach (GameObject X in Xs)
        {
            X.SetActive(false);
        }
        foreach (GameObject O in Os)
        {
            O.SetActive(false);
        }

        foreach (GameObject tile in BotWinTile)
        {
            tile.transform.GetComponent<SpriteRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }

        foreach (GameObject tile in PlayerWinTile)
        {
            tile.transform.GetComponent<SpriteRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }

        for (int i = 0; i < playSpace.Length; i++)
        {
            playSpace[i] = 0;
        }
        
        if (playerFirst == true)
        {
            playerFirst = false;
        }
        else
        {
            playerFirst = true;
        }
        botResetAnimation();

        if (PlayerTurnFirst == true)
        {
            BotTurn = true;
            playerTurn = false;
        }
        else
        {
            BotTurn = false;
            playerTurn = true;
        }
        switch (PlayerTurnFirst)
        {
            case true:
                PlayerTurnFirst = false;
                break;
            case false:
                PlayerTurnFirst = true;
                break;
        }
        if (GameRound == 10)
        {
            GameRound = 0;
            PlayerScore = 0;
            BotScore = 0;
        }
        GameRound++;
        playerWin = false;
        botWin = false;
        isDraw = false;
        isGameEnd = false;
        delayTimeCounter = false;
        tempMovePosition = 10;

    }
    
    // Delay for bot animation
    void DoDelayBotMove(int movePosition, float delayTime)
    {
        StartCoroutine(delayBotMove(movePosition,delayTime));
    }

    IEnumerator delayBotMove(int movePosition,float delayTime)
    {
        //Wait for the specified delay time before continuing.
        
        yield return new WaitForSeconds(delayTime);
        if (tempMovePosition != 10)
        {
            Os[movePosition].SetActive(true);
            TTTSound.Play();
            playSpace[movePosition] = 2;
            delayBotTime = false;
            botResetAnimation();
            winConditionChecking();
        }
    }

    void BotAnimation(int movePosition)
    {
        //Debug.Log(movePosition);
        switch (movePosition)
        {
            case 0:
                //Debug.Log("Move1");
                animFromBot.SetBool("Move1", true);
                break;
            case 1:
                //Debug.Log("Move2");
                animFromBot.SetBool("Move2", true);
                break;
            case 2:
                //Debug.Log("Move3");
                animFromBot.SetBool("Move3", true);
                break;
            case 3:
                //Debug.Log("Move4");
                animFromBot.SetBool("Move4", true);
                break;
            case 4:
                //Debug.Log("Move5");
                animFromBot.SetBool("Move5", true);
                break;
            case 5:
                //Debug.Log("Move6");
                animFromBot.SetBool("Move6", true);
                break;
            case 6:
                //Debug.Log("Move7");
                animFromBot.SetBool("Move7", true);
                break;
            case 7:
                //Debug.Log("Move8");
                animFromBot.SetBool("Move8", true);
                break;
            case 8:
                //Debug.Log("Move9");
                animFromBot.SetBool("Move9", true);
                break;
        }
        //botResetAnimation();
    }
    void botResetAnimation()
    {
        animFromBot.SetBool("Move1", false);
        animFromBot.SetBool("Move2", false);
        animFromBot.SetBool("Move3", false);
        animFromBot.SetBool("Move4", false);
        animFromBot.SetBool("Move5", false);
        animFromBot.SetBool("Move6", false);
        animFromBot.SetBool("Move7", false);
        animFromBot.SetBool("Move8", false);
        animFromBot.SetBool("Move9", false);
    }

    void DoDelayResetAnime(float delayTime)
    {
        StartCoroutine(DelayResetAnime(delayTime));
    }

    IEnumerator DelayResetAnime(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        animFromBot.SetBool("Move1", false);
        animFromBot.SetBool("Move2", false);
        animFromBot.SetBool("Move3", false);
    }

    float Minimax(int[] playSpace, int depth, bool isMax)
    {
        string winner = GameWinner();
        if (winner == "bot")
            return 10;
        if (winner == "player")
            return -10;
        if (winner == "raw")
            return 0;

        if (isMax)
        {
            float bestScore = -Mathf.Infinity;
            for (int i = 0; i < playSpace.Length; i++)
            {
                if (playSpace[i] == 0)
                {
                    playSpace[i] = 2;
                    float tempScore = Minimax(playSpace, depth + 1, false);
                    playSpace[i] = 0;
                    bestScore = Mathf.Max(tempScore, bestScore);
                    
                }
            }
            return bestScore;
        }
        else
        {
            float bestScore = Mathf.Infinity;
            for (int i = 0; i < playSpace.Length; i++)
            {
                if (playSpace[i] == 0)
                {
                    playSpace[i] = 1;
                    float tempScore = Minimax(playSpace, depth + 1, true);
                    playSpace[i] = 0;
                    bestScore = Mathf.Min(tempScore, bestScore);
                }
            }
            return bestScore;
        }
    }

    bool PlayableSpace()
    {
        for (int i = 0; i < playSpace.Length; i++)
        {
            if (playSpace[i] == 0)
            {
                return true;
            }
        }
            return false;
    }

    // return winner name base on tiles
    string GameWinner()
    {
        string winner ="";
        if (playSpace[0] == 1 && playSpace[1] == 1 && playSpace[2] == 1 ||
           playSpace[3] == 1 && playSpace[4] == 1 && playSpace[5] == 1 ||
           playSpace[6] == 1 && playSpace[7] == 1 && playSpace[8] == 1 ||
           playSpace[0] == 1 && playSpace[3] == 1 && playSpace[6] == 1 ||
           playSpace[1] == 1 && playSpace[4] == 1 && playSpace[7] == 1 ||
           playSpace[2] == 1 && playSpace[5] == 1 && playSpace[8] == 1 ||
           playSpace[0] == 1 && playSpace[4] == 1 && playSpace[8] == 1 ||
           playSpace[2] == 1 && playSpace[4] == 1 && playSpace[6] == 1
           )
        {
            winner =  "player";

        }

        if (playSpace[0] == 2 && playSpace[1] == 2 && playSpace[2] == 2 ||
            playSpace[3] == 2 && playSpace[4] == 2 && playSpace[5] == 2 ||
            playSpace[6] == 2 && playSpace[7] == 2 && playSpace[8] == 2 ||
            playSpace[0] == 2 && playSpace[3] == 2 && playSpace[6] == 2 ||
            playSpace[1] == 2 && playSpace[4] == 2 && playSpace[7] == 2 ||
            playSpace[2] == 2 && playSpace[5] == 2 && playSpace[8] == 2 ||
            playSpace[0] == 2 && playSpace[4] == 2 && playSpace[8] == 2 ||
            playSpace[2] == 2 && playSpace[4] == 2 && playSpace[6] == 2
            )
        {
            winner = "bot";
        }

        bool IsNotPlayble = PlayableSpace();
        if (IsNotPlayble == false && playerWin == false && botWin == false
            )
        {
            winner = "raw";
        }
        return winner;
    }


    // game winning effect
    void GameWinnerEffect()
    {
        if(playSpace[0] == 1 && playSpace[1] == 1 && playSpace[2] == 1)
        {
            PlayerWinTile[0].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[1].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[2].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[3] == 1 && playSpace[4] == 1 && playSpace[5] == 1)
        {
            PlayerWinTile[3].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[4].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[5].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }       
        if (playSpace[6] == 1 && playSpace[7] == 1 && playSpace[8] == 1 )
        {
            PlayerWinTile[6].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[7].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[8].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[0] == 1 && playSpace[3] == 1 && playSpace[6] == 1 )
        {
            PlayerWinTile[0].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[3].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[6].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[1] == 1 && playSpace[4] == 1 && playSpace[7] == 1 )
        {
            PlayerWinTile[1].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[4].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[7].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[2] == 1 && playSpace[5] == 1 && playSpace[8] == 1 )
        {
            PlayerWinTile[2].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[5].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[8].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[0] == 1 && playSpace[4] == 1 && playSpace[8] == 1 )
        {
            PlayerWinTile[0].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[4].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[8].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[2] == 1 && playSpace[4] == 1 && playSpace[6] == 1 )
        {
            PlayerWinTile[2].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[4].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            PlayerWinTile[6].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }


        if (playSpace[0] == 2 && playSpace[1] == 2 && playSpace[2] == 2)
        {
            BotWinTile[0].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[1].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[2].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[3] == 2 && playSpace[4] == 2 && playSpace[5] == 2)
        {
            BotWinTile[3].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[4].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[5].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[6] == 2 && playSpace[7] == 2 && playSpace[8] == 2)
        {
            BotWinTile[6].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[7].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[8].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[0] == 2 && playSpace[3] == 2 && playSpace[6] == 2)
        {
            BotWinTile[0].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[3].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[6].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[1] == 2 && playSpace[4] == 2 && playSpace[7] == 2)
        {
            BotWinTile[1].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[4].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[7].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[2] == 2 && playSpace[5] == 2 && playSpace[8] == 2)
        {
            BotWinTile[2].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[5].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[8].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[0] == 2 && playSpace[4] == 2 && playSpace[8] == 2)
        {
            BotWinTile[0].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[4].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[8].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
        if (playSpace[2] == 2 && playSpace[4] == 2 && playSpace[6] == 2)
        {
            BotWinTile[2].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[4].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            BotWinTile[6].transform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        }
    }

}
