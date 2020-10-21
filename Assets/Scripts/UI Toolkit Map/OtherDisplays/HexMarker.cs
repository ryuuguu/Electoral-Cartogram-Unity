using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.UIElements;

public class HexMarker : MonoBehaviour {

    public const string VTAHexMarker = "HexMarker";
    public static VisualElement marker;
    public static string localSpaceId;

    public static VisualElement MakeHexMarker(string aLocalSpaceId) {
        localSpaceId = aLocalSpaceId;
        marker = new VisualElement();
        var treeDetailDisplay = Resources.Load<VisualTreeAsset>(VTAHexMarker);
        treeDetailDisplay.CloneTree(marker);
        Show(false);
        return marker;
    }

    public static void MoveTo(Vector3 coord) {
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        marker.transform.position = CubeCoordinates.ConvertPlaneToLocalPosition(coord, ls);
        marker.transform.scale = Vector3.one * ls.gameScale*2; //*2 because gamescale uses radius not diameter
    }

    public static void Show(bool val) {
        marker.visible = val;
    }
}
