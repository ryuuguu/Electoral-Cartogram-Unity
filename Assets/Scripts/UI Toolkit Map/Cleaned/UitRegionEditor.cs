using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UitRegionEditor : MonoBehaviour {
    public float indent;
    public static RegionList currentRegionList;
    public string votesImageName = "VotesImage.png";
    public string seatsImageName = "SeatsImage.png";
    public string displayOnlyDataDirectory = "Files/DisplayOnlyData/";
    public int imageFrame = -5;
    
    public static UitRegionEditor inst;

    private void Awake() {
        inst = this;
    }

    void Start() {
        if(!GameController.inst.isEditMode) this.gameObject.SetActive(false);
    }
    
    void Update() {
        //since  UI toolkit operates asynchronously
        // this is the easiest way to make sure changes happen between each step
        // when ui toolkit is out of beta there may be a simple API to use instead
        if (imageFrame + 2 == Time.frameCount) {
            TakeVotesImage();
        } else if (imageFrame + 4 == Time.frameCount) {
            TakeSeatsImage();
        }else if (imageFrame + 6 == Time.frameCount) {
            UitHexGridMap.DisplayOverlayAndHexMarker(true);
        }
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
    
    /// <summary>
    /// Screen shot with without overlays
    /// For use in Editor only
    /// if needed outside Editor change capture location 
    /// </summary>
    public void TakeVotesImage() {
        UitHexGridMap.DisplayOverlayAndHexMarker(false); 
        UitHexGridMap.inst.ShowVotes(true);
        ScreenCapture.CaptureScreenshot( "Assets/" + displayOnlyDataDirectory + votesImageName);
    }

    /// <summary>
    /// Screen shot with without overlays
    /// For use in Editor only
    /// if needed outside Editor change capture location 
    /// </summary>
    public void TakeSeatsImage() {
        UitHexGridMap.DisplayOverlayAndHexMarker(false); 
        UitHexGridMap.inst.ShowVotes(false);
        ScreenCapture.CaptureScreenshot("Assets/" + displayOnlyDataDirectory + seatsImageName);
       
    }

    public static void SaveDisplayOnlyData() {
        WriteDisplayOnlyData(RegionController.inst.regionList,"RegionList.json");
        var pdw = new PartyDatasWrapper() {
            partyDatas = PartyController.inst.partyDatas
        };
        WriteDisplayOnlyData(pdw,"PartyDataWrapper.json");
        WriteDisplayOnlyData(UitHexGridMap.inst.mapGrid.mapLayout, "MapLayout.json");
        inst.imageFrame = Time.frameCount;
    }

    private static void WriteDisplayOnlyData(object obj, string fileName) {
        string path = Application.dataPath + "/" + inst.displayOnlyDataDirectory;
        var jsonText = JsonUtility.ToJson(obj);
        if (!System.IO.Directory.Exists(path)) {
            System.IO.Directory.CreateDirectory(path);
            Debug.Log("Create Dir: " + path);
        }

        string fName = path + fileName;
        System.IO.File.WriteAllText(fName, jsonText);
    }
}

/// <summary>
/// Unity JsonUtility does not save bare lists or arrays
/// putting the list in a structure or object is the work around 
/// </summary>
public struct PartyDatasWrapper {
    public List<PartyData> partyDatas;
}