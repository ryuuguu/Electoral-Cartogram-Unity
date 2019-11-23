using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Ryuuguu.HexGrid {
    public class CoordinateHelper<T> where T: class,ICoordinate  {
        //make & return new coordinates
        // make & return coordinate containers 

        public Transform group;

        /// <summary>
        ///  dictionary [spaceId][containerKey][cubeCoordinate] = coordinate
        /// </summary>
        Dictionary<string, Dictionary<Vector3, T>> _coordinateContainers = new Dictionary<string, Dictionary<Vector3, T>>();
        
        /*
        public void NewCoordinate(Vector3 cube) {
            if (GetCoordinateFromContainer(cube, "all") != null)
                return;

            GameObject obj = new GameObject("Coordinate: [" + cube.x + "," + cube.y + "," + cube.z + "]");
            obj.transform.parent = group.transform;

            T coordinate = obj.AddComponent<T>();
            coordinate.Init(
                cube,
                ConvertCubeToWorldPosition(cube)
            );

            AddCoordinateToContainer(coordinate, "all");
        }

*/
        #region  containers

        
        
        // Validates all cube coordinate results against instantiated Coordinate GameObjects
        public List<Vector3> CleanCubeResults(List<Vector3> cubes) {
            List<Vector3> r = new List<Vector3>();
            foreach (Vector3 cube in cubes)
                if (GetCoordinateContainer("all").ContainsKey(cube))
                    r.Add(cube);
            return r;
        }

        // Returns a coordinate container given a container key
        private Dictionary<Vector3, T> GetCoordinateContainer(string key) {
            if (!_coordinateContainers.TryGetValue(key, out var coordinateContainer)) {
                _coordinateContainers.Add(key, new Dictionary<Vector3, T>());
                _coordinateContainers.TryGetValue(key, out coordinateContainer);
            }

            return coordinateContainer;
        }

        // Removes empty coordinate containers
        private void CleanEmptyCoordinateContainers(string spaceId) {
            List<string> coordinateContainerKeysToRemove = new List<string>();
            Dictionary<Vector3, T> coordinateContainer;
            foreach (string key in _coordinateContainers.Keys) {
                _coordinateContainers.TryGetValue(key, out coordinateContainer);
                if (coordinateContainer.Values.Count == 0)
                    coordinateContainerKeysToRemove.Add(key);
            }

            foreach (string key in coordinateContainerKeysToRemove)
                _coordinateContainers.Remove(key);
        }

        // Returns a Coordinate given a cube coordinate and a container key
        public T GetCoordinateFromContainer(Vector3 cube, string key) {
            T coordinate ;
            Dictionary<Vector3, T> coordinateContainer = GetCoordinateContainer(key);
            if (cube == Vector3.zero)
                coordinateContainer.TryGetValue(Vector3.zero, out coordinate);
            else
                coordinateContainer.TryGetValue(cube, out coordinate);
            return coordinate;
        }

        // Returns a list of Coordinates given a container key
        public List<T> GetCoordinatesFromContainer(string key) {
            List<T> coordinates = new List<T>();
            Dictionary<Vector3, T> coordinateContainer = GetCoordinateContainer(key);
            foreach (KeyValuePair<Vector3, T> entry in coordinateContainer)
                coordinates.Add(entry.Value);
            return coordinates;
        }

        // Returns a list of cube coordinates given a container key
        public List<Vector3> GetCubesFromContainer(string key) {
            List<Vector3> cubes = new List<Vector3>();
            Dictionary<Vector3, T> coordinateContainer = GetCoordinateContainer(key);
            foreach (Vector3 cube in coordinateContainer.Keys)
                cubes.Add(cube);
            return cubes;
        }

        // Adds a given cube coordinate to the given container key
        public void AddCubeToContainer(Vector3 cube, string key) {
            AddCoordinateToContainer(GetCoordinateFromContainer(cube, "all"), key);
        }

        // Adds a list of cube coordinates to the given container key
        public void AddCubesToContainer(List<Vector3> cubes, string key,string spaceId) {
            foreach (Vector3 cube in cubes)
                AddCoordinateToContainer(GetCoordinateFromContainer(cube, "all"), key);
        }

        // Adds a given Coordinate to the given container key
        public bool AddCoordinateToContainer(T coordinate, string key) {
            Dictionary<Vector3, T> coordinateContainer = GetCoordinateContainer(key);
            if (!coordinateContainer.ContainsKey(coordinate.cube)) {
                coordinateContainer.Add(coordinate.cube, coordinate);
                return true;
            }

            return false;
        }

        // Removes a given Coordinate from the given container key
        public void RemoveCoordinateFromContainer(T coordinate, string key) {
            Dictionary<Vector3, T> coordinateContainer = GetCoordinateContainer(key);
            if (coordinateContainer.ContainsKey(coordinate.cube))
                coordinateContainer.Remove(coordinate.cube);
        }

        // Removes all Coordinates from given container key
        public void RemoveAllCoordinatesInContainer(string key) {
            Dictionary<Vector3, T> coordinateContainer = GetCoordinateContainer(key);
            coordinateContainer.Clear();
        }

        // Removes a given Coordinate from all containers
        public void RemoveCoordinateFromAllContainers(T coordinate) {
            foreach (string key in _coordinateContainers.Keys)
                RemoveCoordinateFromContainer(coordinate, key);
        }

        // Clears all coordinate containers
        public void ClearAllCoordinateContainers(string spaceId) {
            _coordinateContainers.Clear();
        }

        // Clears all Coordinates from a given container key
        public void ClearCoordinatesFromContainer(string key) {
            Dictionary<Vector3, T> coordinateContainer = GetCoordinateContainer(key);
            coordinateContainer.Clear();
        }

        // Hides all Coordinates for a given container key
        public void HideCoordinatesInContainer(string key) {
            foreach (T coordinate in GetCoordinatesFromContainer(key)) {
                coordinate.Hide();
                RemoveCoordinateFromContainer(coordinate, "visible");
            }
        }

        // Shows all Coordinates for a given container key
        public void ShowCoordinatesInContainer(string key, bool bCollider = true) {
            foreach (T coordinate in GetCoordinatesFromContainer(key)) {
                coordinate.Show(bCollider);
                AddCoordinateToContainer(coordinate, "visible");
            }
        }

        // Hides and Clears all Coordinates for a given container key
        public void HideAndClearCoordinateContainer(string key) {
            foreach (T coordinate in GetCoordinatesFromContainer(key))
                coordinate.Hide();
            ClearCoordinatesFromContainer(key);
        }
    

        #endregion
        
        
        #region worldSpace
        
        private float _gameScale = 1.0f;
        private float _coordinateRadius = 1.0f;

        private float _coordinateWidth = 0.0f;
        private float _coordinateHeight = 0.0f;

        private float _spacingVertical = 0.0f;
        private float _spacingHorizontal = 0.0f;
        
        // Converts an axial coordinate to a world transform position
        public Vector3 ConvertAxialToWorldPosition(Vector2 axial) {
            float x = axial.x * _spacingHorizontal;
            float z = -((axial.x * _spacingVertical) + (axial.y * _spacingVertical * 2.0f));

            return new Vector3(x, 0.0f, z);
        }

        // Converts a cube coordinate to a world transform position
        public Vector3 ConvertCubeToWorldPosition(Vector3 cube) {
            float x = cube.x * _spacingHorizontal;
            float y = 0.0f;
            float z = -((cube.x * _spacingVertical) + (cube.z * _spacingVertical * 2.0f));

            return new Vector3(x, y, z);
        }

        // Converts a world transform position to the nearest axial coordinate
        public virtual Vector2 ConvertWorldPositionToAxial(Vector3 wPos) {
            float q = (wPos.x * (2.0f / 3.0f)) / _coordinateRadius;
            float r = ((-wPos.x / 3.0f) + ((Mathf.Sqrt(3) / 3.0f) * wPos.z)) / _coordinateRadius;
            return CubeCoordinates<T>.RoundAxial(new Vector2(q, r));
        }

        public void CalculateCoordinateDimensions() {
            _coordinateRadius = _coordinateRadius * _gameScale;

            _coordinateWidth = _coordinateRadius * 2;
            _spacingHorizontal = _coordinateWidth * 0.75f;

            _coordinateHeight = (Mathf.Sqrt(3) / 2.0f) * _coordinateWidth;
            _spacingVertical = _coordinateHeight / 2.0f;

            HexMeshCreator.Instance.SetRadius(_coordinateRadius);
        }
        
        
        #endregion
    }
}
