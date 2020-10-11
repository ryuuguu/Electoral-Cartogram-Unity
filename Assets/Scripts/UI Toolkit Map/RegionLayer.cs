using System;
using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// Maange Region names displayed over map
/// </summary>
public class RegionLayer : MonoBehaviour {
    public RegionData[] regionDatas;

    public static VisualElement layer;
    public static RegionLayer inst;
    static private string localSpaceId;
    
    private void Awake() {
        inst = this;
    }

    static public void Init(string aLocalSpaceId, VisualElement aLayer) {
        localSpaceId = aLocalSpaceId;
        layer = aLayer;
    }
    
    public static void Redraw() {
        // cubeCord stuff
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        foreach (var rd in inst.regionDatas) {
            var ve = new TextElement();
            var location = CubeCoordinates.ConvertPlaneToLocalPosition(rd.coord, ls);
            layer.Add(ve);
            ve.text = rd.name;
            ve.transform.position = location;
            ve.style.position = Position.Absolute;
            ve.style.fontSize = 40;
        }
    }
    
    
    [Serializable]
    public struct RegionData {
        public Vector3 coord;
        public string name;
    }
    
}
