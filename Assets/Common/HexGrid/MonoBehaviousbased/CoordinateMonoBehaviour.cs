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

        // Initializes the Coordinate given a cube coordinate and worldSpaceId
        //still needs ?? to put in container and check container 
        public void Init(Vector3 aCube,  string worldSpaceId) {
            Vector3 position = ConvertCubeToWorldPosition(aCube, worldSpaceId);
            cube = aCube;
            transform.parent = groups[worldSpaceId];
            transform.localPosition = position;
            name= "Coordinate: [" + cube.x + "," + cube.y + "," + cube.z + "]";

            ExtraInit(cube,worldSpaceId,position);
        }

        protected virtual void ExtraInit(Vector3 aCube,  string worldSpaceId, Vector3 position) {
            
            //override and place code to do extra initialization such creating visuals
        }

        
        // Hides the Coordinate
        public virtual void Hide() {
            //override to hide hex 
        }

        // Shows the Coordinate
        public virtual void Show(bool bCollider = true) {
            //override to show hex 
        }

        #region WorldSpace
        
        protected  static int worldSpaceIndex = 0;
        
        protected static Dictionary<string,Transform> groups = new Dictionary<string, Transform>();
        
        protected static Dictionary<string, WorldSpace> worldSpaces = new Dictionary<string, WorldSpace>();
        /// <summary>
        /// A WorldSpace is stores the data needed to calculate position of new hexes
        /// There can be multiple worldSpaces for CoordinateTnansforms
        /// </summary>
        [Serializable]
        public class WorldSpace {
            public float gameScale;
            public float coordinateRadius;
            public float coordinateWidth;
            public float coordinateHeight;
            public float spacingVertical;
            public float spacingHorizontal;
        }
        
        /// <summary>
        /// Setup an new worldSpace with Scale
        /// </summary>
        /// <param name="gameScale"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static string NewWorldSpaceId(float gameScale, Transform group) {
            var result = worldSpaceIndex.ToString();
            worldSpaceIndex++;
            worldSpaces[result] =new WorldSpace();
            groups[result] = group;
            CalculateCoordinateDimensions(gameScale,worldSpaces[result]);
            return result;
        }
        
        // Converts an axial coordinate to a world transform position
        public Vector3 ConvertAxialToWorldPosition(Vector2 axial, string worldSpaceId) {
            var ws = worldSpaces[worldSpaceId];
            float x = axial.x * ws.spacingHorizontal;
            float z = -((axial.x * ws.spacingVertical) + (axial.y * ws.spacingVertical * 2.0f));

            return new Vector3(x, 0.0f, z);
        }

        // Converts a cube coordinate to a world transform position
        public Vector3 ConvertCubeToWorldPosition(Vector3 aCube, string worldSpaceId) {
            var ws = worldSpaces[worldSpaceId];
           
            float x = aCube.x * ws.spacingHorizontal;
            float y = 0.0f;
            float z = -((aCube.x * ws.spacingVertical) + (aCube.z * ws.spacingVertical * 2.0f));
            return new Vector3(x, y, z);
        }

        // Converts a world transform position to the nearest axial coordinate
        public virtual Vector2 ConvertWorldPositionToAxial(Vector3 wPos, string worldSpaceId) {
            var ws = worldSpaces[worldSpaceId];
            float q = (wPos.x * (2.0f / 3.0f)) / ws.coordinateRadius;
            float r = ((-wPos.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * wPos.z)) / ws.coordinateRadius;
            return CubeCoordinates<CoordinateTransform>.RoundAxial(new Vector2(q, r));
        }

       
        static public void CalculateCoordinateDimensions(float gameScale,WorldSpace ws) {
            ws.coordinateRadius =  gameScale;

            ws.coordinateWidth = ws.coordinateRadius * 2;
            ws.spacingHorizontal = ws.coordinateWidth * 0.75f;

            ws.coordinateHeight = (Mathf.Sqrt(3) / 2.0f) * ws.coordinateWidth;
            ws.spacingVertical = ws.coordinateHeight / 2.0f;
            HexMeshCreator.Instance.SetRadius(ws.coordinateRadius); //MeshCreator should not be a singleton 
        }
        
        
        #endregion
    }
}
