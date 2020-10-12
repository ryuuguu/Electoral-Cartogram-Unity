using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class PartyTotalsDisplay : MonoBehaviour {
    
    public static VisualElement partyDisplay;
    
    private static List<PartyData> items;
    
    // all these sizes are hard coded as a hack until textMeshPro is supported
    

    public float partySize = 20;
    public float partySizeSmall = 12;
    public int   partyLength = 15;
    
    
    public static PartyTotalsDisplay inst;

    void Awake() {
        inst = this;
    }
    public static void SetPartyList() {
        var ordered = PartyController.inst.partyDatas.OrderByDescending(partyData => partyData.totalVotes);
        items.AddRange( ordered);
        
        Debug.Log("SetPartyList: "+ items.Count);
        partyDisplay.Q<ListView>().Refresh();
    }
    
    public void Redraw() {
        partyDisplay.Q<ListView>().Refresh();
    }
    
    public static void Shrink(Label label, float baseSize, float smallSize, int maxSize) {
        label.style.fontSize = baseSize;
        if (label.text.Length > maxSize) {
            label.style.fontSize = smallSize;
        }
    }

    public static VisualElement MakePartyTotalsDisplay() {
        partyDisplay = new VisualElement();
        var treeDetailDisplay = Resources.Load<VisualTreeAsset>("PartyTotals");
        treeDetailDisplay.CloneTree(partyDisplay);
        

        var listView = partyDisplay.Q<ListView>();
        items = new List<PartyData>();
        
        Func<VisualElement> makeItem = () => {
            var result = new VisualElement();
            var treePartyRecordlDisplay = Resources.Load<VisualTreeAsset>("PartyTotalsRecord");
            treePartyRecordlDisplay.CloneTree(result);
            return result;
        };
        
        Action<VisualElement, int> bindItem = (e, i) => {
            var partyData = items[i];
            
            var partyName = LanguageController.ChooseName(partyData.names);
            var partyColor = partyData.color;
            var percentVote = partyData.percentVotes.ToString("F1");
            var seats = partyData.totalSeats.ToString();
            var propSeats = partyData.propSeats.ToString("F1");
            
            var label = e.Q<Label>("PartyName");
            label.text = partyName;
            Shrink(label,inst.partySize, inst.partySizeSmall, inst.partyLength);
            
            e.Q<VisualElement>("PartyColor").style.backgroundColor = partyColor;
            e.Q<Label>("Seats").text = seats;
            e.Q<Label>("PropSeats").text = propSeats;
            e.Q<Label>("PercentVote").text = percentVote;
            
        };
        listView.makeItem = makeItem;
        listView.bindItem = bindItem;
        listView.itemHeight = 30;
        listView.itemsSource = items;
        listView.Refresh();
        return partyDisplay;
    }
    
}
