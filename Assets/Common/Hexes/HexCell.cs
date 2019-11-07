using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class HexCell  : MonoBehaviour {
    public const float InnerOuterRatio = 0.866025404f;
    
    public TMP_Text positionText;
    public Vector3Int location;
    public Vector3Int cubeCoord;
    public float outerRadius = 2f;
    // this is a useful formula
    //innerRadius = outerRadius *innerOuterRatio;
    public Image center;
    public Image[] edges;
    
    
    public void SetLocation(Vector3Int v3, bool isRotate30) {
        location = v3;
        if (isRotate30) {
            cubeCoord = new Vector3Int(v3.x -v3.y/2, v3.y ,-1*(v3.x+(- v3.y / 2)+v3.y));
        } else {
            cubeCoord = new Vector3Int(v3.x , v3.y- v3.x/2,-1*(v3.x+(- v3.x / 2)+v3.y));  
        }
        if (positionText != null) {
            positionText.text = cubeCoord.ToString();
        }

        name += cubeCoord.ToString();
    }

}
