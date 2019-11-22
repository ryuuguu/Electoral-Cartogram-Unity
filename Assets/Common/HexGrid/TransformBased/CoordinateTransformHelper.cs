using System.Collections;
using System.Collections.Generic;
using Com.Ryuuguu.HexGrid;
using UnityEngine;

namespace Com.Ryuuguu.HexGrid {
    
    public class CoordinateTransformHelper : ICoordinateHelper {

        public CoordinateTransform prefab;
        
        public Object NewCoordinate() {

           return  Object.Instantiate<CoordinateTransform>(prefab);
        }
    }
}
