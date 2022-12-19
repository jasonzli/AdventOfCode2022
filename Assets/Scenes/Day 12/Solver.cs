using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using OpenCover.Framework.Model;
using SharedUtility;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.Experimental.GraphView;
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
            GridMap map = new GridMap(inputs);
            
            Debug.Log($"The shortest steps from starting to end is {map.ConnectStartToEnd()}");
        }

        class GridMap
        {
            private Dictionary<Vector2Int, int> _map = new Dictionary<Vector2Int, int>();

            private Vector2Int _startingPosition;
            private Vector2Int _endingPosition;
            
            private HashSet<Vector2Int> _unvisitedLocations = new HashSet<Vector2Int>();

            private Dictionary<Vector2Int, LocationDistancePair> _visitedLocations =
                new Dictionary<Vector2Int, LocationDistancePair>();

            private struct LocationDistancePair
            {
                private Vector2Int _location;
                private int _distance;

                public Vector2Int Location => _location;
                public int Distance => _distance;
            }

            public GridMap(string[] inputMap)
            {
                int width = inputMap[0].Length;
                int height = inputMap.Length;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        AddPosition(new Vector2Int(x,y), inputMap[y][x]);
                    }
                }
                
                Debug.Log($"Map created with width {width} and height {height}");
                Debug.Log($"Starting Position is {_startingPosition.x}, {_startingPosition.y} and ending position is {_endingPosition.x},{_endingPosition.y}");
            }
            
            private void AddPosition(Vector2Int mapIndex, char value)
            {
                int CharToInt(char c)
                {
                    // Convert character to integer with a as 0 and b as 1 and etc.
                    return c - 'a'; 
                }

                //Add to the unvisited locations
                _unvisitedLocations.Add(mapIndex);
                
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
            
            public int ConnectStartToEnd()
            {

       

                //Go through all the places in the map until we have found the end point
                Queue<FromToDistancePair> queue = new Queue<FromToDistancePair>();
                
                queue.Enqueue(new FromToDistancePair(Vector2Int.zero, _startingPosition, 0));
                
                return BreadthFirstSearchMap();
            }

            private int BreadthFirstSearchMap()
            {
                return 0;
            }
        }
        
    }
}