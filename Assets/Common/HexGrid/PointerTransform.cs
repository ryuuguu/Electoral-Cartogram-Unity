using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTransform : MonoBehaviour {
    public GameObject pointerObject;


    public void ShowPointer(Vector3 location,  bool val) {
        pointerObject.SetActive(val);
        pointerObject.transform.localPosition = location;
    }
}
