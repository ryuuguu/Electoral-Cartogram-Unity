using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HexGrid : MonoBehaviour {

    public bool isRotate30 = true;
    public int width = 54;
    public int height = 23;
    public Vector2 offset = Vector2.zero;
    
    public HexCell cellPrefab;

    public float scale = 2;
    public float posCellScale = 4.94f;
    
    public List<HexCell> cells;

    public static List<Vector3Int> edgeDirections = new List<Vector3Int>() {
        new Vector3Int(0,1,-1),
        new Vector3Int(1,0,-1),
        new Vector3Int(1,-1,0),
        new Vector3Int(0,-1,1),
        new Vector3Int(-1,0,1),
        new Vector3Int(-1,1,0)
    };
    /*
     0,1,-1
     1,0,-1
     1,-1,0
     0,-1,1
     -1,0,1
     -1,1,0
     */
    public void ClearGrid() {
        var cells = GetComponentsInChildren<HexCell>();
        for (int i = 0; i < cells.Length; i++) {
            Destroy(cells[i].gameObject);
        }
    }
    
    public void MakeGrid() {
        MakeGrid(new Vector2Int(0, width), new Vector2Int(0, height));
    }

    public void MakeGrid(Vector2Int widths, Vector2Int heights) {
        ClearGrid();
        cells = new List<HexCell>();
        int z = 0;
        for (int y = heights[0]; y < heights[1]; y++) {
            for (int x = widths[0]; x < widths[1]; x++) {
                var v3 = new Vector3Int(x,y,z);
                CreateCell(v3,offset);
            }
        }
    }

    
    
    public virtual void CreateCell(Vector3Int v3,Vector2 anOffset) {
        HexCell cell = Instantiate<HexCell>(cellPrefab);
        cells.Add(cell);
        SetCellPosition(cell, v3, anOffset);
    }

    public void SetCellPosition(HexCell cell, Vector3Int v3, Vector2 anOffset) {
        Vector3 pos = Vector3.zero;
        if (isRotate30) { // not tested 
            pos = new Vector3(
                (anOffset.x + v3.x+ v3.y * 0.5f  - v3.y / 2) * cell.outerRadius * HexCell.InnerOuterRatio *2f,
                (anOffset.y * cell.outerRadius * 2f) + v3.y * cell.outerRadius * 1.5f,
                0);
        }
        else { 
            pos = new Vector3(
                (anOffset.x + v3.x)  * cell.outerRadius * 1.5f,
                (anOffset.y + 
                + v3.y + v3.x * 0.5f - v3.x / 2 )* cell.outerRadius * HexCell.InnerOuterRatio * 2f,
                0); 
        }

        var tran = cell.transform;
        tran.SetParent(transform, false);
        tran.localPosition = pos*scale*posCellScale;
        tran.localScale = cell.transform.localScale*scale;
        cell.SetLocation(v3,isRotate30);
    }
    
}
        
    
