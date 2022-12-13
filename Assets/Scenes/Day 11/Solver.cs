using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SharedUtility;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_11
{
    public class Solver : MonoBehaviour
    {
        [SerializeField] private AssetReference _addressableTextAsset = null;

        private string[] _inputLines;

        [SerializeField] private List<string> _separatingCharacters = new List<string>();

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
            foreach (string input in inputs)
            {
                string pattern = @"\r\n\r\n";
                Debug.Log(Regex.Split(input, pattern).Length);
            };
        }

       
        
    }
}