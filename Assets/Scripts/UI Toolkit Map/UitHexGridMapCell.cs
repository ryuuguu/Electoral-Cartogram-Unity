using System;
using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UitHexGridMapCell : UitHex {
    public RegionList regionList;

    public Texture2D centerRiding; 
    public Texture2D centerOther;  
    public Image targetHighlight;
    public UitHex uitHex; 
    
    //TODO: port Subgrid
    //public UIHexGridOrdered prefabSubGrid;
    //public UIHexGridOrdered subGrid;
   // public Transform subGridHolder;
    // public int subGridSize = 91;
   // public int subGridPosition = 7;
    
   public void SetRegion(RegionList aRegionList) {
       regionList = aRegionList;
       if (regionList.isRiding) {
           uitHex.style.backgroundImage = centerRiding;
           var partyId =  regionList.districtResult.candidateResults[0].partyId;
            
           // need to set this style.background color 
           uitHex.style.backgroundColor = PartyController.GetPartyData(partyId).color;
            
            
           // not doing sub grids yet
           /*
           if (!(prefabSubGrid is null) && !GameController.inst.isEditMode) {
               subGrid = Instantiate(prefabSubGrid,subGridHolder);
               var transform1 = subGrid.transform;
               transform1.localPosition = Vector3.zero;
               transform1.localScale = 0.1f * Vector3.one;
               ColorSubGrid();
           }
           */
       } else {
            
           uitHex.style.backgroundImage = centerOther;
           uitHex.style.backgroundColor = regionList.color;
       }
        
        
   }
    
    //TODO: set borders
    /*
    public void SetBorder() {
        
        for (int i = 0;i<6;i++) {
            int border = -1;
            var hierarchy = RegionController.inst.regionList.HierarchyList(regionList.id);
            var otherCell = UIHexGridMap.GetCellAt(CubeCoordinates.CubeDirections[i] + cubeCoord);
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
    */
    
    
    //TODO: port Subgrid
    /*
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
    */
    
    //TODO: buttons
    /*
    public void ButtonPressed() {
        SetHighLight(true);
        //TODO convertRegionEditor.SetMapCellActive() -> UIHexGridMapCell
        //RegionEditor.SetMapCellActive(this);
        ElectoralDistrictPanel.SetRegionList(this.regionList);
    }
    
    public void SetHighLight(bool val) {
        targetHighlight.gameObject.SetActive(val);
    }
    */
}
