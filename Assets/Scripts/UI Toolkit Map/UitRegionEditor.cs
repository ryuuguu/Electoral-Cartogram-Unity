using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UitRegionEditor : MonoBehaviour {
    public float indent;
    public static RegionList currentRegionList;
    
    //Uit Version
    
    public static UitRegionEditor inst;

    private void Awake() {
        inst = this;
    }

    void Start() {
        if(!GameController.inst.isEditMode) this.gameObject.SetActive(false);
    }


    public static void ButtonReloadMap() {
        UitHexGridMap.inst.LoadMakeMap();
        EditorRegionListDisplay.resetItems();
    }

    public static void ButtonSaveMap() {
        UitHexGridMap.inst.SaveMapData();
    }
    
    public static void ButtonSetHex() {
        AssignRegion(currentRegionList);
        EditorRegionListDisplay.setHexResetItems(currentRegionList);
        
    }

    public static void AssignRegion(RegionList rl) {
         if (rl.isRiding) {
             rl.AssignConstituency(true);
         }
         var regionId = UitHexGridMap.ChangeMapData(UitHexGridMap.inst.selectedCoord, rl);
         var oldRL = RegionController.inst.regionList.Find(regionId);
         if (oldRL != null && oldRL.isRiding && regionId != rl.id) {
             oldRL.AssignConstituency(false);
         }
    }
     
    
    
}
