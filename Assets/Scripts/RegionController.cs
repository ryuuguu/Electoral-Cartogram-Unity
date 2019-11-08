using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class RegionController : MonoBehaviour {
    public RegionList regionList;
    public List<Color> borderColors;

    public string election2015;
    public string election2019 = "ED-Canada_2016";
        
    public static RegionController inst;
   // public Dictionary<string, string> partyList;
   
    private void Awake() {
        inst = this;
    }

    private void Start() {
        LoadElectoralDistricts();
        LoadElectionResults();
    }

    public void Convert() {
        // change this to usin unity stuff :)
        FileStream test = new FileStream("YourFile",FileMode.Open,FileAccess.Read);

        byte[] data = new byte[test.Length];
        test.Read(data, 0, (int) test.Length);
        Encoding.Convert(Encoding.ASCII, Encoding.UTF8, data);
        //Do stuff
    }
    
    public void LoadElectoralDistricts() {
        var sourceFile = (TextAsset) Resources.Load(election2019, typeof(TextAsset));
        var strings = CSV.SplitString(sourceFile.text);

        Dictionary<string, string> regionCodes = new Dictionary<string, string>();
        regionCodes["10"] = "NL";
        regionCodes["11"] = "PE";
        regionCodes["12"] = "NS";
        regionCodes["13"] = "NB";
        regionCodes["24"] = "QC";
        regionCodes["35"] = "ON";
        regionCodes["46"] = "MB";
        regionCodes["47"] = "SK";
        regionCodes["48"] = "AB";
        regionCodes["59"] = "BC";
        regionCodes["60"] = "YT";
        regionCodes["61"] = "NT";
        regionCodes["62"] = "NU";

        foreach (var line in strings) {
            if (line.Length < 1 || line[0].Length < 2) continue;
            var regionCode = line[0].Substring(0, 2);
            if (regionCodes.ContainsKey(regionCode)) {
                //Debug.Log(line.Length);
                //Debug.Log(line[0]);
                var id = regionCodes[regionCode];
                //Debug.Log(id);
                var parent = regionList.Find(id);
                //Debug.Log(parent);
                //Debug.Log(line[0] + ":" + id + ":" + parent.id);
                var rl = new RegionList() {
                    id = line[0],
                    names = new List<string>() {line[1], line[2]},
                    population = int.Parse(line[3]),
                    isRiding = true,
                    color = Color.white
                };
                parent.subLists.Add(rl);
            }
        }

    }

    public void LoadElectionResults() {
        PartyController.ClearPartyVotes();
        var sourceFile = (TextAsset) Resources.Load("EventResults_2019", typeof(TextAsset));
        var strings = CSV.SplitString(sourceFile.text,new[] { "\t"});

        foreach (var line in strings) {
            //Debug.Log("LoadElectionResults" + line[0]);
            int dummyTest;
            if (!int.TryParse(line[0], out dummyTest)) continue; //there may be extra comments at beginning/end of file
            var candidateResult = new CandidateResult();
            string regionId = line[0];
            candidateResult.regionId = regionId;
            candidateResult.surname = line[5];
            candidateResult.middleName = line[6];
            candidateResult.givenName = line[7];
            candidateResult.partyId = line[8];
            candidateResult.votes = int.Parse(line[10]);
            PartyController.AddPartyData(candidateResult.partyId, line[9],candidateResult.votes);
            candidateResult.percentVotes = float.Parse(line[11]);
            var aRegionList = regionList.Find(regionId);
            if (aRegionList.districtResult == null) {
                aRegionList.districtResult = new DistrictResult {
                    regionId = regionId,
                    totalVotes = int.Parse(line[13])
                };
            }
            aRegionList.districtResult.candidateResults.Add(candidateResult);
        }
        ProcessElectionResults(regionList);
    }

    public void ProcessElectionResults(RegionList aRegionList) {
        if (aRegionList.isRiding) {
            CandidateResult winner = null;
            int maxVotes = 0;
            foreach (var candidateResult in aRegionList.districtResult.candidateResults) {
                if (candidateResult.votes > maxVotes) {
                    maxVotes = candidateResult.votes;
                    winner = candidateResult;
                }
            }
            aRegionList.districtResult.winningCandidateResult = winner;
            PartyController.AddPartySeat(winner.partyId);
        }
        else {
            if (aRegionList.subLists != null) {
                foreach (var rl in aRegionList.subLists) {
                    ProcessElectionResults(rl);
                }
            }
        }
    }
}


    // base
    //  make map borders 
    //   base colors 
    
    
    //read electionData 
    //  assign seat winners 
    //  add seats to party data
    
    //make basic riding colored map
        //like one on web
    
    // make detailed riding colored map 
        //with proportional vote numbers 


[System.Serializable]
public class RegionList {
    public string id;
    public string borderType;
    public Color color;
    public List<string> names;
    public bool isRiding;
    public bool isAssigned;
    public List<RegionList> subLists;
    public int population;
    public DistrictResult districtResult ;

    public RegionList Find(string anId) {
        var il = HierarchyList(anId);
        return il?.Last();
    }

    public List<RegionList> HierarchyList(string anId) {
        if (anId == id) return new List<RegionList>() {this};
        if (subLists != null) {
            foreach (var rl in subLists) {
                var il = rl.HierarchyList(anId);
                if (il != null) {
                    il.Insert(0, this);
                    return il;
                }
            }
        }

        return null;
    }
}

[System.Serializable]
public class CandidateResult {
    public string partyId;
    public string regionId;
    public string surname;
    public string middleName;
    public string givenName;
    public int votes;
    public float percentVotes;
   
}

[System.Serializable]
public class DistrictResult {
    public string regionId;
    public List<CandidateResult> candidateResults = new List<CandidateResult>();
    public int totalVotes;
    public CandidateResult winningCandidateResult;

}

