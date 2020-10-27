using UnityEngine;
using System.Collections.Generic;
using static System.Single;
using System.Linq;


namespace Com.Ryuuguu.HexGridCC {
    /// <summary>
    /// A coordinates in a single instance can be compared to each other
    /// Local spaces are static 
    /// </summary>
    public class CubeCoordinates
    {
        protected Dictionary<string, Dictionary<Vector3, Coord>> _coordinateContainers =
            new Dictionary<string, Dictionary<Vector3, Coord>>();

        public const string AllContainer = "ALL";
        
        //counter clockwise
        public static readonly Vector3[] CubeDirections = {
            new Vector3(0,1,-1),
            new Vector3(1,0,-1),
            new Vector3(1,-1,0),
            new Vector3(0,-1,1),
            new Vector3(-1,0,1),
            new Vector3(-1,1,0)
            
        };

        public static readonly Vector3[] CubeDiagonalDirections = {
            new Vector3(2.0f, -1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, -2.0f),
            new Vector3(-1.0f, 2.0f, -1.0f),
            new Vector3(-2.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, -1.0f, 2.0f),
            new Vector3(1.0f, -2.0f, 1.0f)
        };



        public static readonly Coord EmptyCoord = new Coord();
        /// <summary>
        /// uses cubeCoord.isNotEmpty == false for not assign Coords
        ///    used like null for a class
        /// </summary>
        public struct Coord {
            public bool isNotEmpty;
            public Vector3 cubeCoord;
            public float fCost;  //used for a*  path
            public float gCost;  //used for a*  path
            public float hCost;  //used for a*  path

            public Coord(Vector3 cube) {
                isNotEmpty = true;
                cubeCoord = cube;
                fCost = 0;  //used for a*  path
                gCost= 0;  //used for a*  path
                hCost = 0; 
            }
            
        }
        
        
        // Constructs new set of coordinates from 0,0,0 given a radius
        public List<Coord> Construct(int radius) {
            Clear();
            //group.transform.parent = holder;
            for (int x = -radius; x <= radius; x++) {
                for (int y = -radius; y <= radius; y++) {
                    for (int z = -radius; z <= radius; z++) {
                        if ((x + y + z) == 0) {
                            AddCube(new Vector3(x, y, z));
                        }
                    }
                }
            }
            return GetCoordinatesFromContainer(AllContainer);
        }

        // Destroys all coordinates and entries
        public void Clear() {
            ClearAllCoordinateContainers();
        }

        // Creates a Coordinate GameObject for a given cube coordinate
        public Coord AddCube(Vector3 cube) {

            return  GetAddCoordinateFromContainer(cube, AllContainer);
            
        }

        // Creates a set of Coordinate GameObjects for a given list of cube coordinates
        public List<Coord> AddCubes(List<Vector3> cubes) {
            var result = new List<Coord>();
            foreach (Vector3 cube in cubes)
                result.Add(AddCube(cube));
            return result;
        }

        // Removes and destroys a Coordinate for a given cube coordinate
        public void RemoveCube(Vector3 cube) {
            var coordinate = GetAddCoordinateFromContainer(cube, AllContainer);
            if (IsNaN(coordinate.cubeCoord.x))
                return;
            RemoveCoordinateFromAllContainers(coordinate);
        }

        // Removes and destroys a set of Coordinates for a given list of cube coordinates
        public void RemoveCubes(List<Vector3> cubes) {
            foreach (Vector3 cube in cubes)
                RemoveCube(cube);
        }

        // Converts a cube coordinate to an axial coordinate
        public static Vector2 ConvertCubetoAxial(Vector3 cube) {
            return new Vector2(cube.x, cube.z);
        }

        // Converts an axial coordinate to a cube coordinate
        public static Vector3 ConvertAxialtoCube(Vector2 axial) {
            return new Vector3(axial.x, (-axial.x - axial.y), axial.y);
        }

        
        // Rounds a given Vector2 to the nearest axial coordinate
        public static Vector2 RoundAxial(Vector2 axial) {
            return RoundCube(ConvertAxialtoCube(axial));
        }

        // Rounds a given Vector3 to the nearest cube coordinate
        public static Vector3 RoundCube(Vector3 coord) {
            float rx = Mathf.Round(coord.x);
            float ry = Mathf.Round(coord.y);
            float rz = Mathf.Round(coord.z);

            float x_diff = Mathf.Abs(rx - coord.x);
            float y_diff = Mathf.Abs(ry - coord.y);
            float z_diff = Mathf.Abs(rz - coord.z);

            if (x_diff > y_diff && x_diff > z_diff)
                rx = -ry - rz;
            else if (y_diff > z_diff)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new Vector3(rx, ry, rz);
        }

        // Return a cube vector for a given direction index
        public Vector3 GetCubeDirection(int direction) {
            return CubeDirections[direction];
        }

        // Returns an cube diagonal vector for a given direction index
        public Vector3 GetCubeDiagonalDirection(int direction) {
            return CubeDiagonalDirections[direction];
        }

        // Returns the neighboring cube coordinate for a given direction index at a coordinate distance of 1
        public Vector3 GetNeighborCube(Vector3 cube, int direction) {
            return GetNeighborCube(cube, direction, 1);
        }

        // Returns the neighboring cube coordinate for a given direction index at a given coordinate distance
        public Vector3 GetNeighborCube(Vector3 cube, int direction, int distance) {
            return cube + (GetCubeDirection(direction) * (float) distance);
        }

        // Returns all neighboring cube coordinates at a coordinate distance of 1
        public List<Vector3> GetNeighborCubes(Vector3 cube) {
            return GetNeighborCubes(cube, 1);
        }

        // Returns all neighboring cube coordinates inclusively up to a given coordinate distance
        public List<Vector3> GetNeighborCubes(Vector3 cube, int radius, bool cleanResults = true) {
            List<Vector3> cubes = new List<Vector3>();

            for (int x = (int) (cube.x - radius); x <= (int) (cube.x + radius); x++)
            for (int y = (int) (cube.y - radius); y <= (int) (cube.y + radius); y++)
            for (int z = (int) (cube.z - radius); z <= (int) (cube.z + radius); z++)
                if ((x + y + z) == 0)
                    cubes.Add(new Vector3(x, y, z));

            cubes.Remove(cube);

            if (cleanResults)
                return CleanCubeResults(cubes);
            return cubes;
        }

        // Returns a neighboring diagonal cube coordinate for a given direction index at a coordinate distance of 1
        public Vector3 GetDiagonalNeighborCube(Vector3 cube, int direction) {
            return cube + GetCubeDiagonalDirection(direction);
        }

        // Returns a neighboring diagonal cube coordinate for a given direction index at a given coordinate distance
        public Vector3 GetDiagonalNeighborCube(Vector3 cube, int direction, int distance) {
            return cube + (GetCubeDiagonalDirection(direction) * (float) distance);
        }

        // Returns all neighboring diagonal cube coordinates at a coordinate distance of 1
        public List<Vector3> GetDiagonalNeighborCubes(Vector3 cube) {
            return GetDiagonalNeighborCubes(cube, 1);
        }

        // Returns all neighboring diagonal cube coordinates at a given coordinate distance
        public List<Vector3> GetDiagonalNeighborCubes(Vector3 cube, int distance, bool cleanResults = true) {
            List<Vector3> cubes = new List<Vector3>();
            for (int i = 0; i < 6; i++)
                cubes.Add(GetDiagonalNeighborCube(cube, i, distance));
            if (cleanResults)
                return CleanCubeResults(cubes);
            return cubes;
        }

        /// <summary>
        /// returns true if cube is inside rectangle rect
        /// not generalized for orientation of grid
        /// </summary>
        /// <param name="cube">cubeCoordinate</param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool InRectXY(Vector3 cube, Rect rect) {
            return (cube.x >= rect.x 
                    && cube.x < rect.xMax 
                    && cube.y >= (-cube.x / 2f) + rect.y 
                    && cube.y < rect.yMax + (-cube.x / 2f));
        }
        
        // Boolean combines two lists of cube coordinates
        public List<Vector3> BooleanCombineCubes(List<Vector3> a, List<Vector3> b) {
            List<Vector3> vec = a;
            foreach (Vector3 vb in b)
                if (!a.Contains(vb))
                    a.Add(vb);
            return vec;
        }

        // Boolean differences two lists of cube coordinates
        public List<Vector3> BooleanDifferenceCubes(List<Vector3> a, List<Vector3> b) {
            List<Vector3> vec = a;
            foreach (Vector3 vb in b)
                if (a.Contains(vb))
                    a.Remove(vb);
            return vec;
        }

        // Boolean intersects two lists of cube coordinates
        public List<Vector3> BooleanIntersectionCubes(List<Vector3> a, List<Vector3> b) {
            List<Vector3> vec = new List<Vector3>();
            foreach (Vector3 va in a)
            foreach (Vector3 vb in b)
                if (va == vb)
                    vec.Add(va);
            return vec;
        }

        // Boolean excludes two lists of cube coordinates ?? is this right
        public List<Vector3> BooleanExclusionCubes(List<Vector3> a, List<Vector3> b) {
            List<Vector3> vec = new List<Vector3>();
            foreach (Vector3 va in a)
            foreach (Vector3 vb in b)
                if (va != vb) {
                    vec.Add(va);
                    vec.Add(vb);
                }

            return vec;
        }

        // Rotates a cube coordinate right by one coordinate  ?? is this right
        public Vector4 RotateCubeCoordinatesRight(Vector3 cube) {
            return new Vector3(-cube.z, -cube.x, -cube.y);
        }

        // Rotates a cube coordinate left by one coordinate ?? is this right
        public Vector4 RotateCubeCoordinatesLeft(Vector3 cube) {
            return new Vector3(-cube.y, -cube.z, -cube.x);
        }

        // Calculates the distance between two cube coordinates
        public float GetDistanceBetweenTwoCubes(Vector3 a, Vector3 b) {
            return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
        }

        // Calculates the lerp interpolation between two cube coordinates given a value between 0.0f and 1.0f
        public Vector3 GetLerpBetweenTwoCubes(Vector3 a, Vector3 b, float t) {
            Vector3 cube = new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );

            return cube;
        }

        // Returns the cube coordinate a given distance interval between two cube coordinates
        public Vector3 GetPointOnLineBetweenTwoCubes(Vector3 a, Vector3 b, int distance) {
            float cubeDistance = GetDistanceBetweenTwoCubes(a, b);
            return RoundCube(GetLerpBetweenTwoCubes(a, b, ((1.0f / cubeDistance) * distance)));
        }

        // Returns a list representing the line of cube coordinates between two given cube coordinates (inclusive)
        public List<Vector3> GetLineBetweenTwoCubes(Vector3 a, Vector3 b, bool cleanResults = true) {
            List<Vector3> cubes = new List<Vector3>();
            float cubeDistance = GetDistanceBetweenTwoCubes(a, b);
            for (int i = 0; i <= cubeDistance; i++)
                cubes.Add(RoundCube(GetLerpBetweenTwoCubes(a, b, ((1.0f / cubeDistance) * i))));
            cubes.Add(a);
            if (cleanResults)
                return CleanCubeResults(cubes);
            return cubes;
        }

        // Returns a list of validated neighboring cube coordinates at a evaluation distance of 1
        public List<Vector3> GetReachableCubes(Vector3 cube, bool cleanResults = true) {
            List<Vector3> cubes = new List<Vector3>();

            var originCoordinate = GetAddCoordinateFromContainer(cube, AllContainer);
            var currentCoordinate = EmptyCoord;
            Vector3 currentCube = cube;

            for (int i = 0; i < 6; i++) {
                currentCube = GetNeighborCube(cube, i);
                currentCoordinate = GetCoordinateFromContainer(currentCube, AllContainer);

                if (currentCoordinate.isNotEmpty)
                    cubes.Add(currentCube);
            }

            if (cleanResults)
                return CleanCubeResults(cubes);
            return cubes;
        }

        // Returns a list of validated neighboring cube coordinates at a given evaluation radius
        public List<Vector3> GetReachableCubes(Vector3 cube, int radius, bool cleanResults = true) {
            if (radius == 1)
                return GetReachableCubes(cube);

            List<Vector3> visited = new List<Vector3>();
            visited.Add(cube);

            List<List<Vector3>> fringes = new List<List<Vector3>>();
            fringes.Add(new List<Vector3>());
            fringes[0].Add(cube);

            for (int i = 1; i <= radius; i++) {
                fringes.Add(new List<Vector3>());
                foreach (Vector3 v in fringes[i - 1]) {
                    foreach (Vector3 n in GetNeighborCubes(v)) {
                        if (!visited.Contains(n)) {
                            visited.Add(n);
                            fringes[i].Add(n);
                        }
                    }
                }
            }

            if (cleanResults)
                return CleanCubeResults(visited);
            return visited;
        }

        // Returns an ordered list of cube coordinates following a spiral pattern around a given cube coordinate at a given coordinate distance
        public List<Vector3> GetSpiralCubes(Vector3 cube, int radius, bool cleanResutls = true) {
            List<Vector3> vec = new List<Vector3>();
            Vector4 current = cube + GetCubeDirection(4) * (float) radius;

            for (int i = 0; i < 6; i++){
                for (int j = 0; j < radius; j++) {
                 vec.Add(current);
                 current = GetNeighborCube(current, i);
                }
            }

            if (cleanResutls)
                return CleanCubeResults(vec);
            return vec;
        }

        // Returns an ordered list of cube coordinates following a spiral pattern around a given cube coordinate up to a given coordinate distance (inclusive)
        public List<Vector3> GetMultiSpiralCubes(Vector3 cube, int radius) {
            List<Vector3> cubes = new List<Vector3>();
            cubes.Add(cube);
            for (int i = 0; i <= radius; i++)
                foreach (Vector4 r in GetSpiralCubes(cube, i))
                    cubes.Add(r);
            return cubes;
        }

        // Returns an ordered list of cube coordinates following the A* path results between two given cube coordinates
        public List<Vector3> GetPathBetweenTwoCubes(Vector3 origin, Vector3 target, string container = AllContainer) {
            if (origin == target)
                return new List<Vector3>();

            List<Vector3> openSet = new List<Vector3>();
            List<Vector3> closedSet = new List<Vector3>();
            openSet.Add(origin);

            Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
            cameFrom.Add(origin, Vector3.zero);

            Vector3 current;

            while (openSet.Count > 0) {
                current = openSet[0];
                var currentCoordinate = GetAddCoordinateFromContainer(current, container);

                Coord coordinate;
                for (int i = 1; i < openSet.Count; i++) {
                    coordinate = GetAddCoordinateFromContainer(openSet[i], container);
                    if (coordinate.fCost < currentCoordinate.fCost || coordinate.fCost == currentCoordinate.fCost &&
                        coordinate.hCost < currentCoordinate.hCost) {
                        current = openSet[i];
                        currentCoordinate = GetAddCoordinateFromContainer(current, container);
                    }
                }

                openSet.Remove(current);
                closedSet.Add(current);

                if (current == target)
                    break;

                var neighbors = GetReachableCubes(current);

                foreach (Vector3 neighbor in neighbors) {
                    coordinate = GetAddCoordinateFromContainer(neighbor, container);
                    if (!coordinate.isNotEmpty || closedSet.Contains(neighbor))
                        continue;

                    var newCost = currentCoordinate.gCost + GetDistanceBetweenTwoCubes(current, neighbor);
                    var neighborCoordinate = GetAddCoordinateFromContainer(neighbor, container);

                    if (newCost < neighborCoordinate.gCost || !openSet.Contains(neighbor)) {
                        neighborCoordinate.gCost = newCost;
                        neighborCoordinate.hCost = GetDistanceBetweenTwoCubes(current, neighbor);
                        cameFrom.Add(neighbor, current);

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            List<Vector3> path = new List<Vector3>();

            current = target;
            path.Add(target);

            while (current != origin) {
                cameFrom.TryGetValue(current, out current);
                path.Add(current);
            }

            path.Reverse();

            return path;
        }

        // Validates all cube coordinate results against instantiated Coordinate GameObjects
        public List<Vector3> CleanCubeResults(List<Vector3> cubes) {
            List<Vector3> r = new List<Vector3>();
            foreach (Vector3 cube in cubes)
                if (GetCoordinateContainer(AllContainer).ContainsKey(cube)) {
                    r.Add(cube);
                }
            return r;
        }

        // Returns a coordinate container given a container key
        private Dictionary<Vector3, Coord> GetCoordinateContainer(string key) {
            Dictionary<Vector3, Coord> coordinateContainer;
            if (!_coordinateContainers.TryGetValue(key, out coordinateContainer)) {
                _coordinateContainers.Add(key, new Dictionary<Vector3, Coord>());
                _coordinateContainers.TryGetValue(key, out coordinateContainer);
            }

            return coordinateContainer;
        }

        // Removes empty coordinate containers
        private void CleanEmptyCoordinateContainers() {
            List<string> coordinateContainerKeysToRemove = new List<string>();
            Dictionary<Vector3, Coord> coordinateContainer;
            foreach (string key in _coordinateContainers.Keys) {
                _coordinateContainers.TryGetValue(key, out coordinateContainer);
                if (coordinateContainer.Values.Count == 0)
                    coordinateContainerKeysToRemove.Add(key);
            }

            foreach (string key in coordinateContainerKeysToRemove)
                _coordinateContainers.Remove(key);
        }

        // Returns a Coordinate given a cube coordinate and a container key
        public Coord GetAddCoordinateFromContainer(Vector3 cube, string key) {
            Dictionary<Vector3, Coord> coordinateContainer = GetCoordinateContainer(key);
            if (!coordinateContainer.TryGetValue(cube, out var coord)) {
                coord = new Coord(cube);
                coordinateContainer[cube] = coord;
            }
            return coord;
        }
        
        public Coord GetCoordinateFromContainer(Vector3 cube, string key) {
            Dictionary<Vector3, Coord> coordinateContainer = GetCoordinateContainer(key);
            if (!coordinateContainer.TryGetValue(cube, out var coord)) {
                coord = new Coord();
            }
            return coord;

        }

        // Returns a list of Coordinates given a container key
        public List<Coord> GetCoordinatesFromContainer(string key) {
            List<Coord> coordinates = new List<Coord>();
            Dictionary<Vector3, Coord> coordinateContainer = GetCoordinateContainer(key);
            foreach (KeyValuePair<Vector3, Coord> entry in coordinateContainer)
                coordinates.Add(entry.Value);
            return coordinates;
        }

        // Returns a list of cube coordinates given a container key
        public List<Vector3> GetCubesFromContainer(string key) {
            List<Vector3> cubes = new List<Vector3>();
            Dictionary<Vector3, Coord> coordinateContainer = GetCoordinateContainer(key);
            foreach (Vector3 cube in coordinateContainer.Keys)
                cubes.Add(cube);
            return cubes;
        }

        // Adds a given cube coordinate to the given container key
        public void AddCubeToContainer(Vector3 cube, string key) {
            AddCoordinateToContainer(GetAddCoordinateFromContainer(cube, AllContainer), key);
        }

        // Adds a list of cube coordinates to the given container key
        public void AddCubesToContainer(List<Vector3> cubes, string key) {
            foreach (Vector3 cube in cubes)
                AddCoordinateToContainer(GetAddCoordinateFromContainer(cube, AllContainer), key);
        }

        // Adds a given Coordinate to the given container key
        public bool AddCoordinateToContainer(Coord coordinate, string key) {
            Dictionary<Vector3, Coord> coordinateContainer = GetCoordinateContainer(key);
            if (!coordinateContainer.ContainsKey(coordinate.cubeCoord)) {
                coordinateContainer.Add(coordinate.cubeCoord, coordinate);
                return true;
            }
            return false;
        }

        // Removes a given Coordinate from the given container key
        public void RemoveCoordinateFromContainer(Coord coordinate, string key) {
            Dictionary<Vector3, Coord> coordinateContainer = GetCoordinateContainer(key);
            if (coordinateContainer.ContainsKey(coordinate.cubeCoord))
                coordinateContainer.Remove(coordinate.cubeCoord);
        }

        // Removes all Coordinates from given container key
        public void RemoveAllCoordinatesInContainer(string key) {
            Dictionary<Vector3, Coord> coordinateContainer = GetCoordinateContainer(key);
            coordinateContainer.Clear();
        }

        // Removes a given Coordinate from all containers
        public void RemoveCoordinateFromAllContainers(Coord coordinate) {
            foreach (string key in _coordinateContainers.Keys)
                RemoveCoordinateFromContainer(coordinate, key);
        }

        // Clears all coordinate containers
        public void ClearAllCoordinateContainers() {
            _coordinateContainers.Clear();
        }

        // Clears all Coordinates from a given container key
        public void ClearCoordinatesFromContainer(string key) {
            Dictionary<Vector3, Coord> coordinateContainer = GetCoordinateContainer(key);
            coordinateContainer.Clear();
        }
        
        #region LocalSpace
        /// <summary>
        /// All static 
        /// LocalSpaces are used to convert from Coord vector space to local space coordinates.
        /// local space coordinates are used transform.localPosition,  rectrTransform.localPosition or UIElements 
        /// </summary>
        
        
        protected static int localSpaceIndex = 0;
        
        public static Dictionary<string, LocalSpace> localSpaces = new Dictionary<string, LocalSpace>();
        /// <summary>
        /// A LocalSpace is stores the data needed to calculate position of new hexes
        /// There can be multiple localSpaces for CoordinateTransforms
        /// </summary>
        /// 
        [System.Serializable]
        public class LocalSpace {
            public string id;
            public float gameScale;
            public Vector2 scaleV2;
            public float coordinateRadius;
            public float coordinateWidth;
            public float coordinateHeight;
            public float spacingVertical;
            public float spacingHorizontal;
            public Transform spaceTransform;
            public RectTransform spaceRectTransform;
            public Vector2 offset;
            
            public Orientation orientation;
            
            public enum Orientation {
                XY,
                XZ,
                YX,
                YZ,
                ZX,
                ZY
            }    
            
        }

        public static Vector3 ConvertOrientation(LocalSpace.Orientation or, Vector2 pc) {
            switch (or) {
                case LocalSpace.Orientation.XY:
                    return new Vector3(pc.x,pc.y,0);
                case LocalSpace.Orientation.YX:
                    return new Vector3(pc.y,pc.x,0);
                case LocalSpace.Orientation.XZ:
                    return new Vector3(pc.x,0,pc.y);
                case LocalSpace.Orientation.ZX:
                    return new Vector3(0,pc.x,pc.y);
                case LocalSpace.Orientation.ZY:
                    return new Vector3(0,pc.y,pc.x);
                case LocalSpace.Orientation.YZ:
                    return new Vector3(pc.y,0,pc.x);

                default: 
                    return new Vector3(pc.x,pc.y,0);
            }
        }
        
        public static Vector2 ConvertOrientation(LocalSpace.Orientation or, Vector3 localCoord) {
            switch (or) {
                case LocalSpace.Orientation.XY:
                    return new Vector2(localCoord.x,localCoord.y);
                case LocalSpace.Orientation.YX:
                    return new Vector2(localCoord.y,localCoord.x);
                default: 
                    return new Vector2(localCoord.x,localCoord.y);
            }
        }


        /// <summary>
        /// Setup an new localSpace with Scale
        /// </summary>
        /// <param name="gameScale"> scale for hexes in this localSpace</param>
        /// <param name="aScaleV2">used to scale local coord</param>
        /// <param name="anOrientation"> Orientation Hex Plane </param>
        /// <param name="aSpaceTransform"> transform used for translating world to localSpace </param>
        /// <returns></returns>
        public static string NewLocalSpaceId(float gameScale, Vector2 aScaleV2 , LocalSpace.Orientation anOrientation, Transform aSpaceTransform, Vector3 offset = default) {
            var result = localSpaceIndex.ToString();
            localSpaceIndex++;
            var ls = new LocalSpace {orientation = anOrientation, spaceTransform = aSpaceTransform, offset = offset};
            CalculateCoordinateDimensions(gameScale, aScaleV2, ls);
            localSpaces[result] = ls;
            return result;
        }

        public static LocalSpace GetLocalSpace(string id) {
           return localSpaces[id];
        }

        /// <summary>
        /// Setup an new localSpace with Scale
        /// </summary>
        /// <param name="gameScale"> scale for hexes in this localSpace</param>
        /// <param name="anOrientation"> Orientation Hex Plane </param>
        /// <param name="aSpaceRectTransform"> rectTransform used for translating world to localSpace </param>
        /// <param name="offset"> offset used in convert to and from localSpace </param>
        /// <returns></returns>
        public static string NewLocalSpaceId(float gameScale, Vector2 scaleV2, LocalSpace.Orientation anOrientation, RectTransform aSpaceRectTransform, Vector2 offset = default) {
            //Debug.Log("One NewLocalSpaceId: " + gameScale + " : " + scaleV2);
            
            var result = localSpaceIndex.ToString();
            localSpaceIndex++;
            var ls = new LocalSpace {id = result,orientation = anOrientation, spaceRectTransform = aSpaceRectTransform, offset = offset };
            CalculateCoordinateDimensions(gameScale,scaleV2, ls);
            localSpaces[result] = ls;
            return result;
        }
        
        // Converts an plane coordinate to a local transform position
        public static Vector3 ConvertPlaneToLocalPosition(Vector2 planeCoord, string localSpaceId) {
            var ls = localSpaces[localSpaceId];
            return ConvertPlaneToLocalPosition(planeCoord,ls);
        }

        public static Vector3 ConvertPlaneToLocalPosition(Vector2 planeCoord, LocalSpace ls) {
            var calcV2 = planeCoord + ls.offset;
            var v2 = new Vector2();
            v2.x = calcV2.x * ls.spacingHorizontal;
            v2.y = -((calcV2.x * ls.spacingVertical) + (calcV2.y * ls.spacingVertical * 2.0f));
            v2.Scale(ls.scaleV2);
            return ConvertOrientation(ls.orientation, v2);
        }
        
        public static Vector3 ConvertPlaneToLocalPosition(Vector2 planeCoord, LocalSpace ls, Vector2 offset) {
            var calcV2 = planeCoord + offset;
            var v2 = new Vector2();
            v2.x = calcV2.x * ls.spacingHorizontal;
            v2.y = -((calcV2.x * ls.spacingVertical) + (calcV2.y * ls.spacingVertical * 2.0f));
            v2.Scale(ls.scaleV2);
            return ConvertOrientation(ls.orientation, v2);
        }

        public static Vector3 PlaneToCube(Vector2 plane) {
            return new Vector3(plane.x, plane.y, -plane.x - plane.y);
        }
        
        // Converts a cube coordinate to a local transform position
        public static Vector3 ConvertCubeToLocalPosition(Vector3 aCube, string localSpaceId) {
            var ls = localSpaces[localSpaceId];
            return ConvertPlaneToLocalPosition(CubeCoordinates.ConvertCubetoAxial(aCube ), ls);
        }

        // Converts a local transform position to the nearest plane coordinate
        public static Vector2 ConvertLocalPositionToPlane(Vector3 wPos, string localSpaceId) {
            var ls = localSpaces[localSpaceId];
            var planeCoord = ConvertOrientation(ls.orientation, wPos) ;
            var invertScaleV2 = new Vector2(1/ls.scaleV2.x,1/ls.scaleV2.y);
            planeCoord.Scale(invertScaleV2);
            float q = (planeCoord.x * (2.0f / 3.0f)) / ls.coordinateRadius;
            float r = ((-planeCoord.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * planeCoord.y)) / ls.coordinateRadius;
            return CubeCoordinates.RoundAxial(new Vector2(q, r)+ls.offset);
        }
        
        // Converts a local transform position to the nearest plane coordinate
        public static Vector2 ConvertLocalPositionToPlane(Vector3 wPos, string localSpaceId,Vector2 offset) {
            var ls = localSpaces[localSpaceId];
            var planeCoord = ConvertOrientation(ls.orientation, wPos) ;
            var invertScaleV2 = new Vector2(1/ls.scaleV2.x,1/ls.scaleV2.y);
            planeCoord.Scale(invertScaleV2);
            planeCoord += offset*ls.coordinateRadius*2f;
            float q = (planeCoord.x * (2.0f / 3.0f)) / ls.coordinateRadius;
            float r = ((-planeCoord.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * planeCoord.y)) / ls.coordinateRadius;
            return CubeCoordinates.RoundAxial(new Vector2(q, r)+ls.offset );
        }
        
        public static Vector3 ConvertLocalPositionToCube(Vector3 wPos, string localSpaceId) {
            return CubeCoordinates.ConvertAxialtoCube(ConvertLocalPositionToPlane(wPos, localSpaceId));
        }
        
        public static void CalculateCoordinateDimensions(float gameScale, Vector2 aScaleV2, LocalSpace ws) {
            ws.gameScale = gameScale;
            ws.scaleV2 = aScaleV2; 
            
            ws.coordinateRadius =  ws.gameScale;

            ws.coordinateWidth = ws.coordinateRadius * 2f;
            ws.spacingHorizontal = ws.coordinateWidth * 0.75f;

            ws.coordinateHeight = (Mathf.Sqrt(3) / 2.0f) * ws.coordinateWidth;
            ws.spacingVertical = ws.coordinateHeight / 2.0f;
            
        }
        
        #endregion

    }
    
}
