﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Com.Ryuuguu.HexGrid;

public class UIHexGridExampleScript1 : MonoBehaviour {

    public RectTransform holder;
    public HexUI prefab;
    public PointerTransform pointerTransform;

    public int exmpleRadius = 8;
    
    public Vector3 debugPos;
    public Vector3 mouseCoord;

    public string localSpaceId;

    protected string AllToken;
    
    public CubeCoordinates1 cubeCoordinates;

    public Dictionary<string,Dictionary<Vector3, HexUI>> hexes = new Dictionary<string,Dictionary<Vector3, HexUI>>();

    
    private void Start() {
        cubeCoordinates = new CubeCoordinates1();
        AllToken = CubeCoordinates1.AllContainer;
        localSpaceId =  CubeCoordinates1.NewLocalSpaceId(20, CubeCoordinates1.LocalSpace.Orientation.XY, holder);
        var coordList = cubeCoordinates.Construct(1);
        Debug.Log(" Start(): "+ coordList.Count);
        MakeAllHexes(localSpaceId);
    }
    
    private void Update() {
        MovePointer();
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NewMap();
            return;
        }
        if (cubeCoordinates.GetCoordinatesFromContainer(AllToken).Count == 0)
            return;

        if (Input.GetKeyDown(KeyCode.C))
        {
           // _cubeCoordinates.ShowCoordinatesInContainer(AllToken);
        }

        if (Input.GetKeyDown(KeyCode.L))
            ShowExample(localSpaceId,"line");

        if (Input.GetKeyDown(KeyCode.R))
            ShowExample(localSpaceId,"reachable");

        if (Input.GetKeyDown(KeyCode.S))
            ShowExample(localSpaceId,"spiral");

        if (Input.GetKeyDown(KeyCode.P))
            ShowExample(localSpaceId,"path");
    }

    public void MakeAllHexes(string aLocalSpaceId) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        var ls = CubeCoordinates1.GetLocalSpace(localSpaceId);
        if (!hexes.ContainsKey(localSpaceId)) {
            hexes[aLocalSpaceId] = new Dictionary<Vector3, HexUI>();
        }
        if (ls.spaceRectTransform != null) {
            foreach (var coord in allCoords) {
                var localCoord = CubeCoordinates1.ConvertPlaneToLocalPosition(coord.cubeCoord, ls);
                var hex = Instantiate(prefab, ls.spaceRectTransform);
                var tran = hex.transform;
                tran.localPosition = localCoord;
                tran.localScale = Vector3.one*ls.gameScale;
                hexes[aLocalSpaceId][coord.cubeCoord] = hex;
                hex.name += coord.cubeCoord;
            }
        }
    }

    public void DestroyAllHexes(string aLocalSpaceId) {
       
        foreach (var hex in hexes[aLocalSpaceId].Values) {
            Destroy(hex.gameObject);
        }
        hexes[aLocalSpaceId].Clear();

    }
    
    private void MovePointer() {
        Vector3 worldPoint;
        
        if (Input.GetMouseButton(0)) {
            // convert mouse to local rectTransform 
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, Input.mousePosition,null,out  var localPoint);
            // convert rectTrans to plane coord  & round
            mouseCoord = CubeCoordinates1.ConvertLocalPositionToPlane(localPoint, localSpaceId);
            //convert backTrans 
            var hexCenteredPos =
                CubeCoordinates1.ConvertPlaneToLocalPosition(mouseCoord, localSpaceId);
            pointerTransform.ShowPointer(hexCenteredPos,true);
        }
    }

    
    private void NewMap() {
        DestroyAllHexes(localSpaceId);
        cubeCoordinates.Construct(exmpleRadius);

        // Remove 25% of Coordinates except 0,0,0
        foreach (Vector3 cube in cubeCoordinates.GetCubesFromContainer(AllToken)) {
            if (cube == Vector3.zero)
                continue;

            if (Random.Range(0.0f, 100.0f) < 25.0f)
                cubeCoordinates.RemoveCube(cube);
        }

        // Remove Coordinates not reachable from 0,0,0
        cubeCoordinates.RemoveCubes(
            cubeCoordinates.BooleanDifferenceCubes(
                cubeCoordinates.GetCubesFromContainer(AllToken),
                cubeCoordinates.GetReachableCubes(Vector3.zero, exmpleRadius )
            )
        );

        MakeAllHexes(localSpaceId);
        
        // Construct Examples
        ConstructExamples();
    }

    private void ConstructExamples() {
        List<Vector3> allCubes = cubeCoordinates.GetCubesFromContainer(AllToken);
        // Line between the first and last cube coordinate
        var line = cubeCoordinates.GetLineBetweenTwoCubes(allCubes[0], allCubes[allCubes.Count - 1]);
        cubeCoordinates.AddCubesToContainer(line , "line");
        
        // Path between the first and last cube coordinate
        cubeCoordinates.AddCubesToContainer(cubeCoordinates.GetPathBetweenTwoCubes(allCubes[0], allCubes[allCubes.Count - 1]), "path");
        
        // Reachable, 3 coordinates away from 0.0.0
        cubeCoordinates.AddCubesToContainer(cubeCoordinates.GetReachableCubes(Vector3.zero, 3), "reachable");
 
        // Spiral, 3 coordinates away from 0.0.0
        cubeCoordinates.AddCubesToContainer(cubeCoordinates.GetSpiralCubes(Vector3.zero, 3), "spiral");
        
    }

    private void ShowExample(string aLocalSpaceId, string containerId) {
        
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        foreach (var coord in allCoords) {
            hexes[aLocalSpaceId][coord.cubeCoord].Hide();
             
        }
        
        var exampleCoords = cubeCoordinates.GetCoordinatesFromContainer(containerId);
        foreach (var coord in exampleCoords) {
            hexes[aLocalSpaceId][coord.cubeCoord].Show();
        }
    }
    
    private void DebugCoords(string msg, string containerName) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(containerName);
        Debug.Log("=============== " + msg );
        foreach (var coord in allCoords) {
            Debug.Log( msg+ " : " + containerName+ " : " +coord.cubeCoord);
        }
    }

}
