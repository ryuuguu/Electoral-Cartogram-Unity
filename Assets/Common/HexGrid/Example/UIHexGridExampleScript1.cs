using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Com.Ryuuguu.HexGrid;

public class UIHexGridExampleScript1 : MonoBehaviour {

    public RectTransform holder;
    public CoordinateUI prefab;
    public PointerTransform pointerTransform;
    
    public Vector3 debugPos;
    public Vector3 mouseCoord;

    public string localSpaceId;

    protected string AllToken;
    
    public CubeCoordinates1 cubeCoordinates;

    
    private void Start() {
        cubeCoordinates = new CubeCoordinates1();
        AllToken = CubeCoordinates1.AllContainer;
        localSpaceId =  CubeCoordinates1.NewLocalSpaceId(20, CubeCoordinates1.LocalSpace.Orientation.XY, holder);
        var coordList = cubeCoordinates.Construct(2);
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
            ShowExample("line");

        if (Input.GetKeyDown(KeyCode.R))
            ShowExample("reachable");

        if (Input.GetKeyDown(KeyCode.S))
            ShowExample("spiral");

        if (Input.GetKeyDown(KeyCode.P))
            ShowExample("path");
    }

    public void MakeAllHexes(string aLocalSpaceId) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        var ls = CubeCoordinates1.GetLocalSpace(localSpaceId);
        Debug.Log("MakeAllHexes: "+allCoords.Count);
        
        if (ls.spaceRectTransform != null) {
            foreach (var coord in allCoords) {
                var localCoord = CubeCoordinates1.ConvertPlaneToLocalPosition(coord.cubeCoord, ls);
                var hex = Instantiate(prefab, ls.spaceRectTransform);
                var tran = hex.transform;
                tran.localPosition = localCoord;
                tran.localScale = Vector3.one*ls.gameScale;
            }
        }
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
        cubeCoordinates.Construct(10);

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
                cubeCoordinates.GetReachableCubes(Vector3.zero, 10)
            )
        );

        // Display Coordinates
        // cubeCoordinates.ShowCoordinatesInContainer(AllToken);

        // Construct Examples
        ConstructExamples();
    }

    private void ConstructExamples()
    {
        List<Vector3> allCubes = cubeCoordinates.GetCubesFromContainer(AllToken);

        // Line between the first and last cube coordinate
        cubeCoordinates.AddCubesToContainer(cubeCoordinates.GetLineBetweenTwoCubes(allCubes[0], allCubes[allCubes.Count - 1]), "line");

        // Reachable, 3 coordinates away from 0.0.0
        cubeCoordinates.AddCubesToContainer(cubeCoordinates.GetReachableCubes(Vector3.zero, 3), "reachable");

        // Spiral, 3 coordinates away from 0.0.0
        cubeCoordinates.AddCubesToContainer(cubeCoordinates.GetSpiralCubes(Vector3.zero, 3), "spiral");

        // Path between the first and last cube coordinate
        cubeCoordinates.AddCubesToContainer(cubeCoordinates.GetPathBetweenTwoCubes(allCubes[0], allCubes[allCubes.Count - 1]), "path");
    }

    private void ShowExample(string key)
    {
        //cubeCoordinates.HideCoordinatesInContainer(AllToken);
        //cubeCoordinates.ShowCoordinatesInContainer(key);
    }

    
}
