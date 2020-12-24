using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Com.Ryuuguu.HexGridCC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

///why is this a uitHex to allow it to be store in some dictionary
/// refactor this
public class UitHexGridMapCell : UitHex {
    public const string SEATClass = "Seat";
    public const string VOTESClass = "Votes";
    
    
    public RegionList regionList;
    public Vector3 cubeCoord;

    //todo: make below static
    //below should be   ===static== 
    public Texture2D centerRiding;
    public Texture2D borderImage;
    public string localSpaceId;
    public float subHexScale;
    //above should be   ===static== 
    
  
    public UitHex uitCell; //why is this hex? to get location and scale correct
    
    // holders are used to make graphic visible or not
    public VisualElement voteHolder;
    public UitSubHex seatHolder;
    public VisualElement borderHolder;
    
    //border stuff
    //move to static  or up to top of mapgrid
    protected float borderOffsetX; //calculated from hexRadius
    protected float borderOffsetY;  //calculated from hexRadius

    protected float hexRadius; //temp  remove later 
    protected float borderScaleFactor = 1.1f; // should be set in map
    
    
   public void SetRegion(RegionList aRegionList, Vector3 aCubeCoord) {
       
       
       regionList = aRegionList;
       cubeCoord = aCubeCoord;
       //this used to be excluded when in editor
       if (regionList.isRiding) {
           // important border must show on top of seat & votes
           // is creating seat here a problem
           seatHolder = MakeSubHex(Vector3.zero, Vector3.one);
           seatHolder.style.backgroundImage = centerRiding;
           seatHolder.AddToClassList(SEATClass);
           uitCell.Add(seatHolder);
           voteHolder = new VisualElement();
           uitCell.Add(voteHolder);
           var partyId =  regionList.districtResult.candidateResults[0].partyId;
           seatHolder.style.unityBackgroundImageTintColor = PartyController.GetPartyData(partyId).color;
           //if ( !GameController.inst.isEditMode ) {
               voteHolder.AddToClassList(VOTESClass);
               uitCell.Add(voteHolder);
               ColorSubGrid(aRegionList, voteHolder,uitCell.transform.scale.x);
           //}
           
       } else {
            
           uitCell.style.backgroundImage = centerRiding;
           uitCell.style.unityBackgroundImageTintColor = regionList.color;
       }
       /*
       //borderholder is assigned last so it is drawn on top 
       borderHolder = new VisualElement();
       uitCell.Add(borderHolder);
       */
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
   
    public List<Color> BorderList () {
        var colors = new List<Color>(){Color.clear,Color.clear,Color.clear,Color.clear,Color.clear,Color.clear,};
        for (int i = 0;i<6;i++) {
            int border = -1;
            var hierarchy = RegionController.inst.regionList.HierarchyList(regionList.id);

            var otherCell = UitHexGridMap.GetRidingCellAt(CubeCoordinates.CubeDirections[i] + cubeCoord);
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
                colors[(i+3)%6] = RegionController.inst.borderColors[border];
                //otherCell.edges[(i+3)%6].color = RegionController.inst.borderColors[border];
                //Debug.Log( "Draw: " + cubeCoord + ":"+i +" : "+ otherCell.cubeCoord + ":"+((i+3)%6) );
            }
           
            // this makes a double wid border against non riding cells 
            // is it needed?
            //otherCell.edges[(i+3)%6].gameObject.SetActive(border >= 0);
            
        }

        return colors;
    }
    
    public void  MakeHexBorder() {
        // is scale needed?
        //Vector3 scale,
        //radius is to vertex not center of an edge
        // offset needs to be distance to center of the edge
        
        var borderList = BorderList();

        MakeHexBorder(borderList);
    }

    public void MakeHexBorder(List<Color> borderList) {
        //move to static
        // or just calc from uitHex width
        hexRadius = uitCell.transform.scale.x;
        Debug.Log("hexRadius: "+ hexRadius);
        borderOffsetX = -1*hexRadius / 2;
        borderOffsetY = hexRadius * Mathf.Cos(Mathf.PI / 3) / 2;
        
        
        var borderCenter = new VisualElement();
        borderHolder.Add(borderCenter);
        borderCenter.transform.position = // this for centering
            new Vector3(hexRadius / 2f, hexRadius / 2f, 0); // I think adjustment is not needed 
        //because it was needed because hexes were a bit oof center
        //*(1- hexScaleFactor/2f); ;


        for (int i = 0; i < 6; i++) {
            if (borderList[i] == Color.clear) continue;
            var border1 = new VisualElement();
            borderCenter.Add(border1);
            border1.transform.rotation = Quaternion.Euler(0, 0, 60 * i);
            border1.transform.position = new Vector3(hexRadius / 2, hexRadius / 2, 0) * borderScaleFactor;
            Debug.Log("HexMap border1.transform.position: " +border1.transform.position);
            var border2 = new VisualElement();
            //border2.transform.scale = scale;

            border2.pickingMode = PickingMode.Ignore; // border will not receive or block mouse clicks
            border1.Add(border2);
            border2.EnableInClassList("HexGrid-Hex-Border", true);
            border2.AddToClassList("Border");
            var image = new Image();
            border2.Add(image);
            border2.style.backgroundImage = borderImage;
            border2.transform.position = new Vector3(borderOffsetX, borderOffsetY, 0) * borderScaleFactor;
            Debug.Log("HexMap border2.transform.position: " +border2.transform.position);
        }
    }


    /// <summary>
    /// Make a square subgrid
    /// </summary>
    public void  ColorSubGrid(RegionList aRegionList, VisualElement aParent, float coordScale) {
        int subGridSize = 91;
        var holder = aParent;
        List<VisualElement> subHexes = new List<VisualElement>();
        
        holder = new VisualElement();
        aParent.Add(holder);
        holder.transform.rotation = Quaternion.Euler(0,0,30);
        // need to shift holder because hex is placed using 
        // top left not center
        holder.transform.position += new Vector3(1, 1, 0) * 0.5f;
        
        //radius 5 gives 91 subhexes
        subHexes = MakeSubHexes(holder, 5, coordScale,subHexScale);
        subGridSize = subHexes.Count;
       
        
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
        coords = ReorderSubhexes(coords, radius);
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

    /// <summary>
    /// reorder list to clockwise inward spiral
    /// </summary>
    /// <param name="coords"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    List<Vector3> ReorderSubhexes(List<Vector3> coords, int radius) {
        var clockwiseDirection = new List<Vector3> {
            new Vector3(1, 0, -1),
            new Vector3(1, -1, 0),
            
            new Vector3(0, -1, 1),
            new Vector3(-1, 0, 1),
            
            
            new Vector3(-1, 1, 0),
            new Vector3(0, 1, -1)
            
        };
        
        var result = new List<Vector3>();
        
        var directions = CubeCoordinates.CubeDirections;
        var prev = Vector3.zero;
        for (int i = radius; i >= 0; i--) {
            
            prev = directions[5] * i;
            //Debug.Log("==i: "+ i + " : "+ prev);
            
            for ( int j = 0 ;j<6;j++) {
                var direction = clockwiseDirection[j];
                //Debug.Log("==j: "+ j + " : "+ direction);
                for (int k = 0; k < i; k++) {
                    prev = prev + direction;
                    result.Add(prev);
                    //Debug.Log("==k: " + k + " : " + prev);
                } 
            }
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
