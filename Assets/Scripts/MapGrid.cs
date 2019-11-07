using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : HexGrid {
    public override void CreateCell(Vector3Int v3,Vector2 anOffset) {
        MapCell cell = Instantiate<MapCell>((MapCell)cellPrefab);
        cell.SetRegion(RegionController.inst.regionList);
        cells.Add(cell);
        SetCellPosition(cell, v3, anOffset);
    }

    public void ClearHighLight() {
        foreach(var c in cells) {
            ((MapCell)c).SetHighLight(false);
        }
    }
    
    public MapCell GetCellAt(Vector3Int v3) {
        return (MapCell)cells.Find((cell => cell.cubeCoord == v3));
    }
    
}
