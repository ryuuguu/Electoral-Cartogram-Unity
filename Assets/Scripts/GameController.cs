using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{
    public bool isEditMode = false;

    public static GameController inst;
    
    private void Awake() {
        inst = this;
    }
}
