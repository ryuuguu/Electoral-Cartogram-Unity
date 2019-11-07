using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionEditLine : MonoBehaviour {
    public RegionList regionList;
    public Text regionName;
    public Image colourIcon;
    public Image ridingIcon;
    public Image assignedIcon;
    public RectTransform indentRT;
    public float indent;
    public float incrIndent;
    public int level;
    public RegionEditor regionEditor;
    
    public void SetUp(RegionList aRegionList, RegionEditLine parent) {
        regionList = aRegionList;
        if (regionList.names.Count == 0) {
            regionName.text = "";
        } else {
            var i = Mathf.Min(regionList.names.Count - 1, LanguageController.CurrentLanguage());
            regionName.text = regionList.names[i]; 
        }

        ridingIcon.gameObject.SetActive(regionList.isRiding && !regionList.isAssigned);
        assignedIcon.gameObject.SetActive(regionList.isAssigned);
        colourIcon.gameObject.SetActive(!regionList.isRiding);
        colourIcon.color = regionList.color;
        if (parent != null) {
            indent = parent.indent + parent.incrIndent;
            incrIndent = parent.incrIndent;
            level = parent.level+1;
        }
        indentRT.offsetMin = new Vector2(indent,0);
    }
    
    public void ButtonPressed() {
        regionEditor.selectedRegionName.text = regionName.text;
        regionEditor.selectedRegionList = regionList;
        ClearSiblings();
        AddSubRegions();
    }
    
    public void ClearSiblings() {
        //Debug.Log("ClearSiblings(): " +regionList.id);
        var rels = transform.parent.GetComponentsInChildren<RegionEditLine>();
        for (int i = 0; i < rels.Length; i++) {
            var rel = rels[i];
            if (rel.level >= level && rel != this) {
                Destroy(rel.gameObject);
                rel.transform.SetParent(null,true);
            }
        }
    }
    
    public void AddSubRegions() {
        int rank = transform.GetSiblingIndex();
        if (regionList.subLists != null) {
            foreach (var rl in regionList.subLists) {
                if ((regionEditor.mapCell != null && (regionEditor.mapCell.regionList != rl)) && 
                    !regionEditor.showAssigned && rl.isAssigned) continue;
                var rel = Instantiate(this, transform.parent);
                rel.transform.SetSiblingIndex(++rank);
                rel.SetUp(rl, this);
            }
        }
    }  
    
}
