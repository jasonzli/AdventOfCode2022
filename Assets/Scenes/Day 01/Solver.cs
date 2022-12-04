using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scenes.Day_01
{
    public class Solver : MonoBehaviour
    {
        [SerializeField] private string _textAssetPath;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        string[] CreateCalorieListFromTextInput(string path)
        {
            // Get the file at the path

            // Go through line by line and make an Elf for each group, breaking up based on empty lines
            return new string[] { };
            
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
            foreach (Elf elf in listOfElves)
            {
                if (highestCalorieLoad < elf.TotalCalories)
                {
                    highestCalorieLoad = elf.TotalCalories;
                }
            }

            //highestCalorieLoad = listOfElves.Max(elf => elf.TotalCalories);

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
