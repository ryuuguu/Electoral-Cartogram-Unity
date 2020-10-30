using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class EditorRegionListDisplay : MonoBehaviour {
    public const string VEIndent = "Indent";
    public const string VERegionName = "RegionName";
    public const string VERegionColor = "RegionColor";
    public const string VEConstituencyCount = "ConstituencyCount";
    public const string VECurrentRegion = "CurrentRegion";
    public const string VESetHex = "SetHex";
    public const string VEReloadMap = "ReloadMap";
    public const string VESaveMap = "SaveMap";
    
    public const string VTARegionListDisplay = "EditorRegionListDisplay";
    public const string VTARegionListRecord = "RegionListRecord";

    public struct RegionListRecord {
        public string id;
        public float indent;
        public string[] names;
        public bool isAssignable;
        public Color color;
        public int constituencyCount;

        public RegionListRecord(RegionList rl) {
            id = rl.id;
            indent = indentIncr* (rl.hierarchyList.Count -1);
            names = rl.names.ToArray();
            isAssignable = rl.isAssignable;
            color = rl.color;
            constituencyCount = rl.unassignedConstituencyCount;
        }
    }
    
    public static VisualElement regionListDisplay;
    public static Button setHexButton;
    public static float indentIncr = 10;

    public static ListView listView;
    private static List<RegionListRecord> items;
    private static RegionList currentExpandedRegionList;
    private static int selectedIndex;
    public static VisualElement currentRegionDisplay;
    
    // all these sizes are hard coded as a hack until textMeshPro is supported

    public float nameSize = 20;
    public float nameSizeSmall = 12;
    public int   nameLength = 15;
    
    public static EditorRegionListDisplay inst;

    void Awake() {
        inst = this;
    }
    
    public static void InitialRegionList() {
        currentExpandedRegionList = RegionController.inst.regionList;
        selectedIndex = 0;
        resetItems();
    }

    public static void resetItems() {
        items.Clear();
        items.Add(new RegionListRecord(RegionController.inst.regionList) );
        foreach (var child in RegionController.inst.regionList.subLists) {
            AddRL( child);
        }
        listView.Refresh();
    }

    public static void setHexResetItems(RegionList rl) {
        items.Clear();
        items.Add(new RegionListRecord(RegionController.inst.regionList) );
        foreach (var child in RegionController.inst.regionList.subLists) {
            AddRL( child);
        }
        listView.Refresh();
    
        if (items.Count <= selectedIndex) {
            selectedIndex = items.Count - 1;
            Clicked(selectedIndex);
        }
        else {
            if (rl.isRiding) {
                if (RegionController.Find(items[selectedIndex].id).isRiding) {
                    Clicked(selectedIndex);
                } else {
                    Clicked(selectedIndex-1);
                }
            }
        }
    }
    
    public static void AddRL(RegionList rl) {
        if (currentExpandedRegionList.hierarchyList.Contains(rl.parent) && !rl.isAssigned) {
            items.Add(new RegionListRecord(rl) );
            if (rl.subLists != null) {
                foreach (var child in rl.subLists) {
                    AddRL(child);
                }
            }
        }
    }

    
    /// <summary>
    /// Redraw is used after changing Language
    /// </summary>
    public void Redraw() {
        listView.Refresh();
        if (UitRegionEditor.currentRegionList != null) {
            var rlr=  items.Find((record =>  UitRegionEditor.currentRegionList.id == record.id));
            bindRegionRecord(currentRegionDisplay, rlr);
        }
    }

    public static void Clicked( int index) {
        selectedIndex = index;
        var rlr = items[index];
        var rl = RegionController.Find(rlr.id);
        if (!rl.isRiding) {
            currentExpandedRegionList = rl;
        }

        if (rl.isAssignable) {
            UitRegionEditor.currentRegionList = rl;
            selectedIndex = index;
            bindRegionRecord(currentRegionDisplay, rlr);
            setHexButton.SetEnabled(true);
        }
        else {
            UitRegionEditor.currentRegionList = null;
            currentRegionDisplay.Clear();
            setHexButton.SetEnabled(false);
        }
        resetItems();
    }
    
    public static void Shrink(Label label, float baseSize, float smallSize, int maxSize) {
        label.style.fontSize = baseSize;
        if (label.text.Length > maxSize) {
            label.style.fontSize = smallSize;
        }
    }
    
    public static VisualElement MakeRegionListDisplay() {
        regionListDisplay = new VisualElement();
        var treeDetailDisplay = Resources.Load<VisualTreeAsset>(VTARegionListDisplay);
        treeDetailDisplay.CloneTree(regionListDisplay);
        
        currentRegionDisplay = regionListDisplay.Q(VECurrentRegion);
        
        setHexButton = regionListDisplay.Q<Button>(VESetHex);
        setHexButton.clicked += UitRegionEditor.ButtonSetHex;
        setHexButton.SetEnabled(false);
        
        regionListDisplay.Q<Button>(VEReloadMap).clicked += UitRegionEditor.ButtonReloadMap;
        regionListDisplay.Q<Button>(VESaveMap).clicked += UitRegionEditor.ButtonSaveMap;

        
        listView = regionListDisplay.Q<ListView>();
        items = new List<RegionListRecord>();
        
        Func<VisualElement> makeItem = () => {
            var result = new VisualElement();
            return result;
        };
        
        Action<VisualElement, int> bindItem = (e, i) => {
            var regionRecord = items[i];
            // it is not possible to change e.Q<Button>().clicked because it is a action & event
            // it can only be added to
            // whole button must be cleared and recreated
            bindRegionRecord(e, regionRecord);
            e.Q<VisualElement>(VEIndent).style.width = regionRecord.indent;
            e.Q<Button>().clicked += () => {Clicked( i); };
        };
        
        /*
         // not using listview selection of change because they do not block triple clicks
        void OnSelectionChange(IEnumerable<object> x) {
            Debug.Log("listView selected: " + listView.selectedIndex);
            Clicked(items[listView.selectedIndex].id);
        }
        listView.onSelectionChange += OnSelectionChange;
        listView.onItemsChosen += OnSelectionChange;
        */
        
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.itemHeight = 30;
        listView.itemsSource = items;
        
        listView.Refresh();
        return regionListDisplay;
    }

    private static void bindRegionRecord(VisualElement e, RegionListRecord regionRecord) {
        e.Clear();
        var treePartyRecordlDisplay = Resources.Load<VisualTreeAsset>(VTARegionListRecord);
        treePartyRecordlDisplay.CloneTree(e);
        var label = e.Q<Label>(VERegionName);
        label.text = LanguageController.ChooseName(regionRecord.names);
        Shrink(label, inst.nameSize, inst.nameSizeSmall, inst.nameLength);
        var rcVE = e.Q<VisualElement>(VERegionColor);
        if (regionRecord.isAssignable) {
            rcVE.style.backgroundColor = regionRecord.color;
            rcVE.visible = true;
        }
        else {
            rcVE.visible = false;
        }

        var countText = regionRecord.constituencyCount == 0 ? "" : regionRecord.constituencyCount.ToString();
        e.Q<Label>(VEConstituencyCount).text = countText;
    }
}
