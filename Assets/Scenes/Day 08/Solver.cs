using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtility;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_08
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
            
            int treesVisible = 0;

            int width = inputs[0].Length; // length of the string
            int height = inputs.Length; // height of the array
            
            Debug.Log($"The width is {width} and height is {height}");
            
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    treesVisible += IsTreeVisibleFromEdge(row, column, inputs) ? 1 : 0;
                }
            }
            
            //Log the number of trees visible
            Debug.Log($"The trees visible are: {treesVisible}");


            Vector2Int tallestTree = new Vector2Int();
            int score = -1;
            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    int newScore = DumbTreeVisibilityScore(row, column, inputs);
                    if (newScore > score)
                    {
                        score = newScore;
                        tallestTree = new Vector2Int(column, row);
                    }
                }
            }
            
            Debug.Log($"The visibility score of the best tree is {score}");
        }

        int DumbTreeVisibilityScore(int yIndex, int xIndex, string[] inputGrid)
        {
            int treeValue = int.Parse(inputGrid[yIndex][xIndex].ToString());

            int width = inputGrid[0].Length;
            int height = inputGrid.Length;

            int leftScore = 0;
            for (int i = 1; i <=xIndex; i++)
            {
                int nextPosition = xIndex - i;
                if (nextPosition < 0)
                {
                    break;
                }
                
                leftScore++;
                int tree = int.Parse(inputGrid[yIndex][nextPosition].ToString());
                if (tree >= treeValue)
                {
                    break;
                }

            }
            
            int rightScore = 0;
            for (int i = 1; i <= width-xIndex; i++)
            {
                int nextPosition = xIndex + i;
                if (nextPosition >= width)
                {
                    break;
                }
                
                rightScore++;
                int tree = int.Parse(inputGrid[yIndex][nextPosition].ToString());
                if (tree >= treeValue)
                {
                    break;
                }

            }

            int upScore = 0;
            for (int i = 1; i <= yIndex; i++)
            {
                int nextPosition = yIndex - i;
                if (nextPosition < 0)
                {
                    break;
                }
                
                upScore++;
                int tree = int.Parse(inputGrid[nextPosition][xIndex].ToString());
                if (tree >= treeValue)
                {
                    break;
                }

            }

            int downScore = 0;
            for (int i = 1; i <= height - yIndex; i++)
            {
                int nextPosition = yIndex + i;
                if (nextPosition >= height)
                {
                    break;
                }
                
                downScore++;
                int tree = int.Parse(inputGrid[nextPosition][xIndex].ToString());
                if (tree >= treeValue)
                {
                    break;
                }

            }

            return leftScore * rightScore * upScore * downScore;
        }

        int TreeVisibilityScore(int yIndex, int xIndex, string[] inputGrid)
        {
            int treeValue = int.Parse(inputGrid[yIndex][xIndex].ToString());
            Vector2Int[] directions = new[]
            {
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
            };

            List<int> viewingDistances = new List<int>();
            foreach (Vector2Int direction in directions)
            {
                int travel = 0;
                Vector2Int directionVector = direction;
                Vector2Int currentLocation = new Vector2Int(xIndex, yIndex);
                Vector2Int nextLocation = currentLocation + directionVector;
                
                // While the next location is within the grid
                while (nextLocation.x >= 0 && nextLocation.x < inputGrid[0].Length && nextLocation.y >= 0 && nextLocation.y < inputGrid.Length)
                {
                    //the next position is in the bounds of the grid, so add to the distance
                    travel++;
                    int treeToCompare = int.Parse(inputGrid[nextLocation.y][nextLocation.x].ToString());
                    
                    if (treeToCompare >= treeValue)
                    {
                        break;
                    }
                    
                    //move to the next location
                    nextLocation = direction * travel;
                }

                viewingDistances.Add(travel);
            }

            int result = viewingDistances[0] * viewingDistances[1] * viewingDistances[2] * viewingDistances[3];
            return result;
        }
        bool IsTreeVisibleFromEdge(int yIndex, int xIndex, string[] inputGrid)
        {
            int treeValue = int.Parse(inputGrid[yIndex][xIndex].ToString());

            int width = inputGrid[0].Length;
            int height = inputGrid.Length;
            
            //check each cardinal direction
            
            //From left
            bool visibleFromLeft = true;
            for (int i = 0; i < xIndex; i++)
            {
                int tree = int.Parse(inputGrid[yIndex][i].ToString());
                if (tree >= treeValue)
                {
                    visibleFromLeft = false;
                    break;
                }
            }
            
            //From right
            bool visibleFromRight = true;
            for (int i = width-1; i > xIndex; i--)
            {
                int tree = int.Parse(inputGrid[yIndex][i].ToString());
                
                if (tree >= treeValue)
                {
                    visibleFromRight = false;
                    break;
                }
            }

            //From top
            bool visibleFromTop = true;
            for (int i = 0; i < yIndex; i++)
            {
                int tree = int.Parse(inputGrid[i][xIndex].ToString());
                if (tree >= treeValue)
                {
                    visibleFromTop = false;
                    break;
                }
            }
            
            //From bottom
            bool visibleFromBottom = true;
            for (int i = height-1; i > yIndex; i--)
            {
                int tree = int.Parse(inputGrid[i][xIndex].ToString());
                
                if (tree >= treeValue)
                {
                    visibleFromBottom = false;
                    break;
                }
            }

            return visibleFromBottom || visibleFromLeft || visibleFromRight || visibleFromTop;
        }
    }
}