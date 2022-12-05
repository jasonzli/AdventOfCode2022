using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SharedUtility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_05
{
    public class Solver : MonoBehaviour
    {
        [SerializeField] private AssetReference _addressableTextAsset = null;

        private string[] _inputLines;

        void Start()
        {
            _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
            {
                Debug.Log($"Length: {handle.Result.text.Length}");

                _inputLines = AdventUtilities.ProcessTextAssetIntoStringArray(handle.Result);

                Process(_inputLines);
            };

        }

        void Process(string[] inputs)
        {
            // split the input array into two arrays one for the grid and one for the movements broken at the empty line
            int splitIndex = System.Array.IndexOf(inputs, String.Empty);
            
            string[] gridOfCrates = inputs.Take(splitIndex).ToArray();
            string[] movements = inputs.Skip(splitIndex + 1).ToArray(); //plus 1 to skip the empty line

            Debug.Log($"Empty line at index: {splitIndex}");
            
            // Need to process the grid strings into stacks, so we can push and pop from them, but they are delimited weirdly
            // so we have to manipulate them into a form that we can use.
            
            // First, get the amount of columns in the grid from the last row in the 
            // a string regex pattern that captures all numbers separated by spaces
            string columnMatchPattern = @"(\d+)";
            string lastRow = gridOfCrates.Last();
            int columns = int.Parse(Regex.Match(lastRow, columnMatchPattern, RegexOptions.RightToLeft).Groups.Last().Value);

            

        }
        
    }
}