using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Ryuuguu.HexGrid {
    public interface ICoordinate  {

        /// <summary>
        /// Cube Coordinate
        /// </summary>
        Vector3 cube { get; set; }
        
        
        /// <summary>
        /// working A* path costs variables used by CubeCoordinate
        /// </summary>
        float gCost { get; set; }

        /// <summary>
        /// working A* path costs variables used by CubeCoordinate
        /// </summary>
        float hCost { get; set; }

        /// <summary>
        /// working A* path costs variables used by CubeCoordinate
        /// </summary>
        float fCost { get; }

        /// <summary>
        /// Initializes the Coordinate given a cube coordinate and worldSpaceId
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="worldSpaceId"></param>
        void Init(Vector3 cube, string worldSpaceId);

        /// <summary>
        /// Make a new set to hold coordinates
        /// </summary>
        /// <returns>set Id</returns>
        string MakeNewSet();


        void Hide();
        void Show(bool bCollider = true);

        /// <summary>
        /// Destroy this coordinate
        /// should be called by holder not cubeCoordinates directly
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="position"></param>
        void DestroyMe();

    }
}
