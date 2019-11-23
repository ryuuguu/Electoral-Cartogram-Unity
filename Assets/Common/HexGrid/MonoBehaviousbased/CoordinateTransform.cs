using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Ryuuguu.HexGrid {
    public class CoordinateTransform : CoordinateMonobehaviour, ICoordinate {
        
        public MeshRenderer centerMesh;

        // Initializes the Coordinate given a cube coordinate and localSpaceId
        //still needs ?? to put in container and check container 
        protected override void ExtraInit(Vector3 aCube,  string localSpaceId, Vector3 position) {
            Hide();
        }

        public virtual Vector3 ConvertAxialToLocalPosition(Vector2 axial, string localSpaceId) {
            var ws = localSpaces[localSpaceId];
            float x = axial.x * ws.spacingHorizontal;
            float y = -((axial.x * ws.spacingVertical) + (axial.y * ws.spacingVertical * 2.0f));

            return new Vector3(x, y,0);
        }
        
        
        // Hides the Coordinate
        public override void Hide() {
            centerMesh.enabled = false;
        }

        // Shows the Coordinate
        public override void Show() {
            centerMesh.enabled = true;
            
        }

        
    }
}
