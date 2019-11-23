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
            outline.enabled = true;
        }

        
    }
}
