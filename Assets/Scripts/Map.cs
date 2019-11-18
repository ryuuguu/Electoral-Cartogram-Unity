using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
    public MapGrid mapGrid;
    public MapData mapData;
    public RegionEditor regionEditor;
    public ElectoralDistrictPanel electoralDistrictPanel;
    public Tooltip tooltip;
    
    public static Map inst;

    void Awake() {
        inst = this;
    }

    private void Start() {
        if (!GameController.inst.isPreloaded) {
            LoadMakeMap();
        }

        if (GameController.inst.isEditMode) {
            regionEditor.mapCell = (MapCell) mapGrid.cells[0];
            regionEditor.gameObject.SetActive(true);
        }
        
    }

    
    [ContextMenu("test makeMap in edit")]
    public void LoadMakeMap() {
        LoadMapDataResource();
        mapGrid.MakeGrid(mapData.widthRange,mapData.heightRange);
        MakeMapFromData();
    }
    
    /// <summary>
    /// Load Map data from a resource
    ///     works in WebGL
    /// </summary>
    public void LoadMapDataResource() {
        var mapDataJSON = Resources.Load<TextAsset>("MapData");
        if (mapDataJSON != null) {
            mapData = JsonUtility.FromJson<MapData>(mapDataJSON.text);
        }
        else {
            Debug.Log("could not load JSON TextAsset resource at MapData");
        }

        mapGrid.widthRange = mapData.widthRange;
        mapGrid.heightRange = mapData.heightRange;
        mapGrid.offset = mapData.offset;
        mapGrid.scale = mapData.scale;
        mapGrid.posCellScale = mapData.posCellScale;
    }

    /// <summary>
    ///Saves Map data in to a directory
    ///    using a directory means it does not work in WebGL
    /// </summary>
    public void SaveMapData() {
        var jsonText = JsonUtility.ToJson(mapData);
        string path =  Application.dataPath +"/Resources/"  ;
        if (!System.IO.Directory.Exists(path)) {
            System.IO.Directory.CreateDirectory(path);
            Debug.Log("Create Dir: " + path);
        }
        string fName = path +  "MapData.json";
        
        System.IO.File.WriteAllText( fName, jsonText);
        Debug.Log("Create file: " + fName); 
    }

    public static void SetMapData(MapCell mapCell) {
        var cellData = inst.mapData.cellDatas.Find((data => data.cubeCoord == mapCell.cubeCoord));
        if (cellData == null) {
            cellData = new CellData() {cubeCoord = mapCell.cubeCoord};
            inst.mapData.cellDatas.Add(cellData);
        }
        cellData.regionID = mapCell.regionList.id;
    }
    
    public void MakeMapFromData() {
        foreach (var cd in mapData.cellDatas) {
            var cell = mapGrid.cells.Find(data => data.cubeCoord == cd.cubeCoord);
            if (cell == null) {
                //Debug.Log("Cell not found:" + cd.cubeCoord + ":" + cd.regionID);
                continue;
            }

            var rl = RegionController.inst.regionList.Find(cd.regionID);
            if (rl.isRiding) rl.isAssigned = true;
            ((MapCell) cell).SetRegion(rl);
        }
    }
    
    public static void ClearHighLight() {
        inst.mapGrid.ClearHighLight();
    }

    public static MapCell GetCellAt(Vector3Int v3) {
        return inst.mapGrid.GetCellAt(v3);
    }

    [System.Serializable]
    public class MapData {
        public Vector2Int widthRange = Vector2Int.up;
        public Vector2Int heightRange = Vector2Int.up;
        public Vector2 offset = Vector2.zero;
        
        public float scale = 2;
        public float posCellScale = 4.94f;
        public List<CellData> cellDatas; 
    }
    
    [System.Serializable]
    public class CellData {
        public string regionID;
        public Vector3Int cubeCoord;
        
    }

    public static void ShowDistrictPopup(string id, Vector2 location, string message, Vector3 worldPos) {
        
        inst.tooltip.Show(id, location, message,worldPos);
    }
    
    public static void HideDistrictPopup(string id) {
        inst.tooltip.Hide(id);
    }
}
