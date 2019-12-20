using System;
using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;

public class UIHexGridOrderCells : MonoBehaviour {
    public UIHexGridOrdered subGrid;
    public Vector2Int widths = new Vector2Int(-5,6);
    public Vector2Int heights = new Vector2Int(-5,6);
    public bool isRectangle = false;
    
    private void Start() {
        Layout();
    }

    [ContextMenu("Make Layout")]
    public void Layout() {
        subGrid.cubeCoordinates.Construct(5);
        subGrid.MakeAllHexes(subGrid.localSpaceId);
        OrderBySpiral();
    }
    
    
    public void OrderBySpiral() {
        var orderedList = new List<HexUI>();
        var allCoords = subGrid.cubeCoordinates.GetCoordinatesFromContainer(subGrid.AllToken);
        Vector3Int currentCubeCoord = new Vector3Int(0,-5,5);
        Vector3Int[] incrs = new Vector3Int[] {
            new Vector3Int(1,-1,0),
            new Vector3Int(1,0,-1),
            new Vector3Int(0,1,-1),
            new Vector3Int(-1,1,0),
            new Vector3Int(-1,0,1),
            new Vector3Int(0,-1,1)
            
        };

        int index = 0;
        CubeCoordinates.Coord currentCell;
        currentCell =allCoords.Find(data => data.cubeCoord == currentCubeCoord);
        orderedList.Add(subGrid.hexes[subGrid.localSpaceId][currentCell.cubeCoord]);
        allCoords.Remove(currentCell);
        currentCubeCoord += incrs[index];

        bool finished = false;
        bool notFound = false;
        
        int i = 0;
        while (!finished) {
            currentCell =allCoords.Find(data => data.cubeCoord == currentCubeCoord);
            //if (currentCell.cubeCoord == Vector3.zero) { 
            if (!currentCell.isNotEmpty) {
                Debug.Log("not Found: " + currentCubeCoord+":"+ incrs[index] + ":"+ (currentCubeCoord - incrs[index]) );
                if (notFound) {
                    finished = true;
                }
                notFound = true;
                currentCubeCoord -= incrs[index];
                //Debug.Log("incr A: " + currentCubeCoord + " : " + incrs[index]);
                index++;
                index %= incrs.Length;
                currentCubeCoord += incrs[index];
                //Debug.Log("incr B: " + currentCubeCoord + " : " + incrs[index]);
                continue;
            }
            if(notFound) Debug.Log("Next: " + currentCubeCoord);
            notFound = false;
            orderedList.Add(subGrid.hexes[subGrid.localSpaceId][currentCell.cubeCoord]);
            allCoords.Remove(currentCell);
            //Debug.Log("count: "+ allCoords.Count);
            currentCubeCoord += incrs[index];
        }
        //hack :(
        currentCell.cubeCoord = Vector3.zero;
        orderedList.Add(subGrid.hexes[subGrid.localSpaceId][currentCell.cubeCoord]);
        subGrid.orderedCoords = orderedList;

    }
}
