using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    
    static DateTime startTime;
    
    public static void StartTimer() {
        startTime = DateTime.Now;
        
    }

    public static int CalcTimer() {
        return ( DateTime.Now - startTime).Milliseconds;
    }
}
