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
            return ConvertAxialToLocalPositionStatic(axial, localSpaceId);
        }
        public static  Vector3 ConvertAxialToLocalPositionStatic(Vector2 axial, string localSpaceId) {
            var ws = localSpaces[localSpaceId];
            float x = axial.x * ws.spacingHorizontal;
            float z = -((axial.x * ws.spacingVertical) + (axial.y * ws.spacingVertical * 2.0f));

            return new Vector3(x,0, z);
        }
        
        
        
        public virtual Vector2 ConvertLocalPositionToAxial(Vector3 wPos, string localSpaceId) {
            return ConvertLocalPositionToAxialStatic(wPos, localSpaceId);
        }

        public static Vector2 ConvertLocalPositionToAxialStatic(Vector3 wPos, string localSpaceId) {
            var ws = localSpaces[localSpaceId];
            float q = (wPos.x * (2.0f / 3.0f)) / ws.coordinateRadius;
            float r = ((-wPos.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * wPos.z)) / ws.coordinateRadius;
            return CubeCoordinates<CoordinateTransform>.RoundAxial(new Vector2(q, r));
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
