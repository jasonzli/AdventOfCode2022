using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtility;
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