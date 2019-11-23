using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Ryuuguu.HexGrid {
    public class CoordinateUI : CoordinateMonobehaviour, ICoordinate {
        
        public Image  center;
        public Image  outline;
        

        // Initializes the Coordinate given a cube coordinate and worldSpaceId
        //still needs ?? to put in container and check container 
        protected override void ExtraInit(Vector3 aCube,  string worldSpaceId, Vector3 position) {
            
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


        public override Vector3 ConvertAxialToWorldPosition(Vector2 axial, string worldSpaceId) {
            var ws = worldSpaces[worldSpaceId];
            float x = axial.x * ws.spacingHorizontal;
            float y = -((axial.x * ws.spacingVertical) + (axial.y * ws.spacingVertical * 2.0f));

            return new Vector3(x, y,0.0f);
        }
    }
}
