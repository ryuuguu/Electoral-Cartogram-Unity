using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LanguageController : MonoBehaviour {
    public int currentLanguage;
    public List<string> languages;
    public List<string> languageTitles;

    public UnityEvent languguageChanged;
    public static LanguageController inst;
    
    private void Awake() {
        inst = this;
    }

    public static int CurrentLanguage() {
        return inst.currentLanguage;
    }

    public static string ChooseName(string[] names) {
        if (names != null && names.Length > 0) {
           return names[Mathf.Min(names.Length - 1, LanguageController.CurrentLanguage())];
        }
        return "";
    }
    
    public static string ChooseName(List<string> names) {
        if (names != null && names.Count > 0) {
          return  names[Mathf.Min(names.Count - 1, LanguageController.CurrentLanguage())];
        }
        return "";
    }

    
    public static void Lang_0(bool val) {
        if (val) inst.LangSet(0);
    }

    public static void Lang_1(bool val) {
        if (val) inst.LangSet(1);
    }
    
    public void LangSet(int val) {
        if (currentLanguage != val) {
            currentLanguage = val; 
            languguageChanged?.Invoke();
        }
        
        // some sorta redraw call??
    }
    
    
}
