using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTracker : MonoBehaviour {
   public Vector3 debugPos;
  

   private void Update() {
       RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, Input.mousePosition,null,out  var localPoint);
       debugPos = localPoint;
   }
}
