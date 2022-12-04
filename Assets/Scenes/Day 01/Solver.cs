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


        private string[] _inputLines;
        
        // Start is called before the first frame update
        void Start()
        {
            List<Elf> elves = new List<Elf>();

            _addressableTextAsset.LoadAssetAsync<TextAsset>().Completed += handle =>
            {
                Debug.Log($"Length: {handle.Result.text.Length}");

                TextAsset inputAsset = handle.Result;

                string[] allLines = inputAsset.text.Split('\n');

                Debug.Log($"Lines: {allLines.Length}");

                _inputLines = allLines;
                
                elves = CreateElvesFromInputs(_inputLines);
                Debug.Log($"The highest amount of calories is {HighestCalorieLoadFromElves(elves)}");

                // Unsorted approach
                Debug.Log($"From the unsorted list the value of the top three is {HighestCalorieLoadFromUnsortedElves(elves)}");
                
                // Sort the list
                elves = elves.OrderByDescending(elf => elf.TotalCalories).ToList();
                Debug.Log($"The highest amount of calories in the sorted list is {HighestCalorieLoadFromTopThreeElves(elves)}");
                
                Addressables.Release(handle); // Need to call this to release the asset stream(?)
            };


            
        }

        List<Elf> CreateElvesFromInputs(string[] lineInput)
        {
            List<Elf> elves = new List<Elf>();
            List<int> calorieGroup = new List<int>();

            for (int i = 0; i < lineInput.Length; i++)
            {
                string line = lineInput[i];
                if (int.TryParse(line, out int calories))
                {
                    calorieGroup.Add(calories);
                }
                else
                {
                    // Could not parse as an integer and should create an new elf
                    elves.Add(new Elf(calorieGroup.ToArray()));
                    calorieGroup.Clear();
                }
            }

            return elves;
        }

        // Unsorted list of Elves
        int HighestCalorieLoadFromUnsortedElves(List<Elf> unsortedListOfElves)
        {
            int firstCalorieLoad = -1;
            int secondCalorieLoad = -1;
            int thirdCalorieLoad = -1;

            foreach (Elf elf in unsortedListOfElves)
            {
                // if the value is higher than the first value
                // 1. shift down the values
                // 2. set the first value
                // continue
                if (elf.TotalCalories > firstCalorieLoad)
                {
                    thirdCalorieLoad = secondCalorieLoad;
                    secondCalorieLoad = firstCalorieLoad;
                    firstCalorieLoad = elf.TotalCalories;
                    continue;
                }

                if (elf.TotalCalories > secondCalorieLoad)
                {
                    thirdCalorieLoad = secondCalorieLoad;
                    secondCalorieLoad = elf.TotalCalories;
                    continue;
                }

                if (elf.TotalCalories > thirdCalorieLoad)
                {
                    thirdCalorieLoad = elf.TotalCalories;
                    continue;
                }
            }
            
            // sum
            return firstCalorieLoad + secondCalorieLoad + thirdCalorieLoad;
        }
        
        
        // Assumes the list is already sorted
        int HighestCalorieLoadFromTopThreeElves(List<Elf> sortedListOfElves)
        {
            return sortedListOfElves[0].TotalCalories + 
                   sortedListOfElves[1].TotalCalories + 
                   sortedListOfElves[2].TotalCalories ;
        }
        
        int HighestCalorieLoadFromElves(IEnumerable<Elf> listOfElves)
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
