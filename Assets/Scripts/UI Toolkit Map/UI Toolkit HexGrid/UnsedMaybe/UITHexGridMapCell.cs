using System;
using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UITHexGridMapCell :MonoBehaviour {
    public RegionList regionList;

    public Texture2D centerRiding; 
    public Texture2D centerOther;  
    public Image targetHighlight;
    public Image[] edges;
    public UitHex uitHex; 
    
    //public UIHexGridOrdered prefabSubGrid;
    //public UIHexGridOrdered subGrid;
    public Transform subGridHolder;
    [FormerlySerializedAs("SubGridSize")] public int subGridSize = 91;
    public int subGridPosition = 7;
    
    // commented well deciding how to sprite equivelent in UI toolkit
    
    public void SetRegion(RegionList aRegionList) {
        regionList = aRegionList;
        if (regionList.isRiding) {
            uitHex.style.backgroundImage = centerRiding;
            var partyId =  regionList.districtResult.candidateResults[0].partyId;
            
            // need to set this style.background color 
            uitHex.style.backgroundColor = PartyController.GetPartyData(partyId).color;
            
        } else {
            
            uitHex.style.backgroundImage = centerOther;
            uitHex.style.backgroundColor = regionList.color;
        }
        
        
    }
   
    
    public void ButtonPressed() {
        SetHighLight(true);
        //TODO convertRegionEditor.SetMapCellActive() -> UIHexGridMapCell
        //RegionEditor.SetMapCellActive(this);
        ElectoralDistrictPanel.SetRegionList(this.regionList);
    }
    
    public void SetHighLight(bool val) {
        targetHighlight.gameObject.SetActive(val);
    }

}
