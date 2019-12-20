using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIHexGridMapCell : HexUI,IPointerEnterHandler,IPointerExitHandler {
    public RegionList regionList;

    public Sprite centerRiding;
    public Sprite centerOther;
    public Image targetHighlight;
    public UIHexGridOrdered prefabSubGrid;
    public UIHexGridOrdered subGrid;
    [FormerlySerializedAs("SubGridSize")] public int subGridSize = 91;
    
    public void SetRegion(RegionList aRegionList) {
        regionList = aRegionList;
        if (regionList.isRiding) {
            center.sprite = centerRiding;
            var partyId =  regionList.districtResult.candidateResults[0].partyId;
            center.color = PartyController.GetPartyData(partyId).color;
            if (!(prefabSubGrid is null) && !GameController.inst.isEditMode) {
                subGrid = Instantiate(prefabSubGrid,transform);
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
        /*
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
        */
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
        Map.ClearHighLight();
        SetHighLight(true);
        //TODO convertRegionEditor.SetMapCellActive() -> UIHexGridMapCell
        //RegionEditor.SetMapCellActive(this);
        ElectoralDistrictPanel.SetRegionList(this.regionList);
    }

    public void ShowLocationPopup() {
        /*
        var debugRT = transform.parent.GetComponent < RectTransform>();
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            debugRT, Input.mousePosition,
            null ,
            out pos);

        var debugLoc =(Vector3) HexGrid.GetCubeCoord(pos, 2f*2f*Map.inst.mapGrid.posCellScale);
        var debugLoc2 = Map.inst.mapGrid.InvertCellPosition(this, pos);
        debugLoc2.z = -1 * (debugLoc2.x + debugLoc2.y);
        debugLoc.z = -1 * (debugLoc.x + debugLoc.y);
        if (regionList.isRiding) {
            //var i = Mathf.Min(regionList.names.Count - 1, LanguageController.CurrentLanguage());
            Map.ShowDistrictPopup(regionList.id, Input.mousePosition, LanguageController.ChooseName(regionList.names)
                + " : " + cubeCoord + " calc: " + (cubeCoord -debugLoc) + " ms: " + pos + " : " + (cubeCoord -debugLoc2)
                ,transform.position);
        }
        */
    }
    
    public void HideLocationPopup() {
        //Map.HideDistrictPopup(regionList.id);
    }

    public void SetHighLight(bool val) {
        //targetHighlight.gameObject.SetActive(val);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        //ShowLocationPopup();
    }

    public void OnPointerExit(PointerEventData eventData) {
        //HideLocationPopup();
    }
}
