using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Com.Ryuuguu.HexGrid;

public class HexGridExampleScript : MonoBehaviour {

    public Transform holder;
    public CoordinateTransform prefab;
    
     CubeCoordinates<CoordinateTransform> _cubeCoordinates;

    private void Awake() {
        
        
        //_cubeCoordinates = gameObject.AddComponent<CubeCoordinates>();
        
    }

    private void Start() {
        _cubeCoordinates = new CubeCoordinates<CoordinateTransform>();
       // holder = transform;
       //  var cth = new CoordinateTransformHelper();
       _cubeCoordinates.prefab = prefab;
        //_cubeCoordinates.coordinateHelper = cth;
        _cubeCoordinates.worldSpaceId = CoordinateTransform.NewWorldSpaceId(1, holder);
        _cubeCoordinates.CalculateCoordinateDimensions();
        _cubeCoordinates.Construct(2);
    }

    private void NewMap() {
        _cubeCoordinates.Construct(10);

        // Remove 25% of Coordinates except 0,0,0
        foreach (Vector3 cube in _cubeCoordinates.GetCubesFromContainer("all")) {
            if (cube == Vector3.zero)
                continue;

            if (Random.Range(0.0f, 100.0f) < 25.0f)
                _cubeCoordinates.RemoveCube(cube);
        }

        // Remove Coordinates not reachable from 0,0,0
        _cubeCoordinates.RemoveCubes(
            _cubeCoordinates.BooleanDifferenceCubes(
                _cubeCoordinates.GetCubesFromContainer("all"),
                _cubeCoordinates.GetReachableCubes(Vector3.zero, 10)
            )
        );

        // Display Coordinates
        _cubeCoordinates.ShowCoordinatesInContainer("all");

        // Construct Examples
        ConstructExamples();
    }

    private void ConstructExamples()
    {
        List<Vector3> allCubes = _cubeCoordinates.GetCubesFromContainer("all");

        // Line between the first and last cube coordinate
        _cubeCoordinates.AddCubesToContainer(_cubeCoordinates.GetLineBetweenTwoCubes(allCubes[0], allCubes[allCubes.Count - 1]), "line");

        // Reachable, 3 coordinates away from 0.0.0
        _cubeCoordinates.AddCubesToContainer(_cubeCoordinates.GetReachableCubes(Vector3.zero, 3), "reachable");

        // Spiral, 3 coordinates away from 0.0.0
        _cubeCoordinates.AddCubesToContainer(_cubeCoordinates.GetSpiralCubes(Vector3.zero, 3), "spiral");

        // Path between the first and last cube coordinate
        _cubeCoordinates.AddCubesToContainer(_cubeCoordinates.GetPathBetweenTwoCubes(allCubes[0], allCubes[allCubes.Count - 1]), "path");
    }

    private void ShowExample(string key)
    {
        _cubeCoordinates.HideCoordinatesInContainer("all");
        _cubeCoordinates.ShowCoordinatesInContainer(key);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NewMap();
            return;
        }
        if (_cubeCoordinates.GetCoordinatesFromContainer("all").Count == 0)
            return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            _cubeCoordinates.ShowCoordinatesInContainer("all");
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
}
