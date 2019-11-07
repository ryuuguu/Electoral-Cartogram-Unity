using System;
using UnityEngine;


public class ExampleBehavior : MonoBehaviour {
    // This will store the string value
    [DropDownList("Cat", "Dog")] public string Animal;


    // Showing a list of loaded scenes
    [DropDownList(typeof(ExamplePropertyDrawersHelper), "methodExample")]
    public string methodExample;

}

public class DropDownList : PropertyAttribute {
    public delegate string[] GetStringList();

    public DropDownList(params string [] list) {
        List = list;
    }

    public DropDownList(Type type, string methodName) {
        var method = type.GetMethod (methodName);
        if (method != null) {
            List = method.Invoke (null, null) as string[];
        } else {
            Debug.LogError ("NO SUCH METHOD " + methodName + " FOR " + type);
        }
    }

    public string[] List;
}