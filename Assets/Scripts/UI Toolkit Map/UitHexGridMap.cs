using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.UIElements;

public class UitHexGridMap : MonoBehaviour {
    public UitHexMapGrid mapGrid;
    public MapData mapData;
    public RegionEditor regionEditor;
    public ElectoralDistrictPanel electoralDistrictPanel;
    public Tooltip tooltip;
    public Vector3 mapVEOffset;
    
    protected VisualElement root;

    
    protected VisualElement mapHolder;
    protected VisualElement hexHolder;
    protected VisualElement borderHolder;
    

    private VisualTreeAsset dummy;
    
    private int delayMapBuild = 1;
    private int delayMaplayout = -2;
    private Rect layoutRect= new Rect();

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
        
        mapHolder = new VisualElement();
        root.Add(mapHolder);
        
        //this acts as visual "Layer"
        hexHolder = new VisualElement();
        hexHolder.transform.position = hexHolder.transform.position + mapVEOffset;
        mapHolder.Add(hexHolder);
        mapGrid.Init(hexHolder);
        
        borderHolder = new VisualElement();
        borderHolder.transform.position = borderHolder.transform.position + mapVEOffset;
        mapHolder.Add(borderHolder);
    }

    private void GeometryChange(Rect screenRect) {
        delayMaplayout = 2;
        layoutRect = screenRect;
    }

    private void TopLevelLayout(Rect screenRect) {
        
        ScaleMapHolder(mapHolder, mapGrid.mapSize,
            screenRect.max);

        
        DebugHexPos();
        Debug.LogError(" screenRect.max: " + screenRect.max);

        /*
        var detailsTopRightPos = _mapHolder.transform.matrix.MultiplyPoint(detailsTopRightCorner);
        var rect = new Rect(0, detailsTopRightPos.y,
            detailsTopRightPos.x, screenRect.yMax - detailsTopRightPos.y);
        ScaledAt(_detailsHolder,rect);
        */
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
            Debug.LogError(" mapHolder transform.scale: " +mapHolder.transform.scale); 
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
    private void ScaleMapHolder(VisualElement ve,Vector2 holderSize, Vector2 parentSize) {
        var parentRatio = parentSize.x / parentSize.y;
        var holderRatio = holderSize.x / holderSize.y;
        var scale = 1f; 
        if (holderRatio > parentRatio) {
            scale = parentSize.x/holderSize.x;
        }
        else {
            scale = parentSize.y/holderSize.y;
            //ve.transform.position = new Vector2((parentSize.x - parentSize.x*scale) / 2f, 0);
        }
        ve.transform.scale = scale * Vector3.one;
    }
    
    void Update() {
        if (delayMapBuild == 0) {
            MapBuild();
        }
        delayMapBuild--;
        if (delayMaplayout== 0) {
            TopLevelLayout(layoutRect);
        }
        if (delayMaplayout== -1) {
            DebugHexPos();
        }
        delayMaplayout--;

        // todo: handle mouse
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
       
       
        
        
        Debug.Log("Hacking mapdata here");
        
        
        foreach (var cd in mapData.cellDatas) {
            var rl = RegionController.inst.regionList.Find(cd.regionID);
            if (rl != null) {
                mapGrid.CreateCellRegion(cd.cubeCoord, rl);
            }
            else {
                Debug.Log("Region not found: "+ cd.regionID );
            }
        }
        mapGrid.MakeAllHexes(mapGrid.localSpaceId);
        foreach (UitHex mapCell in mapGrid.hexes[mapGrid.localSpaceId].Values) {
            
            // set border is not going to work on UitHex
            // need to do this separate border class like in UIDocExampleManager
            // mapCell.SetBorder();

        }
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

    public static void ShowDistrictPopup(string id, Vector2 location, string message, Vector3 worldPos) {
        
        inst.tooltip.Show(id, location, message,worldPos);
    }
    
    public static void HideDistrictPopup(string id) {
        inst.tooltip.Hide(id);
    }
}
