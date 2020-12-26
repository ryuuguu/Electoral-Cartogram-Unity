using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class UitHexGridMap : MonoBehaviour {
    
    public const string StyleSheetHexGrid = "HexGrid_Style";
    public const string VTAHexGrid = "HexGrid_Main";
    public const string VTATopBar = "TopBar";
    public const string VEToggleVotes = "Votes";
    public const string VEToggleFrench = "Fr";
    public const string VEToggleEdit = "Edit";

    public UitHexMapGrid mapGrid;
    public UitHexMapBorderGrid uitHexBorderGrid;
    public MapData mapData;

    public Vector3 selectedCoord; // current Highlighted & info shown for this coord
    
    protected VisualElement root;
    
    protected VisualElement mapSizerLayer;
    protected VisualElement mapLayer;
    protected VisualElement hexLayer;
    protected VisualElement borderLayer;
    protected VisualElement regionLayer;
    protected VisualElement overlayLayer;
    
    protected VisualElement leftInfo;
    protected VisualElement rightInfo;
    protected VisualElement hexMarker;
    protected VisualElement editorRegionList;

    public static Dictionary<Vector3,UitHexGridMapCell> cellDict = new Dictionary<Vector3, UitHexGridMapCell>();

    protected bool currentVotes;
    
    private int delayMapBuild = 1;
    private int delayMaplayout = -2;
    
    bool inRiding = false;

    public static UitHexGridMap inst;

    void Awake() {
        inst = this;
    }

    void Start() {
        // Reference to the root of the window.

        var uiDoc = GetComponent<UIDocument>();
        root = uiDoc.rootVisualElement;
        root.RegisterCallback<GeometryChangedEvent>((evt) =>
            GeometryChange(evt.newRect));

        // Associates a stylesheet to our root. Thanks to inheritance, all root’s
        // children will have access to it.
        root.styleSheets.Add(Resources.Load<StyleSheet>(StyleSheetHexGrid));

        // Loads and clones our VisualTree (eg. our UXML structure) inside the root.
        var tree = Resources.Load<VisualTreeAsset>(VTAHexGrid );
        tree.CloneTree(root);
        

        mapSizerLayer = new VisualElement();
        mapSizerLayer.style.width = mapGrid.mapLayout.mapSize.x;
        mapSizerLayer.style.height = mapGrid.mapLayout.mapSize.y;
        root.Add(mapSizerLayer);
        
        mapLayer = new VisualElement();
        // setting size so FlexBox works with position drawn map 
        //setting top & bottom left height at 0
        //mapLayer.style.top = Length.Percent(0);
        //mapLayer.style.bottom = Length.Percent(0);
        mapLayer.style.left = Length.Percent(0);
        mapLayer.style.right = Length.Percent(0);
        mapLayer.style.height = mapGrid.mapLayout.mapSize.y;
        mapSizerLayer.Add(mapLayer);

        //this acts as visual "Layer"
        hexLayer = new VisualElement();

        //var holderPosition = hexLayer.transform.position + mapVEOffset;
        //hexLayer.transform.position = holderPosition;

        mapLayer.Add(hexLayer);
        var localSpaceId = mapGrid.Init(hexLayer);
        hexMarker = HexMarker.MakeHexMarker(localSpaceId);

        borderLayer = new VisualElement();
        //borderLayer.transform.position = holderPosition;
        mapLayer.Add(borderLayer);
        uitHexBorderGrid.hexRadius = mapGrid.hexRadius;
        uitHexBorderGrid.Init(borderLayer);
        uitHexBorderGrid.SetupHexBorders();

        var markerLayer = new VisualElement();
        //markerLayer.transform.position = holderPosition;
        mapLayer.Add(markerLayer);
        markerLayer.Add(hexMarker);

        regionLayer = new VisualElement();
        regionLayer.style.position = Position.Absolute;
        //regionLayer.transform.position = hexLayer.transform.position;
        //regionLayer.transform.scale = hexLayer.transform.scale;
        mapLayer.Add(regionLayer);
        RegionLayer.Init(localSpaceId, regionLayer);
        RegionLayer.Redraw();

        overlayLayer = new VisualElement();
        overlayLayer.RegisterCallback<MouseMoveEvent>(
            e => MouseOver(e));
        overlayLayer.RegisterCallback<MouseDownEvent>(
            e => MouseDown(e.localMousePosition));
        mapLayer.Add(overlayLayer);

        overlayLayer.style.position = Position.Absolute;
        overlayLayer.style.height = mapGrid.mapLayout.mapSize.y;
        overlayLayer.style.left = Length.Percent(0);
        overlayLayer.style.right = Length.Percent(0);


        // this is a hack get all mouse events
        var ve = new TextElement();

        ve.style.position = Position.Absolute;
        ve.style.width = 4000;
        ve.style.height = 4000;
        ve.style.backgroundColor = Color.clear;
        ve.transform.position = new Vector3(-2000, -2000, 0);
        overlayLayer.Add(ve);

        var topBar = TopBar();
        //topBar.transform.position = new Vector3(0, 150 - mapGrid.mapSize.y, 0);
        overlayLayer.Add(topBar);

        LeftInfoSetup();

        RightInfoSetUp();

        var detailDisplay = ElectoralDistrictDisplay.MakeDetailDisplay();
        rightInfo.Add(detailDisplay);

        var partyTotalsDisplay = PartyTotalsDisplay.MakePartyTotalsDisplay();

        leftInfo.Add(partyTotalsDisplay);

        
        overlayLayer.Add(leftInfo);
        overlayLayer.Add(rightInfo);

        var toolTip = UitTooltip.Init();
        overlayLayer.Add(toolTip);

        editorRegionList = EditorRegionListDisplay.MakeRegionListDisplay();
        editorRegionList.RegisterCallback<MouseDownEvent>(
            e => e.StopPropagation());
        MoveEditor(editorRegionList, false);
        SetEditMode(false);
        overlayLayer.Add(editorRegionList);
    }

    /// <summary>
    /// RightInfoSetup & LeftInfoSetup
    /// are not generallized.
    /// mapGrid.mapLayout.info2Area & mapGrid.mapLayout.mapSize
    /// could be passed in
    /// and rightInfo returned
    /// and correct rect could be calculated for all cases
    /// </summary>
    private void RightInfoSetUp() {
        rightInfo = new VisualElement();
        rightInfo.style.position = Position.Absolute;
        rightInfo.style.backgroundColor = Color.black;
        var top=100*((mapGrid.mapLayout.info2Area.y)/mapGrid.mapLayout.mapSize.y);
        rightInfo.style.top = Length.Percent(top);
        rightInfo.style.bottom = Length.Percent(0);
        var left=100*((mapGrid.mapLayout.info2Area.x)/mapGrid.mapLayout.mapSize.x);
        rightInfo.style.left = Length.Percent(left);
        rightInfo.style.right = Length.Percent(0);
    }

    /// <summary>
    /// RightInfoSetup & LeftInfoSetup
    /// are not generallized.
    /// mapGrid.mapLayout.info2Area & mapGrid.mapLayout.mapSize
    /// could be passed in
    /// and rightInfo returned
    /// and correct rect could be calculated for all cases
    /// </summary>
    private void LeftInfoSetup() {
        leftInfo = new VisualElement();
        leftInfo.style.position = Position.Absolute;
        var top=100*(mapGrid.mapLayout.info1Area.y/mapGrid.mapLayout.mapSize.y);
        leftInfo.style.top = Length.Percent(top);
        leftInfo.style.bottom = Length.Percent(0);
        leftInfo.style.left = Length.Percent(0);
        var right=100*((mapGrid.mapLayout.mapSize.x- mapGrid.mapLayout.info1Area.xMax)/mapGrid.mapLayout.mapSize.x);
        leftInfo.style.right = Length.Percent(right);
        leftInfo.style.backgroundColor = Color.black;
    }

    private void MoveEditor(VisualElement ve, bool right) {
        ve.style.position = Position.Absolute;
        ve.style.top = Length.Percent(10);
        ve.style.bottom = Length.Percent(10);
        ve.style.left = Length.Percent(10);
        ve.style.right = Length.Percent(60);
        ve.style.backgroundColor = Color.black;
        if (right) {
            ve.style.left = Length.Percent(60);
            ve.style.right = Length.Percent(10);
        }
    }


    public void SetEditMode(bool val) {
        GameController.inst.isEditMode = val;
        if (val) {
            editorRegionList.style.display = DisplayStyle.Flex;
        } else {
            editorRegionList.style.display = DisplayStyle.None;
        }
    }

    public static void DisplayOverlayAndHexMarker(bool val) {
        inst.overlayLayer.style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
        inst.hexMarker.style.display = val ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    private void MouseOver(MouseMoveEvent e) {
        
        var cubeCoord = mapGrid.Position2Coord(e.localMousePosition,
            new Vector2(-0.5f,-0.5f));//hack: not centered cell 
        string msg2 = "";
        if (GameController.inst.isEditMode) {
            msg2 = cubeCoord.ToString() + " inMap: " +
                   CubeCoordinates.InRectXY(cubeCoord, mapGrid.mapCubeRect) 
                + " rect: " + mapGrid.mapCubeRect;
        }

        if (cellDict.ContainsKey(cubeCoord)) {
            var regionList = cellDict[cubeCoord].regionList;
            if (regionList.isRiding) {
                var msg = LanguageController.ChooseName(regionList.names);
                if( GameController.inst.isEditMode){msg += msg2;}
                UitTooltip.Show(e.localMousePosition,e.mousePosition,msg);
            }
            else {
                if (GameController.inst.isEditMode) {
                    UitTooltip.Show(e.localMousePosition, e.mousePosition, msg2);
                }
                else {
                    UitTooltip.Hide(); 
                }
            }
        } else {
            if (GameController.inst.isEditMode) {
                UitTooltip.Show(e.localMousePosition, e.mousePosition, msg2);
            }
            else {
                UitTooltip.Hide();
            }
        }
        
    }
    
    private void MouseDown(Vector2 localMousePosition) {
        var cubeCoord = mapGrid.Position2Coord(localMousePosition,
            new Vector2(-0.5f,-0.5f));//hack: not centered cell
        if(CubeCoordinates.InRectXY(cubeCoord, mapGrid.mapCubeRect)){
            MoveTo(cubeCoord);
        }
    }

    public void MoveTo(Vector3 cubeCoord) {
        selectedCoord = cubeCoord;
        if (cellDict.ContainsKey(cubeCoord)) {
            var regionList = cellDict[cubeCoord].regionList;
            if (regionList.isRiding) {
                ElectoralDistrictDisplay.SetRegionList(regionList);
            }
        }

        HexMarker.MoveTo(cubeCoord);
        if (GameController.inst.isEditMode) {
            // offset is not implemented so result is off by a hex row
            float mapSpaceX = mapGrid.Coord2Position(cubeCoord, Vector2.zero).x / mapGrid.mapLayout.mapSize.x;

            if (mapSpaceX > 0.55f) {
                MoveEditor(editorRegionList, false);
            }

            if (mapSpaceX < 0.45f) {
                MoveEditor(editorRegionList, true);
            }
        }
    }

    private void GeometryChange(Rect screenRect) {
        TopLevelLayout(screenRect);
    }
    
    private void TopLevelLayout(Rect screenRect) {
        var scale = ScaleMapHolder(mapSizerLayer, mapGrid.mapLayout.mapSize,
            screenRect.max);
    }
    
    private void ScaledAt(VisualElement  ve,Rect rect) {
        ScaledAt(ve,rect.position, rect.size);
    }
    
    private void ScaledAt(VisualElement ve, Vector3 position, Vector3 scale) {
        ve.transform.position = position;
        ve.transform.scale = scale;
    }

    /// <summary>
    /// set scale  of map holder
    /// based on boxRatio
    /// </summary>
    /// <param name="ve"></param>
    /// <param name="boxRatio"></param>
    /// <param name="geometryRect"></param>
    private Vector3 ScaleMapHolder(VisualElement ve,Vector2 holderSize, Vector2 geometryRect) {
        var parentRatio = geometryRect.x / geometryRect.y;
        var holderRatio = holderSize.x / holderSize.y;
        var scale = 1f;
        if (holderRatio > parentRatio) {
            scale = geometryRect.x/holderSize.x;
            ve.transform.position = Vector2.zero;
        }
        else {
            scale = geometryRect.y/holderSize.y;
            ve.transform.position = new Vector2((geometryRect.x - holderSize.x*scale) / 2f, 0);
        }
        ve.transform.scale = scale * Vector3.one;
        return ve.transform.scale;
    }
    
    /*
    ///debug code for understanding hex grid to screen positions
    ///may be needed again when generalizing map builder to diffent sizes 
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
    */
    
    void Update() {
        if (delayMapBuild == 0) {
            MapBuild();
            ShowVotes(true);
            MouseDown((inst.mapGrid.mapLayout.mapSize / 2)-(Vector2)overlayLayer.transform.position);
        }
        
        
        if (delayMapBuild == -10) {
            //this delay is for webGL builds
            PartyTotalsDisplay.SetPartyList();
            EditorRegionListDisplay.InitialRegionList();
        }
        
        delayMapBuild--;
        
        if (Input.GetKeyUp("w") || Input.GetKeyUp(KeyCode.UpArrow)) {
            MoveToMapCell(0);
        }
        if (Input.GetKeyUp("e") ) {
            MoveToMapCell(1);
        }
        if (Input.GetKeyUp("x") || Input.GetKeyUp(KeyCode.DownArrow)) {
            MoveToMapCell(3);
        }
        if (Input.GetKeyUp("a") || Input.GetKeyUp(KeyCode.LeftArrow)) {
            MoveToMapCell(5);
        }
        if (Input.GetKeyUp("z") ) {
            MoveToMapCell(4);
        }
        if (Input.GetKeyUp("d") || Input.GetKeyUp(KeyCode.RightArrow)) {
            MoveToMapCell(2);
        }
        
    }

    public  void MoveToMapCell(int edgeDirection) {
        for (int i = 0; i < MapGrid.edgeDirections.Count; i++) {
            var target = selectedCoord + MapGrid.edgeDirections[(edgeDirection + i) % MapGrid.edgeDirections.Count];
            if (CubeCoordinates.InRectXY(target, mapGrid.mapCubeRect)) { 
                selectedCoord = target;
                MoveTo(target);
                break;
            }
        }
    }
    
    public void ShowVotes(bool on) {
        var votes = hexLayer.Query<VisualElement>(className: UitHexGridMapCell.VOTESClass);
        votes.ForEach(element => element.visible = on);
        var seat = hexLayer.Query<VisualElement>(className:  UitHexGridMapCell.SEATClass);
        seat.ForEach(element => element.visible = !on);
        currentVotes = on;
    }
    
    public void MapBuild() {
        LoadMakeMap();
    }
    
    
    public void LoadMakeMap() {
        RegionController.inst.LoadRegionData();
        LoadMapDataResource();
        MakeMapFromData();
        ShowVotes(currentVotes);
    }

    /// <summary>
    /// Remove cells outside mapCubeRect
    /// </summary>
    public void CleanMap() {
        var temp = new List<CellData>();
        foreach (var cd in mapData.cellDatas) {
            if (CubeCoordinates.InRectXY(cd.cubeCoord, mapGrid.mapCubeRect)) {
                temp.Add(cd);
            }
        }
        mapData.cellDatas = temp;
    }
    
    
    /// <summary>
    /// Load Map data from a resource
    ///     works in WebGL
    /// </summary>
    [ContextMenu("Load MapData in edit")]
    public void LoadMapDataResource() {
        var mapDataJSON = Resources.Load<TextAsset>("MapData");
        if (mapDataJSON != null) {
            mapData = JsonUtility.FromJson<MapData>(mapDataJSON.text);
        }
        else {
            Debug.Log("could not load JSON TextAsset resource at MapData");
        }
        
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
    
    
   

    /// <summary>
    /// Change Map data in mapData & cellDict
    /// return previous regionId
    /// </summary>
    /// <param name="cubeCoord"></param>
    /// <param name="rl"></param>
    /// <returns>previous region Id for cell possibly null or ""</returns>
    public static string ChangeMapData(Vector3 cubeCoord, RegionList rl) {
        string result = null;
        var cellData = inst.mapData.cellDatas.Find((data => data.cubeCoord == cubeCoord));
        if (cellData == null) {
            cellData = new CellData() {cubeCoord = cubeCoord, regionID = rl.id};
            inst.mapData.cellDatas.Add(cellData);
        }
        else {
            result = cellData.regionID;
        }
        cellData.regionID = rl.id;
        var mapCell = inst.mapGrid.CreateCellRegion(cubeCoord, rl);
        if (mapCell != null) {
            cellDict[cubeCoord] = mapCell;
        }
        
        inst.uitHexBorderGrid.borderHolder.Clear();
        
        foreach (var mapCellB in cellDict.Values) {
            var colors = mapCellB.BorderList();
            inst.uitHexBorderGrid.MakeHexBorders(inst.uitHexBorderGrid.localSpaceId,mapCellB.cubeCoord,colors); 
        }
        
        Debug.Log("Map data is changed but not saved flag map as unsave"); 
        return result;
    }

    public void ClearMap() {
        //clear hexholder VE
        hexLayer.Clear();
        borderLayer.Clear();
        
        //clear UitHexGrid 
        mapGrid.hexes.Clear();
        
    }
    
    public void MakeMapFromData() {
        ClearMap();
        foreach (var cd in mapData.cellDatas) {
            var rl = RegionController.inst.regionList.Find(cd.regionID);
            if (rl != null) { 
                var mapCell = mapGrid.CreateCellRegion(cd.cubeCoord, rl);
                if (mapCell != null) {
                    cellDict[cd.cubeCoord] = mapCell;
                    if (rl.isRiding) {
                        rl.AssignConstituency(true);
                    }
                }
            }
            else {
                Debug.Log("Region not found: "+ cd.regionID );
            }
        }
        uitHexBorderGrid.borderHolder.Clear();
       
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
    
    public VisualElement TopBar() {
        var topBar = new VisualElement();
        var treeTopBar = Resources.Load<VisualTreeAsset>(VTATopBar);
        treeTopBar.CloneTree(topBar);
        topBar.Q<Toggle>(VEToggleVotes)
            .RegisterCallback<ClickEvent>(evt => 
                ShowVotes(((Toggle) evt.target).value));
        topBar.Q<Toggle>(VEToggleFrench)
            .RegisterCallback<ClickEvent>(evt => LanguageController.Lang_1(((Toggle) evt.target).value));
        topBar.Q<Toggle>(VEToggleEdit)
            .RegisterCallback<ClickEvent>(evt => SetEditMode(((Toggle) evt.target).value));
        return topBar;
    }
    
    [System.Serializable]
    public class MapData {
        public Vector2Int widthRange = Vector2Int.up;
        public Vector2Int heightRange = Vector2Int.up;
        public Vector2 offset = Vector2.zero;
        
        public float scale = 2;
        public float posCellScale = 4.94f;
        public List<CellData> cellDatas;

        public void ScaleCoords(Vector3 scale) {
            foreach (var cd in cellDatas) {
                cd.cubeCoord.Scale(scale);
                
            }
        }
        public void ShiftCoords(Vector3 shift) {
            foreach (var cd in cellDatas) {
                cd.cubeCoord+=shift;
                
            }
        }
    }
    
    [System.Serializable]
    public class CellData {
        public string regionID;
        public Vector3 cubeCoord;
        
    }

}
