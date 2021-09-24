using System.Collections;
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
    float controllerSpeedHorizontal = 1.5f;
    float controllerSpeedVertical = 1.5f;
    float controllerYaw = 0.0f;
    float controllerPitch = 0.0f;
    float delayTime = 1.0f;
    private bool delayTimeCounter = false;

    string tileName ;
    int tileNumberX ;


    private bool isGrabbing = false;             //A control variable that stores if the user is grabbing anything
    private Transform grabbedTransform;          //A variable to hold the grabbed object's transform
    private Transform clickTransform;
    public float zSpeed = 4.5f;                  //A variable to control the speed of movement in z-axis (forward/backward)
    public float rotationSpeedMultiplier = 2.0f; //A variable to increase the rotational speed for the controller
    private Transform hitTransform;              //A variable to hold the hit object's transform
    public int turnCount = 0;
    public bool playerFirst = true;
    bool playerTurn = true;
    bool playerWin = false;
    //bool botTurn = false;
    bool botWin = false;

    bool delayBotTime = false;
    private int tempMovePosition =10;

    public GameObject[] Os;
    public GameObject[] Xs;
    public int[] playSpace;
    public GameObject[] TTTs;

    void Start()
    {
        //AITurnFirst(2.0f);
        DoDelayReset(0.0f);
        //Debug.Log(playerFirst.ToString());
        TTTs = GameObject.FindGameObjectsWithTag("TTTSpace");
        foreach (GameObject TTT in TTTs)
        {
            TTT.transform.GetComponent<SpriteRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        }
    }

    void Update()
    {
        /*
        if (delayTimeCounter == true)
            StartCoroutine(delayReset());
        
        if (delayBotTime == true)
            StartCoroutine(delayBotMove(tempMovePosition));
            */
        //winConditionChecking();

        if (Input.GetKey(KeyCode.C)) //When the C key is pressed, we'll rotate the controller with the mouse movements
        {
            controllerYaw += controllerSpeedHorizontal * Input.GetAxis("Mouse X") * rotationSpeedMultiplier;
            controllerPitch += controllerSpeedVertical * Input.GetAxis("Mouse Y") * -rotationSpeedMultiplier;
            transform.localRotation = Quaternion.Euler(controllerPitch, controllerYaw, 0.0f);
        }

        //We are casting a ray from the controller to check for hits with grabbable objects
        RaycastHit hitInfo2;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo2))
        {
            if (hitInfo2.transform.tag == "TTTSpace")
            {
                if (hitTransform != null)
                    hitTransform.GetComponent<SpriteRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                hitTransform = hitInfo2.transform;
                hitTransform.GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                //Debug.Log(hitInfo2.transform.GetComponent<SpriteRenderer>().material.color);
            }
            else
            {
                hitTransform.GetComponent<SpriteRenderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }

            //CheckHit(hitInfo2);
            
            /*
            //If we are pointing at a grabbable object, but not grabbing
            if (hitInfo2.transform.tag == "Grabbable" && !isGrabbing)
            {
                if (hitTransform != null)
                    SetHighlight(hitTransform, false); //We dim the previously pointed object

                hitTransform = hitInfo2.transform; //We update the hitTransform as the currently pointed object's transform
                SetHighlight(hitTransform, true); //We highlight the currently pointed object
            }

            else
            {
                if (hitTransform != null && !isGrabbing) //If we are hitting a non-grabbable object
                    SetHighlight(hitTransform, false); //We dim the previously pointed object
            }
            //
            */


        }

        else //If we are not hitting any object
        {
            if (hitTransform != null && !isGrabbing)
            {
                SetHighlight(hitTransform, false); //We dim the previously pointed object
            }
        }
        
        if (playerTurn == true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0)) //If we are pressing down the left mouse button
            {
                RaycastHit hitInfo;


                if (delayTimeCounter == false)
                {
                    if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo))
                    {

                        tileName = CheckHitName(hitInfo);
                        tileNumberX = CheckHitNumber(hitInfo);
                        if (hitInfo.transform.tag == "TTTSpace" && playSpace[tileNumberX - 1] == 0) //If we are hitting a grabbable object
                        {
                            //clickTransform = hitInfo.transform;
                            playSpace[tileNumberX - 1] = 1;
                            turnCount++;
                            
                            Xs[tileNumberX - 1].SetActive(true);
                            //Debug.Log(tileNumberX - 1);
                            winConditionChecking();
                            playerTurn = false;


                        }
                        //winConditionChecking();

                        if (hitInfo.transform.tag == "Grabbable") //If we are hitting a grabbable object
                        {
                            isGrabbing = true; //We turn the control variable to true indicating that we are grabbing

                            grabbedTransform = hitInfo.transform;
                            //We are setting isKinematic as true to control the movement via code
                            grabbedTransform.GetComponent<Rigidbody>().isKinematic = true;
                            //We are setting useGravity as false to control the movement via code
                            grabbedTransform.GetComponent<Rigidbody>().useGravity = false;
                            //We are setting the gameobject to which this script is attached as the parent of the grabbed 
                            //gameobject so that the movement of the gameobject to which this script is attached is reflected
                            //to the child (the grabbed game object is anchored to this gameobject)
                            grabbedTransform.parent = transform;
                        }
                    }
                }

            }
            
        }
        else
        {
            bool tempGameEnd = checkEndMove(playSpace);
            bool AIMove = true;
            if (delayBotTime == false)
            {
                if (!tempGameEnd)
                {
                    while (AIMove)
                    {
                        int movePosition = Random.Range(0, 8);
                        if (movePosition != (tileNumberX - 1))
                        {
                            if (playSpace[movePosition] == 0)
                            {
                                tempMovePosition = movePosition;
                                delayBotTime = true;
                                DoDelayAction(movePosition, 2.0f);
                                AIMove = false;
                            }
                        }

                    }
                }
            }
            playerTurn = true;
        }
        

        //Debug.Log(turnCount.ToString());

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

    int  CheckHitNumber(RaycastHit hitInfo2)
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

    void winConditionChecking()
    {
        //Debug.Log("Win Check");
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
            Debug.Log("Player Win");
            playerWin = true;
            //playerFirst = false;
            //delayTimeCounter = true;
            DoDelayReset(2.0f);

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
            //Debug.Log("Bot Win");
            botWin = true;
            //playerFirst = true;
            //delayTimeCounter = true;
            DoDelayReset(2.0f);
        }

        /*
        bool isDraw = false;
        for ( int i = 0; i> playSpace.Length; i++)
        {
            int tempCount = 0;
            if (playSpace[i] == 0)
            {
                isDraw = false;

            }
            else
            {
                isDraw = true;
            }
        }
        */
        if (turnCount>= 9 && playerWin == false && botWin == false
            )
        {
            Debug.Log("Draw");
            DoDelayReset(2.0f);
            /*
            Debug.Log("Draw");
            delayTimeCounter = true;
            */
        }
        
        if (playerFirst == false)
        {
            //Debug.Log("player first");
            //AITurnFirst(2.0f)
            playerFirst = true;
        }
        
    }
    void DoDelayReset(float delayTime)
    {
        StartCoroutine(resetGame(delayTime));
    }
    IEnumerator resetGame(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        turnCount = 0;
        
        foreach (GameObject X in Xs)
        {
            X.SetActive(false);
        }
        foreach (GameObject O in Os)
        {
            O.SetActive(false);
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

        playerWin = false;
        botWin = false;
        delayTimeCounter = false;
        tempMovePosition = 10;
    }
    

    void AITurnFirst(float delayTime)
    {
        StartCoroutine(delayAITurn(delayTime));

    }

    IEnumerator delayAITurn(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        int movePosition = Random.Range(0, 8);
        if (playSpace[movePosition] == 0)
        {
            Os[movePosition].SetActive(true);
            turnCount++;
            Debug.Log(turnCount.ToString());
            playSpace[movePosition] = 2;
        }
        playerFirst = true;
    }

    /*
    IEnumerator delayReset()
    {
        playerWin = false;
        yield return new WaitForSeconds(2.0f);
        resetGame();
        //playerWin = false;
        delayTimeCounter = false;
        tempMovePosition = 10;
    }
    */
    /*
    IEnumerator delayBotMove(int movePosition)
    {
        yield return new WaitForSeconds(0.0f);
        if (tempMovePosition != 10)
        {
            //Debug.Log("Moving");
            //yield return new WaitForSeconds(0.0f);
            Os[movePosition].SetActive(true);
            
            //Debug.Log(turnCount.ToString());
            playSpace[movePosition] = 2;
            delayBotTime = false;
            winConditionChecking();
        }
        //Debug.Log("not Moving");

    }
    */



    void DoDelayAction(int movePosition, float delayTime)
    {
        StartCoroutine(delayBotMove(movePosition,delayTime));
    }

    IEnumerator delayBotMove(int movePosition,float delayTime)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        if (tempMovePosition != 10)
        {
            //Debug.Log("Moving");
            //yield return new WaitForSeconds(0.0f);

            Os[movePosition].SetActive(true);

            //Debug.Log(turnCount.ToString());
            playSpace[movePosition] = 2;
            delayBotTime = false;
            turnCount++;
            winConditionChecking();
        }
        //Do the action after the delay time has finished.
    }
}
