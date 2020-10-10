﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElectoralDistrictDisplay : MonoBehaviour {
    
    public static VisualElement detailDisplay;
    
    // all these sizes are hard coded as a hack until textMeshPro is supported
    public float regionSize = 35;
    public float regionSizeSmall = 20;

    public static ElectoralDistrictDisplay inst;

    void Awake() {
        inst = this;
    }
    public static void SetRegionList(RegionList rl) {
        if (!rl.isRiding || detailDisplay == null) return;
        var name = LanguageController.ChooseName(rl.parent.names);
        var label = detailDisplay.Query<Label>("Region").First();
        label.text = name;
        Shrink(label,inst.regionSize, inst.regionSizeSmall, name, 10);
        detailDisplay.Query<Label>("Riding").First().text = LanguageController.ChooseName(rl.names);
    }

    public static void Shrink(Label label, float baseSize, float smallSize, string text, int maxSize) {
        label.style.fontSize = baseSize;
        if (text.Length > maxSize) {
            label.style.fontSize = smallSize;
        }
    }

    public static VisualElement MakeDetailDisplay() {
        detailDisplay = new VisualElement();
        var treeDetailDisplay = Resources.Load<VisualTreeAsset>("RidingDisplay");
        treeDetailDisplay.CloneTree(detailDisplay);
        
        
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
