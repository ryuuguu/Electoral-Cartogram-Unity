using System;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DropDownList))]
public class DropDownListDrawer : PropertyDrawer {
  
  public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
    var stringInList = attribute as DropDownList;
    var list = stringInList.List;
    if (property.propertyType == SerializedPropertyType.String) {
      if (list.Length != 0) {
        int index = Mathf.Max(0, Array.IndexOf(list, property.stringValue));
        index = EditorGUI.Popup(position, property.displayName, index, list);
        property.stringValue = list[index];
      }
      else {
        property.stringValue = "";
      }
    } else if (property.propertyType == SerializedPropertyType.Integer) {
      property.intValue = EditorGUI.Popup (position, property.displayName, property.intValue, list);
    } else {
      base.OnGUI (position, property, label);
    }
  }
}

