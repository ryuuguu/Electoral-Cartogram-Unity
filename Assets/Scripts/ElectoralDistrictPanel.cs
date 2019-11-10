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

    public void Redraw() {
        
    }

}
