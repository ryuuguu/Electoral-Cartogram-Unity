using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

/// <summary>
/// Add UitHex MakeHex(Vector3 coord,Vector2 location, VisualElement aHolder)
/// :: Retrieve UitHex hexes[aLocalSpaceId][coord.cubeCoord] 
/// 
/// </summary>
public class UitHexGrid : MonoBehaviour {

    public Texture2D cellBackground;
    public bool isSquare = true;

    public float hexRadius = 50f;
    public float hexScalefactor = 1.8f; //2 would remove gap in most places but show artifact gaps
    public Vector2 offsetCoord = new Vector2(4, -3);
    public float squareScaleHeightHack = 0.004f;

    public string localSpaceId;

    protected static string AllToken;

    // CubeCoordinates manages hexes in 3 a dimensional coordinate space
    // has utility functions like finding paths and translating between cube coordinates and 2D local space for drawing
    public CubeCoordinates cubeCoordinates = new CubeCoordinates();

    public VisualElement hexHolder;

    // the initial string is a localSpaceID it allows multiple hex grids to handled separately 
    // inner dictionary is to find UI Toolkit elements from their coordinates
    public Dictionary<string, Dictionary<Vector3, UitHex>>
        hexes = new Dictionary<string, Dictionary<Vector3, UitHex>>();


    /// <summary>
    /// setup a local space
    /// </summary>
    /// <param name="aHexHolder"></param>
    public string Init(VisualElement aHexHolder) {
        hexHolder = aHexHolder;
        cubeCoordinates = new CubeCoordinates();
        AllToken = CubeCoordinates.AllContainer;
        localSpaceId = CubeCoordinates.NewLocalSpaceId(hexRadius / 2, Vector2.one,
            CubeCoordinates.LocalSpace.Orientation.XY, null, offsetCoord);
        return localSpaceId;
    }



    /// <summary>
    /// Makes a single UI ToolKit Hex at a given location 
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="location"></param>
    /// <param name="aHolder"></param>
    /// <returns></returns>
    protected UitHex MakeHex(Vector3 coord, Vector2 location, Vector3 scale, VisualElement aHolder = null) {
        if (aHolder == null) {
            aHolder = hexHolder;
        }

        var hex = MakeUitHex();
        aHolder.Add(hex);
        hex.name = coord.ToString();
        SetupHex(hex, location, scale);
        if (!hexes.ContainsKey(localSpaceId)) {
            hexes[localSpaceId] = new Dictionary<Vector3, UitHex>();
        }

        hexes[localSpaceId][coord] = hex;
        return hex;
    }

    /// <summary>
    /// returns a single new UitHex
    /// </summary>
    /// <returns></returns>
    public UitHex MakeUitHex() {
        var hex = new UitHex();
        hex.EnableInClassList("HexGrid-Hex", true);
        hex.AddToClassList("Hex");
        return hex;
    }

    /// <summary>
    /// Makes a single UI ToolKit Hex at the correct location for its coord
    /// under the default VisualElement
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    protected UitHex MakeHex(Vector3 coord, VisualElement aHolder = null) {
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        var location = CubeCoordinates.ConvertPlaneToLocalPosition(coord, ls);
        Vector3 scale;
        if (isSquare) {
            var l1 = CubeCoordinates.ConvertPlaneToLocalPosition(coord + new Vector3(0, 1, -1), ls);
            var l2 = CubeCoordinates.ConvertPlaneToLocalPosition(coord + new Vector3(0, -1, 1), ls);
            var l3 = CubeCoordinates.ConvertPlaneToLocalPosition(coord + new Vector3(1, 0, -1), ls);
            var l4 = CubeCoordinates.ConvertPlaneToLocalPosition(coord + new Vector3(-1, 0, 1), ls);
            var l5 = CubeCoordinates.ConvertPlaneToLocalPosition(coord + new Vector3(1, -1, 0), ls);
            var l6 = CubeCoordinates.ConvertPlaneToLocalPosition(coord + new Vector3(-1, 1, 0), ls);
            var highX = Mathf.Max(l1.x, l2.x, l3.x, l4.x, l5.x, l6.x);
            var lowX = Mathf.Min(l1.x, l2.x, l3.x, l4.x, l5.x, l6.x);
            var highY = Mathf.Max(l1.y, l2.y, l3.y, l4.y, l5.y, l6.y);
            var lowY = Mathf.Min(l1.y, l2.y, l3.y, l4.y, l5.y, l6.y);
            scale = new Vector3(highX - lowX, highY - lowY, 1) / 2f;
            //Debug.Log("One MakeHex scale: "+ scale.x + " : "+ scale.y);
        }
        else {
            scale = Vector3.one * ls.gameScale * hexScalefactor;
        }
        return MakeHex(coord, location, scale, aHolder);
    }

    /// <summary>
    /// Make All hexes in a local space
    /// </summary>
    /// <param name="aLocalSpaceId"></param>
    public void MakeAllHexes(string aLocalSpaceId) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        
        if (!hexes.ContainsKey(localSpaceId)) {
            hexes[aLocalSpaceId] = new Dictionary<Vector3, UitHex>();
        }

        foreach (var coord in allCoords) {
            var hex = MakeHex(coord.cubeCoord, hexHolder);
            hexes[aLocalSpaceId][coord.cubeCoord] = hex;
        }

    }

    /// <summary>
    /// setup location specific parts of the hex
    /// </summary>
    /// <param name="hex"></param>
    /// <param name="location"></param>
    static public void SetupHex(UitHex hex, Vector2 location, Vector3 scale) {
        hex.transform.position = (Vector3) location;
        hex.transform.scale = scale;
        hex.style.backgroundImage = null;
    }

    public Vector3 Position2Coord(Vector2 localPosition, Vector2 offsetInCubeCoord) {
        return CubeCoordinates.PlaneToCube(CubeCoordinates.
            ConvertLocalPositionToPlane(localPosition, localSpaceId, offsetInCubeCoord));
    }

}