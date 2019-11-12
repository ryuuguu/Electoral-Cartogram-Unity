using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyController : MonoBehaviour {
    public List<PartyData> partyDatas;
    public Color otherPartyColor;

    public static PartyController inst;

    private void Awake() {
        inst = this;
    }

    public void AddPartyData(string nameE, string nameF, int votes) {
        var pd = partyDatas.Find((data => data.partyId == nameE));
        if (pd == null) {
            pd = new PartyData() {
                partyId = nameE,
                names = new List<string>() {nameE, nameF},
                color = inst.otherPartyColor,
                isOther = true
            };
            partyDatas.Add(pd);
        }
        pd.totalVotes += votes;
    }
    
    public void ClearPartyVotes() {
        foreach (var pd in partyDatas) {
            pd.totalVotes = 0;
            pd.totalSeats = 0;
        }
    }

    public static void AddPartySeat(string partyId) {
        var pd = GetPartyData(partyId);
        if (pd != null) {
            pd.totalSeats++;
        }
    }
    public static PartyData GetPartyData(string partyId) {
       return inst.partyDatas.Find(data => data.partyId == partyId);
    }
    
}

[System.Serializable]
public class PartyData {
    public string partyId;
    public List<string> names;
    public Color color;
    public bool isOther;
    public int totalVotes;
    public int totalSeats;
}
