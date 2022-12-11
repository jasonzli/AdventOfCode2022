using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtility;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_09
{
    public class Solver : MonoBehaviour
    {
        [SerializeField] private AssetReference _addressableTextAsset = null;

        private string[] _inputLines;

        void Start()
        {
            _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
            {
                _inputLines = AdventUtilities.ProcessTextAssetIntoStringArray(handle.Result);
                Process(_inputLines);
            };

        }

        void Process(string[] inputs)
        {
            
            Rope rope = new Rope(Vector2Int.zero,1); // new Rope with length 1

            foreach (string command in inputs)
            {
                
                string[] splitCommand = command.Split(' ');
                string direction = splitCommand[0];
                int distance = int.Parse(splitCommand[1]);

                for (int i = 0; i < distance; i++)
                {
                    rope.Move(direction);    
                }
                
            }
            
            Debug.Log($"Tail has visited {rope.TailVisited} positions");
        }

        class Rope
        {
            private Vector2Int _position;
            private HashSet<Vector2Int> _visitedPositions = new HashSet<Vector2Int>();
            private Rope _tail;

            private static readonly Dictionary<string, Vector2Int> _RopeDirections = new Dictionary<string, Vector2Int>
            {
                {"U", Vector2Int.up},
                {"D", Vector2Int.down},
                {"L", Vector2Int.left},
                {"R", Vector2Int.right}
            };

            public int TailVisited => _tail == null ? _visitedPositions.Count : _tail.TailVisited;
            

            public Rope(Vector2Int position, int length)
            {
                _position = new Vector2Int(position.x, position.y);
                _visitedPositions.Add(_position);

                if (length > 0)
                {
                    _tail = new Rope(_position, length - 1);
                }
            }
            
            public void Move(string direction)
            {
                Vector2Int moveDirection = _RopeDirections[direction];

                Vector2Int currentPosition = _position;
                
                _position += moveDirection;
                
                _visitedPositions.Add(_position);

                if (_tail != null)
                {
                    _tail.Follow(_position, currentPosition);
                }
            }

            public void Follow(Vector2Int target, Vector2Int oldPosition)
            {
                Vector2Int difference = target - _position;

                Vector2Int currentPosition = _position;
                
                //if taxicab difference is 1 or less, we are adjacent
                if (Mathf.Abs(difference.x) <= 1 && Mathf.Abs(difference.y) <= 1)
                {
                    return;
                }
                _position = oldPosition;
                _visitedPositions.Add(_position);

                if (_tail != null)
                {
                    _tail.Follow(_position,currentPosition);

                }
            }
        }
        
    }
}