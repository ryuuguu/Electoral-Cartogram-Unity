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
    }
    
    public static VisualElement regionListDisplay;
    
    private static List<RegionListRecord> items;
    
    
    
    // all these sizes are hard coded as a hack until textMeshPro is supported

    public float nameSize = 20;
    public float nameSizeSmall = 12;
    public int   nameLength = 15;
    
    public static EditorRegionListDisplay inst;

    void Awake() {
        inst = this;
    }

    public static VisualElement DebugTest() {
        var result = MakeRegionListDisplay();
        SetRegionList();
        return result;
    }
    
    
    public static void SetRegionList() {
        //At first put single record in list
        
        var ordered = new List<RegionListRecord>();
         ordered.Add(   new RegionListRecord() {
             id = "first",
             indent = 10,
             color = Color.blue,
             constituencyCount = 3,
             names = new []{"Canada", "Canada2"}
        });
        items.AddRange( ordered);
        regionListDisplay.Q<ListView>().Refresh();
    }
    
    
    /// <summary>
    /// Redraw is used after changing Language
    /// </summary>
    public void Redraw() {
        regionListDisplay.Q<ListView>().Refresh();
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
        

        var listView = regionListDisplay.Q<ListView>();
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
            //Shrink(label,inst.nameSize, inst.nameSizeSmall, inst.nameLength);
            
            e.Q<VisualElement>(VERegionColor).style.backgroundColor = regionRecord.color;
            e.Q<Label>(VEConstituencyCount).text = regionRecord.constituencyCount.ToString();
            e.Q<VisualElement>(VEIndent).style.width = regionRecord.indent;
            
        };
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.itemHeight = 30;
        listView.itemsSource = items;
        listView.Refresh();
        return regionListDisplay;
    }
    
}
