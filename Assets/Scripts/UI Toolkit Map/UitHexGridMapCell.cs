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

///why is this a uitHex to allow it to be store in some dictionary
/// refactor this
public class UitHexGridMapCell : UitHex {
    
    
    public RegionList regionList;

    public Texture2D centerRiding; 
    public Texture2D centerOther;  
    public bool isSquare = true;
    public string localSpaceId;
    public float subHexScale;
    
    public Image targetHighlight;
    public UitHex uitCell; //why is this hex? to get location and scale correct
    
    // holders are used to make graphic visible or not
    public VisualElement voteHolder;
    public UitSubHex seatHolder;
    
   public VisualElement SetRegion(RegionList aRegionList) {
       VisualElement subGridHolder = null;
       regionList = aRegionList;
       if (regionList.isRiding) {
           //add seat holder and 
           // should it be sub hex?
           seatHolder = MakeSubHex(Vector3.zero, Vector3.one);
           seatHolder.style.backgroundImage = centerRiding;
           uitCell.Add(seatHolder);
           var partyId =  regionList.districtResult.candidateResults[0].partyId;
            
           // need to set this style.background color 
           //uitHex.style.backgroundColor = PartyController.GetPartyData(partyId).color;
           seatHolder.style.unityBackgroundImageTintColor = PartyController.GetPartyData(partyId).color;
           //uitHex.style.backgroundColor = Color.clear;
           //should this be a different class or save some attribute so I can filter to find?
           //or just store a ref in dictionary
           
           if ( !GameController.inst.isEditMode ) {
               seatHolder.visible = false; //testing hack
               voteHolder = new VisualElement();
               uitCell.Add(voteHolder);
               ColorSubGrid(aRegionList, voteHolder, isSquare,uitCell.transform.scale.x);
           }
           
       } else {
            
           uitCell.style.backgroundImage = centerRiding;
           uitCell.style.unityBackgroundImageTintColor = regionList.color;
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
    public void  ColorSubGrid(RegionList aRegionList, VisualElement aParent, bool isSquare, float coordScale) {
        int subGridSize = 91;
        var holder = aParent;
        List<VisualElement> subHexes = new List<VisualElement>();
        if (isSquare) {
            subGridSize = 100;
            subHexes = MakeSquareSubHexes();
        }
        else {
            holder = new VisualElement();
            aParent.Add(holder);
            holder.transform.rotation = Quaternion.Euler(0,0,30);
            // need to shift holder because hex is placed using 
            // top left not center
            holder.transform.position += new Vector3(1, 1, 0) * 0.5f;
            
            //radius 5 gives 91 subhexes
            subHexes = MakeSubHexes(holder, 5, coordScale,subHexScale);
            subGridSize = subHexes.Count;
           
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
                holder.Add(hex);
                //hex.style.backgroundColor= color;
                hex.style.backgroundImage = centerRiding;
                hex.style.unityBackgroundImageTintColor = color;
                
            }
            // Debug.Log("Subhex:" + childIndex + " : "  +hexDebug.transform.position + " : " 
            //          + hexDebug.transform.scale + " : " + hexDebug.style.backgroundColor  );
        }
    }

        
    public List<VisualElement>  ColorSubGrid( VisualElement aParent, bool isSquare) {
        var holder = new VisualElement();
        aParent.Add(holder);
        holder.transform.rotation = Quaternion.Euler(0,0,30);
        // need to shift holder because hex is placed using 
        // top left not center
        holder.transform.position += new Vector3(1, 1, 0) * 0.5f;
        List<VisualElement> subHexes = new List<VisualElement>();
        //radius 5 gives 91 subhexes
        subHexes = MakeSubHexes(holder, 5, aParent.transform.scale.x,subHexScale);
        
        var color = new Color(0, 1, 1, 1f);
        foreach(var hex in subHexes){
            holder.Add(hex);
            hex.style.backgroundImage = centerRiding;
            hex.style.unityBackgroundImageTintColor = color;
        }

        return subHexes;
    }
    
    public List<Vector3> ConstructMegaHex(int radius) {
        //needs to be re-ordered by rings 
        var result = new List<Vector3>();
        for (int y = -radius; y <= radius; y++) {
            for (int x = -radius; x <= radius; x++) {
            
                for (int z = -radius; z <= radius; z++) {
                    if ((x + y + z) == 0) {
                        result.Add(new Vector3(x, y, z));
                    }
                }
            }
        }
        return result;
    }

    UitSubHex MakeSubHex(Vector3 coord, float hexScale, float coordScale, Vector2 offset) {
        var ls = CubeCoordinates.GetLocalSpace(localSpaceId);
        var location = CubeCoordinates.ConvertPlaneToLocalPosition(coord, ls, offset);
        return MakeSubHex( location*coordScale, Vector3.one *hexScale);
    }

    List<VisualElement> MakeSubHexes(VisualElement parent, int radius, float coordScale,float scaleHex ) {
        var result = new List<VisualElement>();
        var coords = ConstructMegaHex(radius);
        foreach (var coord in coords) {
           var hex =MakeSubHex(coord,1f*scaleHex / (2 * radius + 1f),
               1f /((coordScale )*(2 * radius + 1)),
               new Vector2(0,0));
           // need to shift hexes because sub hex is placed using 
           // top left not center
           hex.transform.position +=  new Vector3(1,1,0) * (-0.5f/(2*radius+1) );
           
           result.Add(hex);
        }
        return result;
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
