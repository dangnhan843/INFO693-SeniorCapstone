using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotScript : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {/*
        if (Input.GetKey(KeyCode.UpArrow))
        {
            anim.SetTrigger("Move1");
        }
        else
        {
            anim.ResetTrigger("Move1");
        }
        */
    }
}
