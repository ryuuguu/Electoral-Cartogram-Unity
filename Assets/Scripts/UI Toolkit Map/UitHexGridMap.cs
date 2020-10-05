﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.UIElements;

public class UitHexGridMap : MonoBehaviour {
    public UitHexMapGrid mapGrid;
    public UitHexMapBorderGrid uitHexBorderGrid;
    public MapData mapData;
    public RegionEditor regionEditor;
    public ElectoralDistrictPanel electoralDistrictPanel;
    
    public Vector3 mapVEOffset;
    
    protected VisualElement root;
    
    protected VisualElement mapLayer;
    protected VisualElement hexLayer;
    protected VisualElement borderLayer;
    protected VisualElement overlayLayer;
    protected VisualElement detailsLayer;
    
    protected VisualElement ridingInfo;
    
    public static Dictionary<Vector3,UitHexGridMapCell> cellDict = new Dictionary<Vector3, UitHexGridMapCell>(); 
    
    private VisualTreeAsset dummy;
    
    private int delayMapBuild = 1;
    private int delayMaplayout = -2;
    
    bool inRiding = false;
    UIHexGridMapCell prevCell = null;
    UIHexGridMapCell prevSelectedCell = null;

    public static UitHexGridMap inst;

    void Awake() {
        inst = this;
    }

    void Start() {
        // Reference to the root of the window.
        
        var uiDoc= GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        root.RegisterCallback<GeometryChangedEvent>( (evt) =>
            GeometryChange(evt.newRect));

        // Associates a stylesheet to our root. Thanks to inheritance, all root’s
        // children will have access to it.
        root.styleSheets.Add(Resources.Load<StyleSheet>("HexGrid_Style"));

        // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
        var tree = Resources.Load<VisualTreeAsset>("HexGrid_Main");
        tree.CloneTree(root);
        
        mapLayer = new VisualElement();
        // setting size so details top right corner can be calculated
        mapLayer.style.width = mapGrid.mapSize.x;
        mapLayer.style.height = mapGrid.mapSize.x;
        root.Add(mapLayer);
        
        //this acts as visual "Layer"
        hexLayer = new VisualElement();
        
        var holderPosition = hexLayer.transform.position + mapVEOffset;
        hexLayer.transform.position = holderPosition;
        
        
        mapLayer.Add(hexLayer);
        mapGrid.Init(hexLayer);
        
        borderLayer = new VisualElement();
        borderLayer.transform.position = holderPosition;
        mapLayer.Add(borderLayer); 
        uitHexBorderGrid.Init(borderLayer);
        uitHexBorderGrid.SetupHexBorders();
        
        overlayLayer= new VisualElement();
        overlayLayer.RegisterCallback<MouseMoveEvent>(
            e => MouseOver( e));
        overlayLayer.RegisterCallback<MouseDownEvent>(
            e => MouseDown( e.localMousePosition));
        mapLayer.Add(overlayLayer);
        var textElement = UitTooltip.Init();
        overlayLayer.transform.position = hexLayer.transform.position;
        overlayLayer.transform.scale = hexLayer.transform.scale;
        overlayLayer.Add(textElement);
        
        // this is a hack get all mouse events
        var    ve = new TextElement();
        ve.style.position = Position.Absolute;
        ve.style.width = 4000;
        ve.style.height = 4000;
        ve.style.backgroundColor = Color.clear;
        ve.transform.position = new Vector3(-2000,-2000,0);
        overlayLayer.Add(ve);
        
        detailsLayer = new VisualElement();
        root.Add(detailsLayer);
        ridingInfo = new VisualElement();
        ridingInfo.style.position = Position.Absolute;
        ridingInfo.style.width = 1;
        ridingInfo.style.height = 1;
        ridingInfo.style.backgroundColor = Color.black;
        detailsLayer.Add(ridingInfo);
    }

    private void MouseOver(MouseMoveEvent e) {
        
        var cubeCoord = mapGrid.Position2Coord(e.localMousePosition,
            new Vector2(-0.5f,-0.5f));//hack: not centered cell 
        if (cellDict.ContainsKey(cubeCoord)) {
            var regionList = cellDict[cubeCoord].regionList;
            if (regionList.isRiding) {
                var name = LanguageController.ChooseName(regionList.names);
                UitTooltip.Show(e.localMousePosition,e.mousePosition,name);
                return;
                /*
                Debug.Log("mouse: " + e.mousePosition/Screen.safeArea.max + " : " +
                          e.localMousePosition + " : " +
                          name);
                          */
            }
        }
        UitTooltip.Hide();
    }

    
    private void MouseDown(Vector2 localMousePosition) {

        var cubeCoord = mapGrid.Position2Coord(localMousePosition,
            new Vector2(-0.5f,-0.5f));//hack: not centered cell 
        if (cellDict.ContainsKey(cubeCoord)) {
            var regionList = cellDict[cubeCoord].regionList;
            if (regionList.isRiding) {
                var name = LanguageController.ChooseName(regionList.names);
                //send message to detail display panel
            }
            
        }


        /*
        var mouseCoord = mapGrid.Mouse2Coord();
        var cell = GetCellAt(mouseCoord);
        if (cell != null) {
            if (!cell.regionList.isRiding) {
                prevCell= null;
                tooltip.Hide("");
            } else {
                if (cell != prevCell) {
                    tooltip.Show("", Input.mousePosition, LanguageController.ChooseName(cell.regionList.names),
                        Input.mousePosition);
                }
                if (Input.GetMouseButtonDown(0)) {
                    prevSelectedCell?.SetHighLight(false);
                    cell.ButtonPressed();
                    prevSelectedCell = cell;
                }
            }
        }

        prevCell = cell;
        */
    }
    
    private void GeometryChange(Rect screenRect) {
        TopLevelLayout(screenRect);
    }
    
    private void TopLevelLayout(Rect screenRect) {
        
        var scale = ScaleMapHolder(mapLayer, mapGrid.mapSize,
            screenRect.max);

        
       // DebugHexPos();
       // Debug.LogError(" screenRect.max: " + screenRect.max);
       
       // maplayer is mapSize in pixels
       // it is scaled
       
       //we want overlayLayer to be
       // scale same as map Layer?? yes
       // so we know the start pixel width in scaled size
       // it is mapSize.scale(
       // to have box that goes to at most to world pos of detailsTopRightPos
       // 
       
       
        var detailsTopRightPos = mapLayer.transform.matrix.MultiplyPoint(new Vector3(0.3f,0.8f,0));
        var rect = new Rect(0, detailsTopRightPos.y,
            detailsTopRightPos.x, screenRect.yMax - detailsTopRightPos.y);
        ScaledAt(ridingInfo,rect);
        Debug.Log("mapLayer.transform: "+ mapLayer.transform + " : " +mapLayer.transform.scale  
                  + " : " +detailsTopRightPos );
        Debug.Log("ridingInfo: "+ ridingInfo.transform + " : " +ridingInfo.transform.scale  
                  + " : " +detailsTopRightPos );
        
        
    }
    
    private void ScaledAt(VisualElement  ve,Rect rect) {
        ScaledAt(ve,rect.position, rect.size);
    }
    
    private void ScaledAt(VisualElement ve, Vector3 position, Vector3 scale) {
        ve.transform.position = position;
        ve.transform.scale = scale;
    }
    
    
    private void DebugHexPos() {
        if (mapGrid.hexes.Count != 0) {
            var hex1 = mapGrid.hexes[mapGrid.localSpaceId][new Vector3(10,10,-20)];
            
            //Debug.LogError(" Hex1 transfom: " + hex1.transform.position);
            var hex2 = mapGrid.hexes[mapGrid.localSpaceId][new Vector3(11,10,-21)];
            //Debug.LogError(" Hex2 transfom: " + hex2.transform.position);
            var hex3 = mapGrid.hexes[mapGrid.localSpaceId][new Vector3(10,11,-21)];
            //Debug.LogError(" Hex3 transfom: " + hex3.transform.position);
            var hex4 = mapGrid.hexes[mapGrid.localSpaceId][new Vector3(9,11,-20)];
            //Debug.LogError(" Hex4 transfom: " + hex4.transform.position);
            var xLocal =  hex2.transform.position.x - hex4.transform.position.x;
            var yLocal =  hex1.transform.position.y - hex3.transform.position.y;
            var xWorld =  hex2.worldBound.center - hex4.worldBound.center;
            var yWorld =  hex1.worldBound.center - hex3.worldBound.center;
            var worldRatio = (xWorld + yWorld) / (hex1.worldBound.size);
            Debug.LogError("Local : " + xLocal + " : " + yLocal);
            Debug.LogError("World : " + (xWorld + yWorld));
            Debug.LogError(" Hex.worldBound: " + hex1.worldBound);
            Debug.LogError("World ratio  : " 
                           +worldRatio  + (worldRatio.x/worldRatio.y)
                           );
            
            Debug.LogError(" Hex.localBound: " + hex1.localBound);
            Debug.LogError(" Hex.layout: " + hex1.layout);
            
            //Debug.LogError(" Hex transform.scale: " +hex1.transform.scale); 
            Debug.LogError(" mapHolder transform.scale: " +mapLayer.transform.scale); 
            Debug.LogError("ratio : " + xLocal/yLocal + " : "
                           + (xWorld.x/yWorld.y) + " : "
                           + hex1.localBound.width/hex1.localBound.height);
        }
    }
    
    /// <summary>
    /// set scale  of map holder
    /// based on boxRatio
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="boxRatio"></param>
    /// <param name="parentSize"></param>
    private Vector3 ScaleMapHolder(VisualElement ve,Vector2 holderSize, Vector2 parentSize) {
        var parentRatio = parentSize.x / parentSize.y;
        var holderRatio = holderSize.x / holderSize.y;
        var scale = 1f; 
        if (holderRatio > parentRatio) {
            scale = parentSize.x/holderSize.x;
            ve.transform.position = Vector2.zero;
        }
        else {
            scale = parentSize.y/holderSize.y;
            ve.transform.position = new Vector2((parentSize.x - parentSize.x*scale) / 2f, 0);
        }
        ve.transform.scale = scale * Vector3.one;
        return ve.transform.scale;
    }
    
    void Update() {
        
        if (delayMapBuild == 0) {
            MapBuild();
            var votes = hexLayer.Query<VisualElement>(className: UitHexGridMapCell.VOTESClass);
            votes.ForEach(element => element.visible = false);
            var seat = hexLayer.Query<VisualElement>(className:  UitHexGridMapCell.SEATClass);
            seat.ForEach(element => element.visible = true);
        }
        delayMapBuild--;
        
        // todo: handle mouse
        
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
        //todo: add mapGrid.HideVotes(val)
       // mapGrid.HideVotes(val);
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
        
        /*
        Debug.Log(mapGrid);
        Debug.Log(mapGrid.offsetCoord);
        Debug.Log(mapData);
        Debug.Log(mapData.offset);
        Debug.Log(mapData.scale);
        */
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
        //todo: mapCell.SetBorder();
        foreach (var cd in mapData.cellDatas) {
            var rl = RegionController.inst.regionList.Find(cd.regionID);
            if (rl != null) {
                var mapCell = mapGrid.CreateCellRegion(cd.cubeCoord, rl);
                if (mapCell != null) {
                    cellDict[cd.cubeCoord] = mapCell;
                }
            }
            else {
                Debug.Log("Region not found: "+ cd.regionID );
            }
        } 
        mapGrid.MakeAllHexes(mapGrid.localSpaceId);
       
        foreach (var mapCell in cellDict.Values) {
            var colors = mapCell.BorderList();
            uitHexBorderGrid.MakeHexBorders(uitHexBorderGrid.localSpaceId,mapCell.cubeCoord,colors); 
        }
        
    }

    public static UitHexGridMapCell GetRidingCellAt(Vector3 cubeCoord) {
        if (!cellDict.ContainsKey(cubeCoord)) {
            return null;
        }
        return cellDict[cubeCoord];
    }
    
    public static void ClearHighLight() {
        // todo: mapGrid.ClearHighLight()
        //inst.mapGrid.ClearHighLight();
    }

    public static UitHexGridMapCell GetCellAt(Vector3 v3) {
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

}
