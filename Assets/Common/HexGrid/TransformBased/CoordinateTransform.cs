using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Ryuuguu.HexGrid {
    public class CoordinateTransform : MonoBehaviour, ICoordinate {
        private Vector3 _position = Vector3.zero;
        
        private Vector3 _cube = Vector3.zero;

        public Vector3 cube { get; set; }
        

        private GameObject _outline = null;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;

        public float gCost { get; set; }
        public float hCost { get; set; }

        public float fCost {
            get { return gCost + hCost; }
        }

        /// <summary>
        /// not implemented
        /// all coordinates will be in the same set.
        /// </summary>
        /// <returns> dummy set Id </returns>
        public string MakeNewSet() {
            return "Dummy Set Id";
        }

        public void DestroyMe() {
            Destroy(gameObject);
        }

        // Initializes the Coordinate given a cube coordinate and worldSpaceId
        //still needs ?? to put in container and check container 
        public void Init(Vector3 cube,  string worldSpaceId) {
            Vector3 position = ConvertCubeToWorldPosition(cube, worldSpaceId);
            this._cube = cube;
            transform.localPosition = position;
            transform.parent = groups[worldSpaceId];
            name= "Coordinate: [" + cube.x + "," + cube.y + "," + cube.z + "]";

            HexMeshCreator.Instance.AddToGameObject(this.gameObject, HexMeshCreator.Type.Tile, true);
            _meshRenderer = gameObject.GetComponent<MeshRenderer>();
            _meshCollider = gameObject.GetComponent<MeshCollider>();

            _outline = new GameObject("Outline");
            _outline.transform.parent = transform;

            HexMeshCreator.Instance.AddToGameObject(_outline, HexMeshCreator.Type.Outline, false);

            gameObject.transform.position = _position;
            _outline.transform.position = _position;

            Hide();
            
            
        }

        // Hides the Coordinate
        public void Hide() {
            _meshRenderer.enabled = false;
            _meshCollider.enabled = false;
        }

        // Shows the Coordinate
        public void Show(bool bCollider = true) {
            _meshRenderer.enabled = true;

            if (bCollider)
                _meshCollider.enabled = true;
        }
        
        /// <summary>
        /// Most of the code in the  WorldSpace region is generic
        /// the group... fields are not 
        /// meshinstance is not and it should be moved
        /// when making second coordinate type
        ///   make an abstract Coordinate with generic code and subclass it 
        /// </summary>
        #region WorldSpace
        
        private static int worldSpaceIndex = 0;
        
        static Dictionary<string,Transform> groups = new Dictionary<string, Transform>();
        
        static Dictionary<string, WorldSpace> worldSpaces = new Dictionary<string, WorldSpace>();
        
        public struct WorldSpace {
            public float gameScale;
            public float coordinateRadius;

            public float coordinateWidth;
            public float coordinateHeight;

            public float spacingVertical;
            public float spacingHorizontal;
        }
        
        
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
        public Vector3 ConvertCubeToWorldPosition(Vector3 cube, string worldSpaceId) {
            var ws = worldSpaces[worldSpaceId];
            float x = cube.x * ws.spacingHorizontal;
            float y = 0.0f;
            float z = -((cube.x * ws.spacingVertical) + (cube.z * ws.spacingVertical * 2.0f));

            return new Vector3(x, y, z);
        }

        // Converts a world transform position to the nearest axial coordinate
        public virtual Vector2 ConvertWorldPositionToAxial(Vector3 wPos, string worldSpaceId) {
            var ws = worldSpaces[worldSpaceId];
            float q = (wPos.x * (2.0f / 3.0f)) / ws.coordinateRadius;
            float r = ((-wPos.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * wPos.z)) / ws.coordinateRadius;
            return CubeCoordinates.RoundAxial(new Vector2(q, r));
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
