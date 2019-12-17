using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIEHex : VisualElement
{
    public UIEHex() {
        m_Coord = string.Empty;
    }

    string m_Coord;
    public string coord { get; set; }
    
    public new class UxmlFactory : UxmlFactory<UIEHex, UxmlTraits> {}

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription m_Coord = new UxmlStringAttributeDescription { name = "status" };
        
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
            base.Init(ve, bag, cc);
            ((UIEHex)ve).coord = m_Coord.GetValueFromBag(bag, cc);
        }
    }
    
}