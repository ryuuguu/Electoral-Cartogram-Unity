using System;
using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UitHexGridMapCell : UitHex {
    
    
    public RegionList regionList;

    public Texture2D centerRiding; 
    public Texture2D centerOther;  
    public bool isSquare = true;
    
    public Image targetHighlight;
    public UitHex uitHex; 
    
    //TODO: port Subgrid
    //public UIHexGridOrdered prefabSubGrid;
    //public UIHexGridOrdered subGrid;
   // public Transform subGridHolder;
   
   // public int subGridPosition = 7;
    
   public VisualElement SetRegion(RegionList aRegionList) {
       VisualElement subGridHolder = null;
       regionList = aRegionList;
       if (regionList.isRiding) {
           uitHex.style.backgroundImage = centerRiding;
           var partyId =  regionList.districtResult.candidateResults[0].partyId;
            
           // need to set this style.background color 
           //uitHex.style.backgroundColor = PartyController.GetPartyData(partyId).color;
           uitHex.style.unityBackgroundImageTintColor = PartyController.GetPartyData(partyId).color;
           //uitHex.style.backgroundColor = Color.clear;
           //should this be a different class or save some attribute so I can filter to find?
           //or just store a ref in dictionary
           
           if ( !GameController.inst.isEditMode) {
               subGridHolder = new VisualElement();
               uitHex.Add(subGridHolder);
               ColorSubGrid(aRegionList, subGridHolder, isSquare);
           }
           
       } else {
            
           uitHex.style.backgroundImage = centerRiding;
           uitHex.style.unityBackgroundImageTintColor = regionList.color;
       }

       return subGridHolder;
   }

   /// <summary>
   /// Make Square sub hexes and return ordered list of subhexes
   /// </summary>
   /// <returns></returns>
   public List<VisualElement> MakeSquareSubHexes() {
       var scale = Vector3.one * 0.1f ;
       
       var result = new List<VisualElement>();
       var pos = Vector3.one;

       //strange "for loop" settings and order are to get correct order
       for (int j = 9; j >= 0; j--) {
            for (int i = 0;i<10;i++) {
                pos = new Vector3(i, j, 1);
               pos.Scale(scale);
               result.Add(MakeSubHex( pos,scale));
           }
       }
       return result;
   }
   
   
   public UitSubHex MakeSubHex( Vector3 location,Vector3 scale) {
       var hex = new UitSubHex();
       hex.EnableInClassList("HexGrid-Hex", true);
       hex.transform.position = location;
       hex.transform.scale = scale ;
       hex.style.backgroundImage = null;
       return hex;
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
    /// <summary>
    /// 
    /// </summary>
    public void  ColorSubGrid(RegionList aRegionList, VisualElement aParent, bool isSquare) {
        int subGridSize = 91;
        List<VisualElement> subHexes = new List<VisualElement>();
        if (isSquare) {
            subGridSize = 100;
            subHexes = MakeSquareSubHexes();
        }
        else {
            subGridSize = 91; //91 for hex
            return; //hex subgrid not implemented yet
        }
        // need total votes 
        // sorted candidates 
        var candidateResults = aRegionList.districtResult.candidateResults;
        
        int childIndex = 0;
        int sumVotes = 0;
        int totalVotes = 0;
        foreach (var cr in candidateResults) {
            totalVotes += cr.votes;
        }
            
        foreach (var cr in candidateResults) {
            sumVotes += cr.votes;
            int maxIndex = Mathf.Min(subGridSize,Mathf.FloorToInt(subGridSize * sumVotes / totalVotes));
            
            var color = PartyController.GetPartyData(cr.partyId).color;
            //Debug.Log("ColorSubGrid: "+ regionList.names[0]+ " " +regionList.id + ":" +cr.partyId + ": " + childIndex + " : " + maxIndex + " : " + color);

            var hexDebug = subHexes[childIndex];
            
            for (; childIndex < maxIndex; childIndex++) {
                var hex = subHexes[childIndex];
                aParent.Add(hex);
                hex.style.backgroundColor= color;
                
            }
            // Debug.Log("Subhex:" + childIndex + " : "  +hexDebug.transform.position + " : " 
            //          + hexDebug.transform.scale + " : " + hexDebug.style.backgroundColor  );
        }
    }

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
