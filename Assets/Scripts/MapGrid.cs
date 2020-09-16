using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : HexGrid {
    public override void CreateCell(Vector3Int v3,Vector2 anOffset, bool isrectangle = false) {
        MapCell cell = Instantiate<MapCell>((MapCell)cellPrefab);
        cell.SetRegion(RegionController.inst.regionListInternal);
        cells.Add(cell);
        SetCellPosition(cell, v3, anOffset);
    }

    public  void CreateCellRegion(Vector3Int v3,RegionList rl) {
        //hack to not draw WATER & USA because of speed problems
        if (rl.id == "Water" || rl.id == "USA" || rl.id == "Land") return;
        MapCell cell = Instantiate<MapCell>((MapCell)cellPrefab);
        cell.SetRegion(rl);
        cells.Add(cell);
        //hack to convert cubeCoords to old grid cords
        var gridCoord = v3;
        gridCoord.y += v3.x / 2;
        
        SetCellPosition(cell, gridCoord, offset);
    }
    
    public void ClearHighLight() {
        foreach(var c in cells) {
            ((MapCell)c).SetHighLight(false);
        }
    }
    
    public MapCell GetCellAt(Vector3Int v3) {
        return (MapCell)cells.Find((cell => cell.cubeCoord == v3));
    }

    public void HideVotes(bool val) {
        foreach (MapCell mapCell in cells) {
            if (!(mapCell.subGrid is null)) {
                mapCell.subGrid.gameObject.SetActive(val);
            }

        }
    }
    
}
