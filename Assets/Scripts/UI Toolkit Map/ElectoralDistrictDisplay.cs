using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElectoralDistrictDisplay {


    public static RegionList regionList;

    public static Label region_1_name;  // not strings but Labels
    public static Label districtName;

    
    public static VisualElement winnerVE;
    public static ScrollView otherCandHolder; 
    public static VisualElement candidateVE; // method that returns VE?
    
    public static void SetRegionList(RegionList rl) {
        if (!rl.isRiding) return;
        regionList = rl;
        Redraw();
    }

    static VisualElement MakeWinner() {
        var winner = BaseCandidate();
        //adjust children's
        // position and font here
        return winner;
    }

    /// <summary>
    /// Dummy method till correct way is known
    /// </summary>
    /// <returns></returns>
    static VisualElement MakeOtherCandidate() {
        return new VisualElement();
    }
    
    private static VisualElement BaseCandidate() {
        var result = new VisualElement();
        var candidateName = new Label() {name = "CandidateName"};
        result.Add(candidateName);
        var partyColor = new VisualElement() {name = "PartyColor"};
        result.Add(partyColor);
        var partyName = new Label() {name = "PartyName"};
        result.Add(partyName);
        var percentVote = new Label() {name = "PercentVote"};
        result.Add(percentVote);
        return result;
    }


    static void SetCandidateResult(CandidateResult cr, VisualElement ve ) {
        ve.Q<Label>("CandidateName").text = cr.surname + " , " + cr.givenName + " , " + cr.middleName;
        var pd = PartyController.GetPartyData(cr.partyId);
        ve.Q<VisualElement>("PartyColor").style.backgroundColor = pd.color;
        ve.Q<Label>("PartyName").text = LanguageController.ChooseName(pd.names);
        ve.Q<Label>("PercentVote").text = cr.percentVotes.ToString();
    }
    
    
    public static void Redraw() {
        region_1_name.text =LanguageController.ChooseName(regionList.parent.names);
        districtName.text = LanguageController.ChooseName(regionList.names);
        SetCandidateResult(regionList.districtResult.candidateResults[0],winnerVE);
        
        otherCandHolder.Clear(); // will children be GCed?
        
        if (regionList.districtResult.candidateResults.Count > 1) {
            for (int i = 1; i < regionList.districtResult.candidateResults.Count; i++) {
                var cr = MakeOtherCandidate();
                SetCandidateResult(regionList.districtResult.candidateResults[i],cr);
                otherCandHolder.Add(cr);
            }
        }
        
    }

}
