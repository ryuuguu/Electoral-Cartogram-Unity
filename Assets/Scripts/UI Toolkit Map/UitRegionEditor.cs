using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UitRegionEditor : MonoBehaviour {
    public float indent;
    public RegionList selectedRegionList;
    
    //Uit Version

    public Vector3 selectedCoord;
    
    
    //will call static EditorRegionListDisplay??
    // maybe just called by it
    //   
    
    
    public static UitRegionEditor inst;

    private void Awake() {
        inst = this;
    }

    void Start() {
        if(!GameController.inst.isEditMode) this.gameObject.SetActive(false);
        
        // is for setting up 
        // EditorRegionListDisplay
        
        //topLine.SetUp(RegionController.inst.regionList, null);
    }

    void Update() {
        /*
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
        */
    }

    public void ButtonOK() {
        /*
        if (!GameController.inst.isEditMode) return;
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
         */
     }

     public void ButtonCancel() {
         /*
         if (!GameController.inst.isEditMode) return;
         if (inst.mapCell.regionList != RegionController.inst.regionList) {
             inst.selectedRegionList = inst.mapCell.regionList;
             inst.Redraw();
         }
         */
     }

     public void SetShowAssigned() {
         Redraw();
     }

     public static void MoveToMapCell(int edgeDirection) {
         /*
         var cc = inst.mapCell.cubeCoord ;
         MapCell target = null;
         for (int i = 0; i < MapGrid.edgeDirections.Count; i++) {
             var targetCC = cc + MapGrid.edgeDirections[(edgeDirection + i) % MapGrid.edgeDirections.Count];
             //check if targerCC is in bounds of map
             //target = Map.inst.mapGrid.GetCellAt(targetCC);
             //if (target != null){
                //SetMapCellActive(targerCC);
                //break;
             // }
         }
         */
     }
         
     
     
     public static void SetMapCellActive(Vector3 coord) {
         HexMarker.Show(true);
         HexMarker.MoveTo(coord);
         inst.selectedCoord = coord;
     }
     
     public void Redraw() {
         /*
         if (!GameController.inst.isEditMode) return;
         var hList =topLine.regionList.hierarchyList;
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
         */
     }

    
     
}
