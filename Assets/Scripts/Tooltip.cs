using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour {
    public string id;
    public TMP_Text text;
    
    public void Hide(string anId) {
        if(id == anId) gameObject.SetActive(false);
    }

    public void Show(string anId, Vector2 screenLocation, string message, Vector3 worldPos) {
        gameObject.SetActive(true);
        
        id = anId;
        text.text = message;

        var right = (int)(screenLocation.x* 2/ Screen.width );
        var top = (int)(screenLocation.y* 2/  Screen.height);
        var rt = ((RectTransform) transform);
        rt.pivot = new Vector2(right,top);
        Vector3 pos;
        var rectTrans = ((RectTransform) transform.parent);
        //RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTrans, screenLocation, Camera.main, out pos);
        transform.position = worldPos;
    }

}
