using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Ryuuguu.HexGrid {
    public class CoordinateTransform : CoordinateMonobehaviour, ICoordinate {

        
        protected GameObject _outline = null;
        public MeshRenderer _meshRenderer;
        public MeshCollider _meshCollider;

        public MeshRenderer centerMesh;
        public MeshRenderer outlineMesh;

        // Initializes the Coordinate given a cube coordinate and worldSpaceId
        //still needs ?? to put in container and check container 
        protected override void ExtraInit(Vector3 aCube,  string worldSpaceId, Vector3 position) {
            
            HexMeshCreator.Instance.AddToGameObject(gameObject, HexMeshCreator.Type.Tile, true);
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshCollider = GetComponent<MeshCollider>();

            _outline = new GameObject("Outline");
            _outline.transform.parent = transform;

            HexMeshCreator.Instance.AddToGameObject(_outline, HexMeshCreator.Type.Outline, false);


            _outline.transform.position = position;

            Hide();
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
