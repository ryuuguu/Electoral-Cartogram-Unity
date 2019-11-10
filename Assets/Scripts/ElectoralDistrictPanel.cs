using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElectoralDistrictPanel : MonoBehaviour{
    public RegionList regionList;

    public TMP_Text region_1_name;
    public TMP_Text districtName;

    public CandidateRecord winner;
    public List<CandidateRecord> otherCandidates;


    public static ElectoralDistrictPanel inst;

    private void Awake() {
        inst = this;
    }

    public static void SetRegionList(RegionList rl) {
        if (!rl.isRiding) return;
        inst.regionList = rl;
        inst.Redraw();
    }
    
    public void Redraw() {

        region_1_name.text = " not implemented yet";
        districtName.text = LanguageController.ChooseName(regionList.names);
        winner.SetCandidateResult(regionList.districtResult.candidateResults[0]); //[0] for debugging only
        
    }

}
