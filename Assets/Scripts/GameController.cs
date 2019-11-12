using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour{
    public bool isEditMode = false;
    public bool isPreloaded = false;

    public PartyController partyController;
    public LanguageController languageController;

    public static GameController inst;
    
    private void Awake() {
        inst = this;
    }


    public void InEditorSetup() {
        PartyController.inst = partyController;
        
    }
}
