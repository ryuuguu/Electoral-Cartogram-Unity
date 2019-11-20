using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayCells : MonoBehaviour {
    public HexGrid subGrid;
    public Vector2Int widths = new Vector2Int(-5,6);
    public Vector2Int heights = new Vector2Int(-5,6);
    public bool cull2Hex = true;
    
    private void Start() {
        Layout();
    }

    [ContextMenu("Make Layout")]
    public void Layout() {
        subGrid.MakeGrid(widths, heights);
        if (cull2Hex) {
            CullMaxLimit(5);
        }
    }
    
    public void CullMaxLimit(int maxLimit) {
        List<HexCell> newList = new List<HexCell>();
        foreach (var cell in subGrid.cells) {
            if (cell.cubeCoord.x > maxLimit || cell.cubeCoord.y > maxLimit || cell.cubeCoord.z > maxLimit ||
                cell.cubeCoord.x < -1*maxLimit || cell.cubeCoord.y < -1*maxLimit || cell.cubeCoord.z < -1*maxLimit) { 
                //cell.transform.parent = null;
                Destroy(cell.gameObject);
                
            }
            else {
                newList.Add(cell);
            }
        }

        subGrid.cells = newList;
        OrderBySpiral();
    }

    /*
      0,-5,5
  incr by 1,0,-1
    til not found
    now at 5,-5,0
1,0,-1
 0,1,-1
-1,1,0
-1,0,1
0,-1,1
1,-1,0

    */
    public void OrderBySpiral() {
        List<HexCell> newList = new List<HexCell>();
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
        
        var currentCell =subGrid.cells.Find(data => data.cubeCoord == currentCubeCoord);
        newList.Add(currentCell);
        subGrid.cells.Remove(currentCell);
        currentCubeCoord += incrs[index];

        bool finished = false;
        bool notFound = false;
        
        while (!finished) {
            currentCell =subGrid.cells.Find(data => data.cubeCoord == currentCubeCoord);
            if (currentCell == null) {
                Debug.Log("not Found: " + currentCubeCoord+":"+ incrs[index] + ":"+ (currentCubeCoord - incrs[index]) );
                if (notFound) {
                    finished = true;
                }
                notFound = true;
                currentCubeCoord -= incrs[index];
                index++;
                index %= incrs.Length;
                currentCubeCoord += incrs[index];
                continue;
            }
            if(notFound) Debug.Log("Next: " + currentCubeCoord);
            notFound = false;
            newList.Add(currentCell);
            subGrid.cells.Remove(currentCell);
            currentCubeCoord += incrs[index];
        }
        //hack :(
        currentCubeCoord = new Vector3Int(0,0,0);
        currentCell =subGrid.cells.Find(data => data.cubeCoord == currentCubeCoord);
        if (currentCell != null) {
            newList.Add(currentCell);
            subGrid.cells.Remove(currentCell); 
        }

        subGrid.cells = newList;

    }
    
    
    
}
