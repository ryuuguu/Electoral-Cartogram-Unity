﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Com.Ryuuguu.HexGrid;

public class UIHexGridExampleScript : MonoBehaviour {

    public Transform holder;
    public CoordinateUI prefab;

    protected string AllToken;
    
     CubeCoordinates<CoordinateUI> _cubeCoordinates;

    private void Awake() {
        
        
        //_cubeCoordinates = gameObject.AddComponent<CubeCoordinates>();
        
    }

    private void Start() {
        _cubeCoordinates = new CubeCoordinates<CoordinateUI>();
        AllToken = CubeCoordinates<CoordinateUI>.AllContainer;
        _cubeCoordinates.prefab = prefab;
        _cubeCoordinates.worldSpaceId = CoordinateUI.NewWorldSpaceId(1, holder);
        _cubeCoordinates.Construct(2);
    }

    private void NewMap() {
        _cubeCoordinates.Construct(10);

        // Remove 25% of Coordinates except 0,0,0
        foreach (Vector3 cube in _cubeCoordinates.GetCubesFromContainer(AllToken)) {
            if (cube == Vector3.zero)
                continue;

            if (Random.Range(0.0f, 100.0f) < 25.0f)
                _cubeCoordinates.RemoveCube(cube);
        }

        // Remove Coordinates not reachable from 0,0,0
        _cubeCoordinates.RemoveCubes(
            _cubeCoordinates.BooleanDifferenceCubes(
                _cubeCoordinates.GetCubesFromContainer(AllToken),
                _cubeCoordinates.GetReachableCubes(Vector3.zero, 10)
            )
        );

        // Display Coordinates
        _cubeCoordinates.ShowCoordinatesInContainer(AllToken);

        // Construct Examples
        ConstructExamples();
    }

    private void ConstructExamples()
    {
        List<Vector3> allCubes = _cubeCoordinates.GetCubesFromContainer(AllToken);

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
        _cubeCoordinates.HideCoordinatesInContainer(AllToken);
        _cubeCoordinates.ShowCoordinatesInContainer(key);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NewMap();
            return;
        }
        if (_cubeCoordinates.GetCoordinatesFromContainer(AllToken).Count == 0)
            return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            _cubeCoordinates.ShowCoordinatesInContainer(AllToken);
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
