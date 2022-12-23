using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_12
{
    public class Solver : MonoBehaviour
    {
        [SerializeField] private AssetReference _addressableTextAsset = null;

        private string[] _inputLines;

        [SerializeField] private List<string> _separatingCharacters = new List<string>();

        [SerializeField] private int heightLimit = 1;
        void Start()
        {
            _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
            {
                //this input is broken by double line breaks
                _inputLines = AdventUtilities.ProcessTextAssetIntoStringArray(handle.Result, _separatingCharacters.ToArray());
                
                Process(_inputLines);
            };

        }

        void Process(string[] inputs)
        {
            GridMap map = new GridMap(inputs, heightLimit);
            
            Debug.Log($"The shortest steps from starting to end is {map.ConnectStartToEnd()}");
            
            Debug.Log($"The shortest path from the End to the floor is {map.ConnectEndToFloor()}");
        }

        class GridMap
        {
            private Dictionary<Vector2Int, int> _map = new Dictionary<Vector2Int, int>();

            private int MapWidth { get; set; }
            private int MapHeight { get; set; }

            private int HeightLimit;
            private Vector2Int _startingPosition;
            private Vector2Int _endingPosition;
            
            private HashSet<Vector2Int> _unvisitedLocations = new HashSet<Vector2Int>();

            private Dictionary<Vector2Int, ParentDistancePair> _visitedLocations =
                new Dictionary<Vector2Int, ParentDistancePair>();


            private struct ParentDistancePair
            {
                public Vector2Int Location { get; set; }
                public int Distance { get; set; }
                
                public ParentDistancePair(Vector2Int parentLocation, int distance)
                {
                    Distance = distance;
                    Location = parentLocation;
                }
            }
            
            private struct FromToDistancePair
            {
                public Vector2Int FromLocation;
                public Vector2Int ToLocation;
                public int Distance;

                public FromToDistancePair(Vector2Int from, Vector2Int to, int distance)
                {
                    FromLocation = from;
                    ToLocation = to;
                    Distance = distance;
                }
            }
            public GridMap(string[] inputMap, int heightLimit)
            {

                //Create map
                MapWidth = inputMap[0].Length;
                MapHeight = inputMap.Length;
                HeightLimit = heightLimit;
                
                for (int y = 0; y < MapHeight; y++)
                {
                    for (int x = 0; x < MapWidth; x++)
                    {
                        AddPosition(new Vector2Int(x,y), inputMap[y][x]);
                    }
                }

                Debug.Log($"Map created with width {MapWidth} and height {MapHeight}");
                Debug.Log($"Starting Position is {_startingPosition.x}, {_startingPosition.y} and ending position is {_endingPosition.x},{_endingPosition.y}");
                Debug.Log($"There is a floor at {_map[Vector2Int.zero]}");
            }
            
            private void AddPosition(Vector2Int mapIndex, char value)
            {
                int CharToInt(char c)
                {
                    // Convert character to integer with a as 0 and b as 1 and etc.
                    return c - 'a'; 
                }

                // Handle special cases, S and E
                if (value == 'S')
                {
                    _map.Add(mapIndex, CharToInt('a'));
                    _startingPosition = mapIndex;
                    return;
                }

                if (value == 'E')
                {
                    _map.Add(mapIndex, CharToInt('z'));
                    _endingPosition = mapIndex;
                    return;
                }

                _map.Add(mapIndex, CharToInt(value));
            }
            

            
            public int ConnectStartToEnd()
            {
                //clear visited locations
                _visitedLocations.Clear();
                
                //Create the unvisited locations list
                _unvisitedLocations = _map.Keys.ToHashSet();


                //Go through all the places in the map until we have found the end point
                Queue<FromToDistancePair> queue = new Queue<FromToDistancePair>();

                // log the distances
                int accumulatedDistance = 0;
                
                // Enqueue the first position, with starting position
                queue.Enqueue(new FromToDistancePair(Vector2Int.zero, _startingPosition, accumulatedDistance));

                while (queue.Count > 0)
                {
                    FromToDistancePair currentPair = queue.Dequeue();

                    Vector2Int currentPosition = currentPair.ToLocation;
                    
                    if (_visitedLocations.ContainsKey(currentPosition))
                    {
                        //we have already visited this location, so we can skip it
                        continue;
                    }
                    
                    // Add the position to the visited locations
                    _visitedLocations.Add(currentPosition,
                        new ParentDistancePair(currentPair.FromLocation, currentPair.Distance));

                    // Remove from the list of unvisited places
                    _unvisitedLocations.Remove(currentPosition);
                    
                    // Add distance to the accumulated distance
                    accumulatedDistance = currentPair.Distance + 1;
                    
                    // Add all potential places to the queue
                    AllValidLocationsSurroundingPosition(currentPosition)
                        .ForEach(nextPosition => 
                            queue.Enqueue(new FromToDistancePair(currentPosition, nextPosition, accumulatedDistance)));
                    
                    
                }

                return _visitedLocations[_endingPosition].Distance;
            }

            private List<Vector2Int> AllValidLocationsSurroundingPosition(Vector2Int positionToCheck)
            {
                List<Vector2Int> validLocations = new List<Vector2Int>();
                
                // Check all cardinal directions around validlocation
                Vector2Int[] directionsToCheck = new Vector2Int[]
                {
                    Vector2Int.up,
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right
                };

                foreach (Vector2Int direction in directionsToCheck)
                {
                    Vector2Int testPosition = positionToCheck + direction;
                    
                    // Skip if the position is out of the map, either <0 or greater than the map size
                    if (testPosition.x < 0 || testPosition.x >= MapWidth || testPosition.y < 0 || testPosition.y >= MapHeight)
                    {
                        continue;
                    }
                    
                    // Skip if the position's height value is greater than the height limit
                    if (_map[testPosition] - _map[positionToCheck] > HeightLimit)
                    {
                        continue;
                    }
                    
                    // Skip if the position is already visited
                    if (!_unvisitedLocations.Contains(testPosition))
                    {
                        continue;
                    }
                    
                    // Add the position to the list of valid locations
                    validLocations.Add(testPosition);
                }

                return validLocations;
            }
            private int BreadthFirstSearchMap()
            {
                return 0;
            }

            public int ConnectEndToFloor()
            {
                //Do a BFS from the End position to the closest floor position.

                Queue<FromToDistancePair> queue = new Queue<FromToDistancePair>();

                queue.Enqueue(new FromToDistancePair(Vector2Int.zero, _endingPosition, 0));
                _unvisitedLocations = _map.Keys.ToHashSet();
                _visitedLocations = new Dictionary<Vector2Int, ParentDistancePair>();
                _startingPosition = Vector2Int.zero;

                int shortestDistance = int.MaxValue;
                while (queue.Count > 0)
                {
                    FromToDistancePair currentLocation = queue.Dequeue();

                    int locationHeight = _map[currentLocation.ToLocation];

                    if (_visitedLocations.ContainsKey(currentLocation.ToLocation))
                    {
                        continue;
                    }
                    
                    _visitedLocations.Add(currentLocation.ToLocation,new ParentDistancePair(currentLocation.FromLocation,currentLocation.Distance));
                    
                    if (locationHeight == 0 && 
                        currentLocation.Distance < shortestDistance)
                    {
                        //we found the end
                        shortestDistance = currentLocation.Distance;
                        _startingPosition = currentLocation.ToLocation;
                        
                    }

                    foreach (Vector2Int nextLocation in AddViableLocationsFromPosition(currentLocation.ToLocation))
                    {
                        queue.Enqueue(new FromToDistancePair(
                            currentLocation.ToLocation, 
                            nextLocation,
                            currentLocation.Distance + 1));
                    }
                    
                    _unvisitedLocations.Remove(currentLocation.ToLocation);
                    
                }

                return _visitedLocations[_startingPosition].Distance;
            }

            private List<Vector2Int> AddViableLocationsFromPosition(Vector2Int position)
            {
                List<Vector2Int> newLocations = new List<Vector2Int>();

                Vector2Int[] directions = new Vector2Int[]
                {
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.up,
                    Vector2Int.right
                };

                foreach (Vector2Int direction in directions)
                {
                    Vector2Int locationToCheck = position + direction;

                    //Location out of bounds, skip
                    if (locationToCheck.x < 0 || locationToCheck.x >= MapWidth || locationToCheck.y < 0 ||
                        locationToCheck.y >= MapHeight)
                    {
                        continue;
                    }

                    // Need to make sure we're only going down a step, which is this difference, because we're *one above*
                    if (_map[locationToCheck] - _map[position] < -1)
                    {
                        continue;
                    }
                    
                    //Position is already visited
                    if (!_unvisitedLocations.Contains(locationToCheck))
                    {
                        continue;
                    }
                    
                    newLocations.Add(locationToCheck);
                }

                return newLocations;
            }
        }
        
    }
}