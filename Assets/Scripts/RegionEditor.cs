using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionEditor : MonoBehaviour {
    public float indent;
    public RegionEditLine topLine;
    public Text mapRegionName;
    public Text selectedRegionName;
    public RegionList selectedRegionList;
    public bool showAssigned;
    public MapCell mapCell;
    public ScrollRect scrollRect;
    public Transform[] locations;
     
    
    public List<RegionList> debugInvertedRegionLists;


    public static RegionEditor inst;

    private void Awake() {
        inst = this;
      //   (RectTransform) transform;
    }

    void Start() {
        topLine.SetUp(RegionController.inst.regionList, null);
    }

    void Update() {
        if ( mapCell != null && ((transform.localPosition.x > 0 &&  ((RectTransform)mapCell.transform).anchoredPosition.x > 50) ||
            (transform.localPosition.x < 0 && ((RectTransform)mapCell.transform).anchoredPosition.x < -50))) {
            Debug.Log("Loc: " + transform.localPosition);
            var temp = transform.localPosition;
            temp.x *= -1;
            transform.localPosition = temp;
        }

        
        if (Input.GetKeyUp("w") || Input.GetKeyUp(KeyCode.UpArrow)) {
            MoveToMapCell(0);
        }
        if (Input.GetKeyUp("a") || Input.GetKeyUp(KeyCode.LeftArrow)) {
            MoveToMapCell(4);
        }
        if (Input.GetKeyUp("s") || Input.GetKeyUp(KeyCode.DownArrow)) {
            MoveToMapCell(3);
        }
        if (Input.GetKeyUp("d") || Input.GetKeyUp(KeyCode.RightArrow)) {
            MoveToMapCell(1);
        }
        
        
    }

    public void ButtonOK() {
         if (mapCell.regionList.isRiding) {
             mapCell.regionList.isAssigned = false;
         }
         mapCell.SetRegion(selectedRegionList);
         if (mapCell.regionList.isRiding) {
             mapCell.regionList.isAssigned = true;
         }
         Map.SetMapData(mapCell);
         mapRegionName.text = selectedRegionName.text;
         Redraw();
     }

     public void ButtonCancel() {
         if (inst.mapCell.regionList != RegionController.inst.regionList) {
             inst.selectedRegionList = inst.mapCell.regionList;
             inst.Redraw();
         }
     }

     public void SetShowAssigned() {
         Redraw();
     }

     public static void MoveToMapCell(int edgeDirection) {
         var cc = inst.mapCell.cubeCoord ;
         MapCell target = null;
         for (int i = 0; i < MapGrid.edgeDirections.Count; i++) {
             var targetCC = cc + MapGrid.edgeDirections[(edgeDirection+i)%MapGrid.edgeDirections.Count];
             target = Map.inst.mapGrid.GetCellAt(targetCC);
             if (target != null) break;
         }
         if (target != null) {
             Map.ClearHighLight();
             target.SetHighLight(true);
             SetMapCellActive(target);
             
         }
     }
     
     
     public static void SetMapCellActive(MapCell aMapCell) {
         inst.mapCell = aMapCell;
         var i = Mathf.Min(inst.mapCell.regionList.names.Count - 1, LanguageController.CurrentLanguage());
         //Debug.Log("SetMapCellActive: "+ inst.mapCell.regionList.names[i] );
         inst.mapRegionName.text = inst.mapCell.regionList.names[i];
         inst.Redraw();
     }
     
     public void Redraw() {
         var hList =topLine.regionList.HierarchyList(selectedRegionList.id);
         if (hList == null) return;
         RegionEditLine currentRel = null;
         var parent = topLine.transform.parent;
         foreach (var rl in hList) {
             var rels = transform.parent.GetComponentsInChildren<RegionEditLine>();
             for (int i = 0; i < rels.Length; i++) {
                 var rel = rels[i];
                 if (rel.regionList == rl) {
                     currentRel = rel;
                     currentRel.ButtonPressed();
                 }
             }
         }

         SetPos(currentRel);
     }

     public void SetPos(RegionEditLine currentRel) {
         Debug.Log("SetPos: "+ currentRel.regionName);
         var pos = currentRel.transform.localPosition * -1;
         var startPos = scrollRect.content.localPosition;
         startPos.y = pos.y;
         scrollRect.content.localPosition = startPos;
     }
     
}
