using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class RegionController : MonoBehaviour {

    //public RegionList regionList;
    
    
    public RegionList regionList ;
    public List<Color> borderColors;

    public GameController gameController;
    public string district2016 = "ED-Canada_2016_utf8";
        
    public static RegionController inst;
    
    
    /// <summary>
    /// this should be be made in edit mode and stored
    /// </summary>
    Dictionary<string,RegionList> rlDict = new Dictionary<string, RegionList>();

    private void Awake() {
        inst = this;
       // regionList = regionList.DeepCopy(regionList);
        //regionList = null;
    }

    private void Start() {
        if (GameController.inst.isPreloaded) return;
        LoadRegionData();
    }
    
    [ContextMenu("loadAlldata")]
    public void EditorLoadAlldata() {
        gameController.InEditorSetup();
        LoadRegionData();
    }
    
    public void LoadRegionData() {
        
        ClearRidings();
        LoadElectoralDistricts();
        PrepareRegionListData();
        LoadElectionResults(); 
    }

    public void ClearRidings() {
        ClearSubRidings(regionList);
    }

    /// <summary>
    /// Make hierarchyList for each RegionList
    /// Make rlDict
    /// Set parent
    /// 
    /// </summary>
    public static void PrepareRegionListData() {
        inst.regionList.hierarchyList = new List<RegionList>(){inst.regionList};
        SetInternalLinks(inst.regionList);
    }
    
    public static  void SetInternalLinks(RegionList aRegionList) {
        inst.rlDict[aRegionList.id] = aRegionList;
        if (aRegionList.subLists != null) {
            foreach (var rl in aRegionList.subLists) {
                rl.parent = aRegionList;
                rl.hierarchyList = aRegionList.hierarchyList.ToList();
                rl.hierarchyList.Add(rl);
                SetInternalLinks(rl);
            }
        }
    }

    public static RegionList Find(string anId) {
        if (!inst.rlDict.ContainsKey(anId)) return null;
        return inst.rlDict[anId];
    }
    
    public void ClearSubRidings(RegionList aRegionList) {
        bool emptySublist = false;
        foreach (var rl in aRegionList.subLists) {
            if (rl.isRiding) {
                emptySublist = true;
                break;
            }
            else {
                ClearSubRidings(rl);
            }
        }
        if (emptySublist) {
            aRegionList.subLists.Clear();
        }
    }
    
    public void LoadElectoralDistricts() {
        var sourceFile = (TextAsset) Resources.Load(district2016, typeof(TextAsset));
        var temp = sourceFile.text;
        var strings = CSV.SplitString(temp);

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
                var id = regionCodes[regionCode];
                var parent = regionList.Find(id);
                if (parent != null) {
                    var rl = new RegionList() {
                        id = line[0],
                        names = new List<string>() {line[1], line[2]},
                        population = int.Parse(line[3]),
                        isRiding = true,
                        color = Color.white,
                        parent = parent
                    };
                    parent.subLists.Add(rl);
                }
                else {
                    Debug.Log("region not found: " + id);
                }
            }
        }

    }

    /// <summary>
    /// Load and process election results
    /// assumes that districtResult lists are empty.
    /// </summary>
    public void LoadElectionResults() {
        gameController.partyController.ClearVotes();
        
        var sourceFile = (TextAsset) Resources.Load("EventResults_2019", typeof(TextAsset));
        var temp = sourceFile.text; 
        var strings = CSV.SplitString(temp,new[] { "\t"});

        foreach (var line in strings) {
            //Debug.Log("LoadElectionResults" + line[0]);
            int dummyTest;
            if (!int.TryParse(line[0], out dummyTest)) continue; //there may be extra comments at beginning/end of file
            var candidateResult = new CandidateResult();
            string regionId = line[0];
            candidateResult.regionId = regionId;
            candidateResult.resultType = line[3];
            candidateResult.surname = line[5];
            candidateResult.middleName = line[6];
            candidateResult.givenName = line[7];
            candidateResult.partyId = line[8];
            candidateResult.votes = int.Parse(line[10]);
            gameController.partyController.AddPartyData(candidateResult.partyId, line[9],candidateResult.votes);
            candidateResult.percentVotes = float.Parse(line[11]);
            var aRegionList = Find(regionId);
            if (aRegionList != null) {
                if (aRegionList.districtResult == null) {
                    aRegionList.districtResult = new DistrictResult {
                        regionId = regionId,
                        totalVotes = int.Parse(line[13])
                    };
                }

                aRegionList.districtResult.rawCandidateResults.Add(candidateResult);
            }
            else {
                Debug.Log("Region ot found: "+ regionId);
            }
        }
        ProcessElectionResults(regionList);
        gameController.partyController.TotalPartyData();
    }

    public void ProcessElectionResults(RegionList aRegionList) {
        aRegionList.districtResult.MakeCleanResults();
        if (aRegionList.isRiding) {
            aRegionList.districtResult.candidateResults.Sort((cr1,cr2)=>cr2.votes.CompareTo(cr1.votes));
            if (aRegionList.districtResult.candidateResults.Count > 0) {
                PartyController.AddPartySeat(aRegionList.districtResult.candidateResults[0].partyId);
            }
            
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



[System.Serializable]
public class RegionList {
    public string id;
    public string borderType;
    public Color color;
    public List<string> names;
    public bool isRiding;
    public bool isAssigned;
    public List<RegionList> subLists;
    //causes loop errors with inspector
    [NonSerialized]
    public List<RegionList> hierarchyList;
    public RegionList parent;
    public int population;
    public DistrictResult districtResult ;

    public RegionList Find(string anId) {
        var il = HierarchyList(anId);
        return il?.Last();
    }

    /// <summary>
    /// used to make a deep copy of the region list in the editor to one that does not show in the editor
    /// Editor has problem with deep trees with a larger number of nodes
    /// maybe problem is with hierarchyList
    /// hierarchyList is NOT copied
    /// </summary>
    /// <param name="rl"></param>
    /// <param name="aParent"></param>
    /// <returns></returns>
    public RegionList DeepCopy(RegionList rl, RegionList aParent=null) {
        //first shallow copy
        // then shallow copy sublists
        RegionList result = new RegionList();
        result.id = rl.id;
        result.borderType = rl.borderType;
        result.color = rl.color;
        result.names = rl.names;
        result.isRiding = rl.isRiding;
        result.isRiding = rl.isRiding;
        result.subLists = new List<RegionList>();
        result.hierarchyList = new List<RegionList>();
        result.parent = aParent;
        result.population = rl.population;
        result.districtResult = rl.districtResult;
        foreach (var child in rl.subLists) {
            result.subLists.Add(DeepCopy(child,result));
        }
        return result;
    }
    
    /// <summary>
    /// Makes a HierarchyList from anId
    ///      
    /// </summary>
    /// <param name="anId"></param>
    /// <returns></returns>
    [Obsolete("HierarchyList is deprecated, please use SetHierarchyLists() once then use field hierarchyList instead.")]
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
    public string resultType;
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
    public List<CandidateResult> rawCandidateResults = new List<CandidateResult>();
    public int totalVotes;

    public void MakeCleanResults() {
        candidateResults = new List<CandidateResult>();
        foreach (var cr in rawCandidateResults) {
            var index = candidateResults.FindIndex(data => data.surname == cr.surname  && data.partyId == cr.partyId);
            if (index < 0) {
                candidateResults.Add(cr);
            }
            else {
                if (cr.resultType == "validated")
                    candidateResults[index] = cr;
            }
        }
    }
    
}

