using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapCell : HexCell,IPointerEnterHandler,IPointerExitHandler {
    public RegionList regionList;

    public Sprite centerRiding;
    public Sprite centerOther;
    public Image targetHighlight;
    public HexGrid prefabSubGrid;
    public HexGrid subGrid;
    
    public void SetRegion(RegionList aRegionList) {
        regionList = aRegionList;
        if (regionList.isRiding) {
            center.sprite = centerRiding;
            var partyId =  regionList.districtResult.candidateResults[0].partyId;
            center.color = PartyController.GetPartyData(partyId).color;
            if (!(prefabSubGrid is null) && !GameController.inst.isEditMode) {
                subGrid = Instantiate<HexGrid>(prefabSubGrid,transform);
                subGrid.transform.localPosition = Vector3.zero;
                ColorSubGrid();

            }
        } else {
            center.sprite = centerOther;
            center.color = regionList.color;
        }
        
        for (int i = 0;i<6;i++) {
            int border = -1;
            var hierarchy = RegionController.inst.regionList.HierarchyList(regionList.id);
            var otherCell = Map.GetCellAt(MapGrid.edgeDirections[i] + cubeCoord);
            if(otherCell == null) continue;
            //Debug.Log( "Found: " + cubeCoord + ":"+i +" : "+ otherCell.cubeCoord + ":"+((i+3)%6) );
            var otherHierarchy = RegionController.inst.regionList.HierarchyList(otherCell.regionList.id);
            for (int j = 0; j < Mathf.Min(hierarchy.Count, otherHierarchy.Count);j++) {
                if (hierarchy[j] != otherHierarchy[j]) {
                    border = j;
                    break;
                }
            }

            if (border >= 0) {
                edges[i].color = RegionController.inst.borderColors[border];
                otherCell.edges[(i+3)%6].color = RegionController.inst.borderColors[border];
                //Debug.Log( "Draw: " + cubeCoord + ":"+i +" : "+ otherCell.cubeCoord + ":"+((i+3)%6) );
            }
            edges[i].gameObject.SetActive(border >= 0);
            otherCell.edges[(i+3)%6].gameObject.SetActive(border >= 0);
            
        }
    }

    public void  ColorSubGrid() {
        // need total votes 
        // sorted candidates 
        var candidateResults = regionList.districtResult.candidateResults;
        
        int childIndex = 0;
        int sumVotes = 0;
        int totalVotes = 0;
        foreach (var cr in candidateResults) {
            totalVotes += cr.votes;
        }
            
        foreach (var cr in candidateResults) {
            sumVotes += cr.votes;
            
            int maxIndex = Mathf.Min(91,Mathf.FloorToInt(91 * sumVotes / totalVotes));
            
            Debug.Log("ColorSubGrid: "+ regionList.names[0]+ " " +regionList.id + ":" +cr.partyId + ": " + childIndex + " : " + maxIndex );
            var color = PartyController.GetPartyData(cr.partyId).color;
            for (; childIndex < maxIndex; childIndex++) {
                subGrid.cells[childIndex].center.color = color;
            }
        }
        
    }
    
    
    public void ButtonPressed() {
        Map.ClearHighLight();
        SetHighLight(true);
        RegionEditor.SetMapCellActive(this);
        ElectoralDistrictPanel.SetRegionList(this.regionList);
    }

    public void ShowLocationPopup() {
        if (regionList.isRiding) {
            //var i = Mathf.Min(regionList.names.Count - 1, LanguageController.CurrentLanguage());
            Map.ShowDistrictPopup(regionList.id, Input.mousePosition, LanguageController.ChooseName(regionList.names)
                ,transform.position);
        }
    }
    
    public void HideLocationPopup() {
        Map.HideDistrictPopup(regionList.id);
    }

    public void SetHighLight(bool val) {
        targetHighlight.gameObject.SetActive(val);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        ShowLocationPopup();
    }

    public void OnPointerExit(PointerEventData eventData) {
        HideLocationPopup();
    }
}
