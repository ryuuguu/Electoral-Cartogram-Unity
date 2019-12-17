using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Random = UnityEngine.Random;
using Com.Ryuuguu.HexGridCC;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MeshHexGridExampleScript : MonoBehaviour {

    public Transform holder;
    public HexMesh prefab;
    public CubeCoordinates.LocalSpace.Orientation orientation;
    public PointerTransform pointerTransform;
    public RectTransform pointerHolder;
    public CubeCoordinates.LocalSpace.Orientation pointerOrientation;
    public Camera pointerCamera;

    public int exampleRadius = 8;
    public float scale2Radius = 160f;
    public Vector2 offsetCoord;
    
    public CubeCoordinates.LocalSpace debugLocalSpace;
    public Vector3 mouseCoord;

    public string localSpaceId;
    public string pointerSpaceId;
    
    public Text displayTime1;
    public Text displayTime2;
    public Text displayHexes;
    
    protected string AllToken;
    
    public CubeCoordinates cubeCoordinates;

    //  Dictionary<localSpaceId,Coord, hexMesh>
    public Dictionary<string,Dictionary<Vector3, HexMesh>> hexes = new Dictionary<string,Dictionary<Vector3, HexMesh>>();

    
    private void Start() {
        cubeCoordinates = new CubeCoordinates();
        AllToken = CubeCoordinates.AllContainer;
        localSpaceId =  CubeCoordinates.NewLocalSpaceId(scale2Radius/exampleRadius, orientation, holder,offsetCoord);
        pointerSpaceId =  CubeCoordinates.NewLocalSpaceId(scale2Radius/exampleRadius, pointerOrientation, pointerHolder,offsetCoord);
        debugLocalSpace = CubeCoordinates.GetLocalSpace(localSpaceId);
        var coordList = cubeCoordinates.Construct(exampleRadius);
        MakeAllHexes(localSpaceId);
        NewMap();
    }
    
    void Update() {
        mouseCoord = Mouse2Coord();
        MovePointer();
        PointerToggleHighlight();
        if (Input.GetKeyDown(KeyCode.Return)) {
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

   
    
    public void MovePointer() { 
        var hexCenteredPos = CubeCoordinates.ConvertPlaneToLocalPosition(mouseCoord, pointerSpaceId);
        pointerTransform.ShowPointer(hexCenteredPos,true);
       
    }

    public Vector3  Mouse2Coord() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) pointerHolder
            , Input.mousePosition, pointerCamera, out var localPoint);
        // convert rectTrans to plane coord  & round
        return CubeCoordinates.PlaneToCube(CubeCoordinates.ConvertLocalPositionToPlane(localPoint, pointerSpaceId));
    }

    public void PointerToggleHighlight() {
        if (Input.GetMouseButtonDown(0)) {
            if (hexes[localSpaceId].ContainsKey(mouseCoord)) {
                hexes[localSpaceId][mouseCoord].ToggleHighlight();
            }
            
        }
    }
    

    public void MakeAllHexes(string aLocalSpaceId) {
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        if (!hexes.ContainsKey(localSpaceId)) {
            hexes[aLocalSpaceId] = new Dictionary<Vector3, HexMesh>();
        }
        if (ls.spaceTransform != null) {
            foreach (var coord in allCoords) {
                var localCoord = CubeCoordinates.ConvertPlaneToLocalPosition(coord.cubeCoord, ls);
                
                var hex = Instantiate(prefab, ls.spaceTransform);
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
    
    
    private void NewMap() {
        
        DestroyAllHexes(localSpaceId);
        Timer.StartTimer();
        localSpaceId =  CubeCoordinates.NewLocalSpaceId(scale2Radius/exampleRadius, orientation, holder,offsetCoord);
        cubeCoordinates.Construct(exampleRadius);

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
                cubeCoordinates.GetReachableCubes(Vector3.zero, exampleRadius )
            )
        );
        
        displayTime1.text = Timer.CalcTimer().ToString();
        Timer.StartTimer();
        MakeAllHexes(localSpaceId);
        
        displayTime2.text = Timer.CalcTimer().ToString();
        displayHexes.text = cubeCoordinates.GetCoordinatesFromContainer(AllToken).Count.ToString();
        
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
        cubeCoordinates.AddCubesToContainer(cubeCoordinates.GetReachableCubes(Vector3.zero, exampleRadius/3), "reachable");
 
        // Spiral, 3 coordinates away from 0.0.0
        cubeCoordinates.AddCubesToContainer(cubeCoordinates.GetSpiralCubes(Vector3.zero, exampleRadius/3), "spiral");
        
    }

    private void ShowExample(string aLocalSpaceId, string containerId) {
        
        var allCoords = cubeCoordinates.GetCoordinatesFromContainer(AllToken);
        foreach (var coord in allCoords) {
            hexes[aLocalSpaceId][coord.cubeCoord].Unhighlight();
             
        }
        
        var exampleCoords = cubeCoordinates.GetCoordinatesFromContainer(containerId);
        foreach (var coord in exampleCoords) {
            hexes[aLocalSpaceId][coord.cubeCoord].Highlight();
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
