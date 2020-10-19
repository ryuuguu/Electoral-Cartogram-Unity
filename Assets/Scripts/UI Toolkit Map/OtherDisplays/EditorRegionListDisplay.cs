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
    public const string VTARegionListDisplay = "EditorRegionListDisplay";
    public const string VTARegionListRecord = "RegionListRecord";

    public struct RegionListRecord {
        public string id;
        public float indent;
        public string[] names;
        public Color color;
        public int constituencyCount;

        public RegionListRecord(RegionList rl) {
            id = rl.id;
            indent = indentIncr* (rl.hierarchyList.Count -1);
            names = rl.names.ToArray();
            color = rl.color;
            constituencyCount = rl.unassignedConstituencyCount;
        }
    }
    
    public static VisualElement regionListDisplay;
    public static float indentIncr = 10;

    public static ListView listView;
    private static List<RegionListRecord> items;
    private static RegionList currentExpandedRegionList;
    
    // all these sizes are hard coded as a hack until textMeshPro is supported

    public float nameSize = 20;
    public float nameSizeSmall = 12;
    public int   nameLength = 15;
    
    public static EditorRegionListDisplay inst;

    void Awake() {
        inst = this;
    }

    public static VisualElement DebugTest() {
        regionListDisplay = MakeRegionListDisplay();
        return regionListDisplay;
    }
    
    
    public static void InitialRegionList() {
        //At first put single record in list
        currentExpandedRegionList = RegionController.inst.regionList;
        //Debug.Log("Top : "+ RegionController.inst.regionList.hierarchyList[0]);
        resetItems();
        
    }

    public static void resetItems() {
        items.Clear();
        //Debug.Log("reset: "+currentExpandedRegionList.id);
        items.Add(new RegionListRecord(RegionController.inst.regionList) );
        foreach (var child in RegionController.inst.regionList.subLists) {
            AddRL( child);
        }
        listView.Refresh();
    }

    public static void AddRL(RegionList rl) {
        //Debug.Log("AddRL: "+ rl.id + " : " + rl.parent.id);
        if (currentExpandedRegionList.hierarchyList.Contains(rl.parent) && !rl.isAssigned) {
            //Debug.Log("AddRL Parent in OK: "+ rl.id + " : " + rl.parent.id);
            items.Add(new RegionListRecord(rl) );
            if (rl.subLists != null) {
                foreach (var child in rl.subLists) {
                    //Debug.Log("AddRL child: " + child.id + " : " + child.parent.id);
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
    }

    public static void Clicked(int itemIndex) {
        var rl = RegionController.Find(items[itemIndex].id);
        if (!rl.isRiding) {
            Debug.Log("Clicked: "+ rl.id + " =============================" );
            currentExpandedRegionList = rl;
            
        }
        else {
            rl.AssignConstituency(true);
            Debug.Log("riding handling not fully implemented");     
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
        

        listView = regionListDisplay.Q<ListView>();
        items = new List<RegionListRecord>();
        
        Func<VisualElement> makeItem = () => {
            var result = new VisualElement();
            var treePartyRecordlDisplay = Resources.Load<VisualTreeAsset>(VTARegionListRecord);
            treePartyRecordlDisplay.CloneTree(result);
            return result;
        };
        
        Action<VisualElement, int> bindItem = (e, i) => {
            var regionRecord = items[i];
            
            var label = e.Q<Label>(VERegionName);
            label.text = LanguageController.ChooseName(regionRecord.names);
            Shrink(label,inst.nameSize, inst.nameSizeSmall, inst.nameLength);
            
            e.Q<VisualElement>(VERegionColor).style.backgroundColor = regionRecord.color;
            var countText = regionRecord.constituencyCount == 0 ? "" : regionRecord.constituencyCount.ToString();
            e.Q<Label>(VEConstituencyCount).text = countText;
            e.Q<VisualElement>(VEIndent).style.width = regionRecord.indent;
            e.Q<Button>().clicked += () => {Clicked(i); };

        };
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.itemHeight = 30;
        listView.itemsSource = items;
        listView.Refresh();
        return regionListDisplay;
    }
    
}
