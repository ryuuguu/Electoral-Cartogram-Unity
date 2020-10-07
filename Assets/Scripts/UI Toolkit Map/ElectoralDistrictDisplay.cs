using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElectoralDistrictDisplay {


    public static RegionList regionList;

    public static Label region_1_name;  // not strings but Labels
    public static Label districtName;

    public static CandidateRecord winner;
    public static VisualElement otherCandHolder; //scrollview?
    public static CandidateRecord prefabOtherCand; // method that returns VE?
    
    public List<CandidateRecord> otherCandidates;
    

    public static void SetRegionList(RegionList rl) {
        if (!rl.isRiding) return;
        regionList = rl;
        Redraw();
    }
    
    public static void Redraw() {
        region_1_name.text =LanguageController.ChooseName(regionList.parent.names);
        districtName.text = LanguageController.ChooseName(regionList.names);
        winner.SetCandidateResult(regionList.districtResult.candidateResults[0]); 
        
        //destroy tree below
        /*
        foreach (Transform child in otherCandHolder) {
            Destroy(child.gameObject);
        }
        
        otherCandHolder.DetachChildren();
        */
        
        //add item code from example
        /*
        if (regionList.districtResult.candidateResults.Count > 1) {
            for (int i = 1; i < regionList.districtResult.candidateResults.Count; i++) {
                var cr = Instantiate<CandidateRecord>(prefabOtherCand,otherCandHolder,false);
                cr.SetCandidateResult(regionList.districtResult.candidateResults[i]);
            }
        }
        */
    }

}
