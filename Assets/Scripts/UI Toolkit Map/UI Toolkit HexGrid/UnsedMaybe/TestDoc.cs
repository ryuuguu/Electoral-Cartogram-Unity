using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

public class TestDoc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        var uiDoc= GetComponent<UIDocument>();
        var rootVE = uiDoc.rootVisualElement;
        
        var ve2 = new Button();
        
        ve2.style.width = 200;
        ve2.style.height = 200;
        ve2.style.backgroundColor = Color.black;
        //ve2.style.position = Position.Absolute;
        //ve2.style.top = 50;
        //ve2.style.left = 75;
        
        ve2.tooltip = "A toolTip";
        
       
        rootVE.Add(ve2);
       
        
    }
}