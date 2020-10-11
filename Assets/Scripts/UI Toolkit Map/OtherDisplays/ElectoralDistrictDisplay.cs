using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElectoralDistrictDisplay : MonoBehaviour {
    
    public static VisualElement detailDisplay;

    public static RegionList regionList;
    private static List<CandidateResult> items;
    
    // all these sizes are hard coded as a hack until textMeshPro is supported
    public float regionSize = 40;
    public float regionSizeSmall = 20;
    public int regionLength = 15;
    
    public float ridingSize = 35;
    public float ridingSizeSmall = 17;
    public int ridingLength = 20;
    
    public float winnerSize = 25;
    public float winnerSizeSmall = 15;
    public int winnerLength = 20;

    public float wPartySize = 25;
    public float wPartySizeSmall = 15;
    public int wPartyLength = 15;
    
    
    public static ElectoralDistrictDisplay inst;

    void Awake() {
        inst = this;
    }
    public static void SetRegionList(RegionList rl) {
        regionList = rl;
        if (rl == null || !rl.isRiding || detailDisplay == null) return;
        var name = LanguageController.ChooseName(rl.parent.names);
        var label = detailDisplay.Q<Label>("Region");
        label.text = name;
        Shrink(label,inst.regionSize, inst.regionSizeSmall, inst.regionLength);
        
        name = LanguageController.ChooseName(rl.names);
        label = detailDisplay.Q<Label>("Riding");
        label.text = name;
        Shrink(label,inst.ridingSize, inst.ridingSizeSmall, inst.ridingLength);

        var cr = regionList.districtResult.candidateResults[0];
        
        var winnerVE = detailDisplay.Q<VisualElement>("WinnerRecord");

        name = cr.surname + ", " + cr.givenName;
        if(cr.middleName.Length > 0)  {name  += ", " + cr.middleName;}
        var pd = PartyController.GetPartyData(cr.partyId);
        var partyColor = pd.color;
        var partyName = LanguageController.ChooseName(pd.names);
        var percentVote = cr.percentVotes.ToString();
        
        label = winnerVE.Q<Label>("CandidateName");
        label.text = name;
        Shrink(label,inst.winnerSize, inst.winnerSizeSmall, inst.winnerLength);
        
        var ve = winnerVE.Q<VisualElement>("PartyColor");
        ve.style.backgroundColor = partyColor;
        
        label = winnerVE.Q<Label>("PartyName");
        label.text = partyName;
        Shrink(label,inst.wPartySize, inst.wPartySizeSmall, inst.wPartyLength);
        
        label = winnerVE.Q<Label>("VotePercent");
        label.text = percentVote.ToString();

        items.Clear();
        
        
        // fill list of other candidates
        if (regionList.districtResult.candidateResults.Count > 1) {
            for (int i = 1; i < regionList.districtResult.candidateResults.Count; i++) {
                items.Add( regionList.districtResult.candidateResults[i]);
            }
        }
        detailDisplay.Q<ListView>().Refresh();
        
    }
    
    public void Redaw() {
        SetRegionList(regionList);
    }
    
    public static void Shrink(Label label, float baseSize, float smallSize, int maxSize) {
        label.style.fontSize = baseSize;
        if (label.text.Length > maxSize) {
            label.style.fontSize = smallSize;
        }
    }

    public static VisualElement MakeDetailDisplay() {
        detailDisplay = new VisualElement();
        var treeDetailDisplay = Resources.Load<VisualTreeAsset>("RidingDisplay");
        treeDetailDisplay.CloneTree(detailDisplay);
        
        var listView = detailDisplay.Q<ListView>();
        items = new List<CandidateResult>();
        
        Func<VisualElement> makeItem = () => {
            var result = new VisualElement();
            var otreeDetailDisplay = Resources.Load<VisualTreeAsset>("OtherCandidate");
            otreeDetailDisplay.CloneTree(result);
            return result;
        };
        
        
        Action<VisualElement, int> bindItem = (e, i) => {
            var shrinkFactor = 0.8f;
            var ocr = items[i];
            
            var cname = ocr.surname + ", " + ocr.givenName;
            if(ocr.middleName.Length > 0)  {cname  += ", " + ocr.middleName;}
            var cpd = PartyController.GetPartyData(ocr.partyId);
            var cpartyColor = cpd.color;
            var cpartyName = LanguageController.ChooseName(cpd.names);
            var cpercentVote = ocr.percentVotes.ToString();
        
            var clabel = e.Q<Label>("CandidateName");
            clabel.text = cname;
            Shrink(clabel,inst.winnerSize*shrinkFactor, inst.winnerSizeSmall*shrinkFactor, inst.winnerLength);
        
            e.Q<VisualElement>("PartyColor").style.backgroundColor = cpartyColor;
            
        
            clabel = e.Q<Label>("PartyName");
            clabel.text = cpartyName;
            Shrink(clabel,inst.wPartySize*shrinkFactor, inst.wPartySizeSmall*shrinkFactor, inst.wPartyLength);
        
            clabel = e.Q<Label>("VotePercent");
            clabel.text = cpercentVote.ToString();

        };
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.itemHeight = 30;
        listView.itemsSource = items;
        listView.Refresh();
        return detailDisplay;
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

}
