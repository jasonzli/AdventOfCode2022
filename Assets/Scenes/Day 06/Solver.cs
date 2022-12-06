using System.Collections.Generic;
using System.Linq;
using SharedUtility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_06
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
           // Read each line of the inputs and find the index of the first instance of 4 unique characters
           int index = -1;
           foreach (string input in inputs)
           {
               index = IndexOfFirstFourUniqueCharactersInString(input,4);
           }
           
           Debug.Log($"The index of the unique 4 character string is {index}");
           
           foreach (string input in inputs)
           {
               index = IndexOfFirstFourUniqueCharactersInString(input,14);
           }
           
           
           Debug.Log($"The index of the unique 14 character string is {index}");

        }

        int IndexOfFirstFourUniqueCharactersInString(string input, int length)
        {
            for (int i = 0; i < input.Length; i++)
            {
                string subString = input.Substring(i, length); 
                if (subString.Length == length && subString.Distinct().Count() == length)
                {
                    return i + length;
                }
            }

            return -1;
        }

    }
}