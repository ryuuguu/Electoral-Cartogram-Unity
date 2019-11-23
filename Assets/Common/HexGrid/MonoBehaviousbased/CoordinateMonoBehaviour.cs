using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Ryuuguu.HexGrid {
    public class CoordinateMonobehaviour : MonoBehaviour, ICoordinate {
        
        public Vector3 cube { get; set; }
        
        public float gCost { get; set; }
        public float hCost { get; set; }

        public float fCost {
            get { return gCost + hCost; }
        }
        
        public void DestroyMe() {
            Destroy(gameObject);
        }

        // Initializes the Coordinate given a cube coordinate and localSpaceId
        //still needs ?? to put in container and check container 
        public void Init(Vector3 aCube,  string localSpaceId) {
            Vector3 position = ConvertCubeToLocalPosition(aCube, localSpaceId);
            cube = aCube;
            transform.parent = groups[localSpaceId];
            transform.localPosition = position;
            transform.localScale = Vector3.one* localSpaces[localSpaceId].coordinateRadius;
            name= "Coordinate: [" + cube.x + "," + cube.y + "," + cube.z + "]";
            ExtraInit(cube,localSpaceId,position);
        }

        protected virtual void ExtraInit(Vector3 aCube,  string localSpaceId, Vector3 position) {
            //override and place code to do extra initialization such creating visuals
        }
        
        // Hides the Coordinate
        public virtual void Hide() {
            //override to hide hex 
        }

        // Shows the Coordinate
        public virtual void Show() {
            //override to show hex 
        }

        #region LocalSpace
        
        protected  static int localSpaceIndex = 0;
        
        protected static Dictionary<string,Transform> groups = new Dictionary<string, Transform>();
        
        protected static Dictionary<string, LocalSpace> localSpaces = new Dictionary<string, LocalSpace>();
        /// <summary>
        /// A LocalSpace is stores the data needed to calculate position of new hexes
        /// There can be multiple localSpaces for CoordinateTnansforms
        /// </summary>
        [Serializable]
        public class LocalSpace {
            public float gameScale;
            public float coordinateRadius;
            public float coordinateWidth;
            public float coordinateHeight;
            public float spacingVertical;
            public float spacingHorizontal;
        }
        
        /// <summary>
        /// Setup an new localSpace with Scale
        /// </summary>
        /// <param name="gameScale"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static string NewLocalSpaceId(float gameScale, Transform group) {
            var result = localSpaceIndex.ToString();
            localSpaceIndex++;
            localSpaces[result] =new LocalSpace();
            groups[result] = group;
            CalculateCoordinateDimensions(gameScale,localSpaces[result]);
            ExtraLocalSpaceInit(gameScale, group);
            return result;
        }

        public static void ExtraLocalSpaceInit(float gameScale, Transform group) {
            
        }
        
        // Converts an axial coordinate to a local transform position
        public virtual Vector3 ConvertAxialToLocalPosition(Vector2 axial, string localSpaceId) {
            var ws = localSpaces[localSpaceId];
            float x = axial.x * ws.spacingHorizontal;
            float z = -((axial.x * ws.spacingVertical) + (axial.y * ws.spacingVertical * 2.0f));

            return new Vector3(x, 0.0f, z);
        }

        // Converts a cube coordinate to a local transform position
        public Vector3 ConvertCubeToLocalPosition(Vector3 aCube, string localSpaceId) {
            return ConvertAxialToLocalPosition(CubeCoordinates<CoordinateTransform>.ConvertCubetoAxial(aCube), localSpaceId);
        }

        // Converts a local transform position to the nearest axial coordinate
        public virtual Vector2 ConvertLocalPositionToAxial(Vector3 wPos, string localSpaceId) {
            var ws = localSpaces[localSpaceId];
            float q = (wPos.x * (2.0f / 3.0f)) / ws.coordinateRadius;
            float r = ((-wPos.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * wPos.z)) / ws.coordinateRadius;
            return CubeCoordinates<CoordinateTransform>.RoundAxial(new Vector2(q, r));
        }
        
        public Vector3 ConvertLocalPositionToCube(Vector3 wPos, string localSpaceId) {
            return CubeCoordinates<CoordinateTransform>.ConvertAxialtoCube(ConvertLocalPositionToAxial(wPos, localSpaceId));
        }
        
        static public void CalculateCoordinateDimensions(float gameScale,LocalSpace ws) {
            ws.gameScale = gameScale;
            ws.coordinateRadius =  ws.gameScale;

            ws.coordinateWidth = ws.coordinateRadius * 2;
            ws.spacingHorizontal = ws.coordinateWidth * 0.75f;

            ws.coordinateHeight = (Mathf.Sqrt(3) / 2.0f) * ws.coordinateWidth;
            ws.spacingVertical = ws.coordinateHeight / 2.0f;
            
        }
        
        #endregion

    }
}
