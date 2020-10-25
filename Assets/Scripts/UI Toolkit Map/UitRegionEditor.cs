using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UitRegionEditor : MonoBehaviour {
    public float indent;
    public static RegionList currentRegionList;
    
    //Uit Version

    public Vector3 selectedCoord;
    
    public static UitRegionEditor inst;

    private void Awake() {
        inst = this;
    }

    void Start() {
        if(!GameController.inst.isEditMode) this.gameObject.SetActive(false);
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

    public static void ButtonReloadMap() {
        UitHexGridMap.inst.LoadMakeMap();
    }

    public static void ButtonSaveMap() {
        UitHexGridMap.inst.SaveMapData();
    }
    
    public static void ButtonSetHex() {
        AssignRegion(currentRegionList);
    }

    public static void AssignRegion(RegionList rl) {
         if (rl.isRiding) {
             rl.AssignConstituency(true);
         }
         var regionId = UitHexGridMap.ChangeMapData(inst.selectedCoord, rl);
         var oldRL = RegionController.inst.regionList.Find(regionId);
         if (oldRL != null && oldRL.isRiding && regionId != rl.id) {
             oldRL.AssignConstituency(false);
         }
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
     
}
