using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Ryuuguu.HexGrid {
    public class CoordinateUI : CoordinateMonobehaviour, ICoordinate {
        
        public Image  center;
        
        protected override void ExtraInit(Vector3 aCube,  string localSpaceId, Vector3 position) {
            Hide();
        }
        
        // Hides the Coordinate
        public override void Hide() {
            center.enabled = false;
        }

        // Shows the Coordinate
        public override void Show() {
            center.enabled = true;
        }
        
        public override Vector3 ConvertAxialToLocalPosition(Vector2 axial, string localSpaceId) {
            var ws = localSpaces[localSpaceId];
            float x = axial.x * ws.spacingHorizontal;
            float y = -((axial.x * ws.spacingVertical) + (axial.y * ws.spacingVertical * 2.0f));
            return new Vector3(x, y,0.0f);
        }
        
        public virtual Vector2 ConvertWorldPositionToAxial(Vector3 wPos, string localSpaceId) {
            var ws = localSpaces[localSpaceId];
            float q = (wPos.x * (2.0f / 3.0f)) / ws.coordinateRadius;
            float r = ((-wPos.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * wPos.z)) / ws.coordinateRadius;
            return CubeCoordinates<CoordinateTransform>.RoundAxial(new Vector2(q, r));
        }
        
        
        
    }
}
