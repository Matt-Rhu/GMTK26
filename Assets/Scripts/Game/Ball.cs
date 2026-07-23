using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static Ball instance;
    
    //états possibles: tenu, passé, tiré, idle
    //game over si le ballon sort du terrain
    
    private void Awake()
    {
        instance = this;
    }
}
