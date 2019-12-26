using System;
using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;

public class UIHexGridMap2 : MonoBehaviour {
    public UIHexGridMapGrid2 mapGrid;
    public MapData mapData;
    public RegionEditor regionEditor;
    public ElectoralDistrictPanel electoralDistrictPanel;
    public Tooltip tooltip;
    public RectTransform selectedMarker;
    
    
    private int delayMapBuild = 1;

    bool inRiding = false;
    
    static Vector3 nullCoord = Vector3.one;
    Vector3 prevMouseCoord = nullCoord;
    
    public static UIHexGridMap2 inst;

    void Awake() {
        inst = this;
    }
    
    void Update() {
        if (delayMapBuild == 0) {
            MapBuild();
        }
        delayMapBuild--;
       
        var mouseCoord = mapGrid.Mouse2Coord();
        
        var regionList = GetCellDataAt(mouseCoord);
        if (regionList != null) {
            if (!regionList.isRiding) {
                prevMouseCoord = nullCoord;
                tooltip.Hide("");
            } else {
                if (mouseCoord != prevMouseCoord) {
                    tooltip.Show("", Input.mousePosition, LanguageController.ChooseName(regionList.names),
                        Input.mousePosition);
                }
                if (Input.GetMouseButtonDown(0)) {
                    selectedMarker.anchoredPosition = mapGrid.Coord2Local(mouseCoord);
                    ElectoralDistrictPanel.SetRegionList(regionList);
                }
            }
        }
        else {
            prevMouseCoord = nullCoord;
            tooltip.Hide("");
        }
        prevMouseCoord = mouseCoord;

    }
    
    public void MapBuild() {
        LoadMakeMap();
        if (GameController.inst.isEditMode) {
            //TODO fix region editor commented out code 2 lines
            //regionEditor.mapCell = (MapCell) mapGrid.cells[0];
            //regionEditor.gameObject.SetActive(true);
        }
    }

    public void HideVotes(bool val) {
        mapGrid.HideVotes(val);
    }
    
    
    [ContextMenu("test makeMap in edit")]
    public void LoadMakeMap() {
        LoadMapDataResource();
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
        //mapGrid.widthRange = mapData.widthRange;
        //mapGrid.heightRange = mapData.heightRange;
        Debug.Log(mapGrid);
        Debug.Log(mapGrid.offsetCoord);
        Debug.Log(mapData);
        Debug.Log(mapData.offset);
        //mapGrid.offsetCoord = mapData.offset;
        //mapGrid.gridScale = mapData.scale;
        //mapGrid.posCellScale = mapData.posCellScale;
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
        ECSSpawner.Init(CubeCoordinates.GetLocalSpace(mapGrid.localSpaceId));
        foreach (var cd in mapData.cellDatas) {
            //var rl = RegionController.inst.regionList.Find(cd.regionID);
            var rl = RegionController.Find(cd.regionID);
            mapGrid.CreateCellRegion(cd.cubeCoord, rl);
        }

        mapGrid.SetBorders();
    }
    
    public static UIHexGridMapCell2 GetCellAt(Vector3 v3) {
        return inst.mapGrid.GetCellAt(v3);
    }

    public static RegionList GetCellDataAt(Vector3 v3) {
        if (!inst.mapGrid.cellDataDict.ContainsKey(v3)) {
            return null;
        }
        return inst.mapGrid.cellDataDict[v3];
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
