using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_01
{
    public class Solver : MonoBehaviour
    {
        [SerializeField] private string _textAssetPath;

        [SerializeField] private AssetReference _addressableTextAsset = null;
        // Start is called before the first frame update
        void Start()
        {
            _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
            {
                Debug.Log($"Length: {handle.Result.text.Length}");

                TextAsset inputAsset = handle.Result;

                string[] allLines = inputAsset.text.Split('\n');

                Debug.Log($"Lines: {allLines.Length}");

                int[] calorieGroup;
                
                
                Addressables.Release(handle); // Need to call this to release the asset stream(?)
            };
        }

        List<Elf> CreateElfList(string[] calorieList)
        {
            List<Elf> elfList = new List<Elf>();
            // Iterate through calroie list and create an elf based on space limited list
            
            return elfList;
        }

        int FindHighestCalorieLoad(IEnumerable<Elf> listOfElves)
        {
            int highestCalorieLoad = -1;
            
            //highestCalorieLoad = listOfElves.Max(elf => elf.TotalCalories);
            foreach (Elf elf in listOfElves)
            {
                if (highestCalorieLoad < elf.TotalCalories)
                {
                    highestCalorieLoad = elf.TotalCalories;
                }
            }

            return highestCalorieLoad;
        }

        private struct Elf
        {
            private int[] _calories;
            public int TotalCalories => _calories.Sum();

            public Elf(int[] calories)
            {
                _calories = calories;
            }
        }
    }
}
