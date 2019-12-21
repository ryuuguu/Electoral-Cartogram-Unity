using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Com.Ryuuguu.HexGridCC;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIHexGrid : MonoBehaviour {

    public RectTransform holder;
    public HexUI prefab;
    public PointerTransform pointerTransform;

    
    public float gridScale = 20;
    public Vector2 offsetCoord = Vector3.zero;
    public Vector2 scaleV2 = Vector2.one;
    
    public Vector3 mouseCoord;
    

    public string localSpaceId;

    public string AllToken;
    
    public CubeCoordinates cubeCoordinates;

    /// <summary>
    /// hexes[localSpaceId][coord] = hexUI
    /// </summary>
    public Dictionary<string,Dictionary<Vector3, HexUI>> hexes = new Dictionary<string,Dictionary<Vector3, HexUI>>();
    
    protected void Awake() {
        Init();
    }

    public virtual void Init() {
        cubeCoordinates = new CubeCoordinates();
        AllToken = CubeCoordinates.AllContainer;
        localSpaceId = CubeCoordinates.NewLocalSpaceId(gridScale, scaleV2, CubeCoordinates.LocalSpace.Orientation.XY,
            holder, offsetCoord);
        MakeAllHexes(localSpaceId);
    }
    
    public void MakeAllHexes(string aLocalSpaceId) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        if (!hexes.ContainsKey(localSpaceId)) {
            hexes[aLocalSpaceId] = new Dictionary<Vector3, HexUI>();
        }
        if (ls.spaceRectTransform != null) {
            foreach (var coord in allCoords) {
                AddCell( coord.cubeCoord, ls);
            }
        }
    }


    public HexUI AddCell(Vector3 v3, CubeCoordinates.LocalSpace ls) {
        var coord = cubeCoordinates.GetAddCoordinateFromContainer(v3, AllToken);
        var localCoord = CubeCoordinates.ConvertPlaneToLocalPosition(coord.cubeCoord, ls);
        var hex = Instantiate(prefab, ls.spaceRectTransform);
        var tran = hex.transform;
        tran.localPosition = localCoord;
        tran.localScale = Vector3.one * ls.gameScale;
        hexes[ls.id][coord.cubeCoord] = hex;
        hex.name += coord.cubeCoord;
        hex.cubeCoord = coord.cubeCoord;
        return hex;
    }

    public HexUI AddCell(Vector3 v3, CubeCoordinates.LocalSpace ls,HexUI hex) {
        var coord = cubeCoordinates.GetAddCoordinateFromContainer(v3, AllToken);
        hexes[ls.id][coord.cubeCoord] = hex;
        return hex;
    }

    public void DestroyAllHexes(string aLocalSpaceId) {
        foreach (var hex in hexes[aLocalSpaceId].Values) {
            Destroy(hex.gameObject);
        }
        hexes[aLocalSpaceId].Clear();
    }
    
    public void MovePointer() { 
        //TODO refactor to use pointerTransform
        /*
        var hexCenteredPos = CubeCoordinates.ConvertPlaneToLocalPosition(mouseCoord, localSpaceId);
        pointerTransform.ShowPointer(hexCenteredPos,true);
       */
    }

    public Vector3  Mouse2Coord() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle( holder
            , Input.mousePosition, null, out var localPoint);
        // convert rectTrans to plane coord  & round
        return CubeCoordinates.PlaneToCube(CubeCoordinates.ConvertLocalPositionToPlane(localPoint, localSpaceId));
    }

    public void PointerToggleHighlight() {
        if (Input.GetMouseButtonDown(0)) {
            if (hexes[localSpaceId].ContainsKey(mouseCoord)) {
                hexes[localSpaceId][mouseCoord].ToggleHighlight();
            }
            
        }
    }
}

