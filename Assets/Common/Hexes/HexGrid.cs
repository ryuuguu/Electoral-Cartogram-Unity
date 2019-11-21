using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class HexGrid : MonoBehaviour {

    public bool isRotate30 = true;
    public Vector2Int widthRange = Vector2Int.up;
    public Vector2Int heightRange = Vector2Int.up;

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
            #if UNITY_EDITOR
            DestroyImmediate(cells[i].gameObject);
            #else
            Destroy(cells[i].gameObject);
            #endif
        }
    }
    
    
    public void MakeGrid(Vector2Int widths, Vector2Int heights, bool isRectangle = false) {
        ClearGrid();
        cells = new List<HexCell>();
        int z = 0;
        for (int y = heights[0]; y < heights[1]; y++) {
            for (int x = widths[0]; x < widths[1]; x++) {
                var v3 = new Vector3Int(x,y,z);
                CreateCell(v3,offset,isRectangle);
            }
        }
    }

    
    
    public virtual void CreateCell(Vector3Int v3,Vector2 anOffset, bool isRectangle = false) {
        HexCell cell = Instantiate<HexCell>(cellPrefab);
        cells.Add(cell);
        SetCellPosition(cell, v3, anOffset,isRectangle);
    }

    public void SetCellPosition(HexCell cell, Vector3Int v3, Vector2 anOffset, bool isRectangle = false) {
        Vector3 pos = Vector3.zero;
        var majorM = cell.outerRadius * HexCell.InnerOuterRatio * 2f;
        var minorM = cell.outerRadius * 1.5f;
        if (isRectangle) {
            pos = new Vector3(
                (anOffset.x + v3.x) * cell.outerRadius * 1.5f,
                (anOffset.y +
                 +v3.y ) * cell.outerRadius * HexCell.InnerOuterRatio * 2f,
                0);
        }
        else {

            if (isRotate30) {
                pos = new Vector3(
                    (anOffset.x + v3.x + v3.y * 0.5f - v3.y / 2) * majorM,
                    (anOffset.y * cell.outerRadius * 2f) + v3.y * minorM,
                    0);
            }
            else {
                pos = new Vector3(
                    (anOffset.x + v3.x) * minorM,
                    (anOffset.y +v3.y + v3.x * 0.5f - v3.x / 2) *majorM,
                    0);
            }
        }

        var tran = cell.transform;
        tran.SetParent(transform, false);
        tran.localPosition = scale * posCellScale * pos;
        tran.localScale = cell.transform.localScale*scale;
        cell.SetLocation(v3,isRotate30);
    }

    public Vector3 InvertCellPosition(HexCell cell, Vector3 v3) {
        Vector3 pos ;
        var majorM = cell.outerRadius * HexCell.InnerOuterRatio * 2f;
        var minorM = cell.outerRadius * 1.5f;
            if (isRotate30) {
                pos = new Vector3(
                    -1* offset.x + (v3.x - v3.y/2 )/majorM,
                    -1*offset.y * 4f/3f  + v3.y/ minorM,
                    0);
            }
            else {
                pos = new Vector3(
                    -1*offset.x * 4f/3f + v3.x/ minorM,
                    -1*offset.y + (v3.y -v3.x)/majorM,
                    0);
            }

            return pos*(1/(scale*posCellScale));
    }
 
    static float sin60 =Mathf.Sin(Mathf.PI / 6f);
    public static Vector2 GetCubeCoord(Vector2 point, float radius) {
        // Find out which major row and column we are on:
        int row = (int)(point.y / (radius * sin60));
        int column= (int)(point.x / (radius * 1.5f));
     
        // Compute the offset into these row and column:
        float dy = point.y - row * radius * sin60;
        float dx = point.x - column * radius * 1.5f;
     
        // Are we on the left of the hexagon edge, or on the right?
        if (((row ^ column) & 1) == 0)
        {
            dy = sin60 - dy;
        }
     
        int right = dy * radius * 0.5f < sin60 * (dx - radius * 0.5f) ? 1 : 0;
     
        // Now we have all the information we need, just fine-tune row and column.
        row += (column ^ row ^ right) & 1;
        column += right;
     
        return new Vector2(column, row / 2);
    }
    
}
        
    
