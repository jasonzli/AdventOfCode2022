using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SharedUtility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_13
{
    public class Solver : MonoBehaviour
    {
        [SerializeField] private AssetReference _addressableTextAsset = null;

        private string[] _inputLines;

        //Unity automatically will escape your text from a GUI text entry :( annoyingfire pu
        [SerializeField] private List<string> _separatingCharacters = new List<string>();

        void Start()
        {
            _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
            {
                //this input is broken by double line breaks
                _inputLines = AdventUtilities.RegexProcessTextAssetIntoStringArray(handle.Result, _separatingCharacters.ToArray());
                
                // Use Regex for two line separation, in the future use a regex approach anyway
                Process(_inputLines);
            };

        }

        void Process(string[] inputs)
        {
            Debug.Log($"How many line pairs are there? {inputs.Length}");

            string[] inputsWithoutPatterns = inputs.Where(x => x != "\n\n").ToArray();
            
            Debug.Log($"How many line pairs are there after filter? {inputsWithoutPatterns.Length}");
            
            //now you can process the text
            
        }
        
        
        
    }
}