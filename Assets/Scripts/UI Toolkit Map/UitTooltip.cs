using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// display a message near a given point
/// offset position to keep on screen
/// 
/// </summary>
public class UitTooltip  {
    public static TextElement textElement;
    public static float width = 250;
    public static float height = 50 ;
    
    public static TextElement Init() {
        textElement = new TextElement();
        textElement.visible = false;
        textElement.style.position = Position.Absolute;
        textElement.style.width = width;
        textElement.style.height = height;
        textElement.style.backgroundColor = Color.black;
        textElement.style.color = Color.white;
        textElement.style.fontSize = 20;
        return textElement;
    }

    public static void Show(Vector3 position, Vector2 screenPosition, string message) {
        var xRatio = -1 *  screenPosition.x / Screen.safeArea.xMax;
        var yRatio = -1.8f;
        if (screenPosition.y / Screen.safeArea.yMax < 0.2) {
            yRatio = 0.8f;
        }
        var offset = new Vector3(width * xRatio, height * yRatio,0);
        
        textElement.text = message;
        textElement.visible = true;
        textElement.transform.position = position + offset;
    }
    

    public static void Hide() {
        textElement.visible = false;
    }
}
