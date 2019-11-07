using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageController : MonoBehaviour {
    public int currentLanguage;
    public List<string> languages;
    public List<string> languageTitles;

    public static LanguageController inst;
    
    private void Awake() {
        inst = this;
    }

    public static int CurrentLanguage() {
        return inst.currentLanguage;
    }
    
}
