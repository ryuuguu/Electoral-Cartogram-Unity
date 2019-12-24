using System;
using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIHexGridMapCell2 : HexUI {
    public RegionList regionList;

    public Sprite centerRiding;
    public Sprite centerOther;
    public Image targetHighlight;
    public Image[] edges;
    public UIHexGridOrdered2 prefabSubGrid;
    public UIHexGridOrdered2 subGrid;
    public Transform subGridHolder;
    [FormerlySerializedAs("SubGridSize")] public int subGridSize = 91;
    
    public void SetRegion(RegionList aRegionList) {
        regionList = aRegionList;
        if (regionList.isRiding) {
            center.sprite = centerRiding;
            var partyId =  regionList.districtResult.candidateResults[0].partyId;
            center.color = PartyController.GetPartyData(partyId).color;
            if (!(prefabSubGrid is null) && !GameController.inst.isEditMode) {
                subGrid = Instantiate(prefabSubGrid,subGridHolder);
                var transform1 = subGrid.transform;
                transform1.localPosition = Vector3.zero;
                transform1.localScale = 0.1f * Vector3.one;
                ColorSubGrid();
            }
        } else {
            center.sprite = centerOther;
            center.color = regionList.color;
        }
    }

    public void SetBorder() {
        
        for (int i = 0;i<6;i++) {
            int border = -1;
            //var hierarchy = RegionController.inst.regionList.HierarchyList(regionList.id);
            var hierarchy = regionList.hierarchyList;
            var otherCell = UIHexGridMap2.GetCellAt(CubeCoordinates.CubeDirections[i] + cubeCoord);
            if(otherCell == null) continue;
            //Debug.Log( "Found: " + cubeCoord + ":"+i +" : "+ otherCell.cubeCoord + ":"+((i+3)%6) );
            //var otherHierarchy = RegionController.inst.regionList.HierarchyList(otherCell.regionList.id);
            var otherHierarchy = otherCell.regionList.hierarchyList;
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
            
            int maxIndex = Mathf.Min(subGridSize,Mathf.FloorToInt(subGridSize * sumVotes / totalVotes));
            
           // Debug.Log("ColorSubGrid: "+ regionList.names[0]+ " " +regionList.id + ":" +cr.partyId + ": " + childIndex + " : " + maxIndex );
            var color = PartyController.GetPartyData(cr.partyId).color;
            for (; childIndex < maxIndex; childIndex++) {
              subGrid.orderedCoords[childIndex].center.color = color;
            }
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
