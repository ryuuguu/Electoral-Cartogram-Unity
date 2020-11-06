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
    public string displayOnlyDataDirectory = "Assets/DisplayOnlyData/";
    public int imageFrame = -5;
    
    public static UitRegionEditor inst;

    private void Awake() {
        inst = this;
    }

    void Start() {
        if(!GameController.inst.isEditMode) this.gameObject.SetActive(false);
    }
    
    void Update() {
        if (imageFrame + 2 == Time.frameCount) {
            TakeVotesImage();
        } else if (imageFrame + 4 == Time.frameCount) {
            TakeSeatsImage();
        }else if (imageFrame + 6 == Time.frameCount) {
            UitHexGridMap.DisplayOverlay(true);
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
    
    public void TakeVotesImage() {
        UitHexGridMap.DisplayOverlay(false); 
        UitHexGridMap.inst.ShowVotes(true);
        ScreenCapture.CaptureScreenshot(displayOnlyDataDirectory + votesImageName);
    }

    public void TakeSeatsImage() {
        UitHexGridMap.DisplayOverlay(false); 
        UitHexGridMap.inst.ShowVotes(false);
        ScreenCapture.CaptureScreenshot(displayOnlyDataDirectory + seatsImageName);
       
    }

    public static void SaveDisplayOnlyData() {
        WriteDisplayOnlyData(RegionController.inst.regionList,"RegionList.json");
        WriteDisplayOnlyData(PartyController.inst.partyDatas,"PartyDatas.json");
        
        //OtherMapData.json
        //    mapSize for scaling and ratio
        //    radius compared to map size inst.imageFrame = Time.frameCount;
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
