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
            
            List<Stack<Crate>> stacks = new List<Stack<Crate>>();
            
            // We have the empty line and know where the grid ends
            string lastRow = gridOfCrates.Last(); // we can go through this and find the number of columns *and* the index of the crates too
            int column = 1;
            Dictionary<int, int> crateColumnIndex = new Dictionary<int, int>();
            
            while (lastRow.IndexOf(column.ToString()) != -1) // while we can find column in this last row
            {
                int columnIndex = lastRow.IndexOf(column.ToString()); //this is the index of the column's contents

                Stack<Crate> crateStackAtIndex = new Stack<Crate>();
                for (int i = splitIndex - 2; i >= 0; i--) //go through rows backwards
                {
                    char crateContent = gridOfCrates[i][columnIndex];
                    if (crateContent == ' ')
                    {
                        break;
                    }
                        
                    crateStackAtIndex.Push(new Crate(crateContent));
                    
                }

                stacks.Add(crateStackAtIndex);
                
                crateColumnIndex.Add(column, columnIndex); 
                column++; //increment forward
            }
            
            //Stacks now has all the crates in order, we just have to process the movements of them as commands
            string movementRegexPattern = @"move (\d+) from (\d+) to (\d+)";
            foreach (string movement in movements)
            {
                //match the pattern against the movement string
                Match match = Regex.Match(movement, movementRegexPattern);
                if (match.Success)
                {
                    int numberOfCrates = int.Parse(match.Groups[1].Value);
                    int fromColumn = int.Parse(match.Groups[2].Value);
                    int toColumn = int.Parse(match.Groups[3].Value);
                    
                    //MoveCratesBetweenStacks(numberOfCrates, stacks[fromColumn-1], stacks[toColumn-1]);
                    MoveCratesBetweenStacksWithOrderRetention(numberOfCrates, stacks[fromColumn - 1],
                        stacks[toColumn - 1]);
                }
            }
            
            // Read the first entry in all stacks as one string
            string finalString = "";
            foreach (Stack<Crate> stack in stacks)
            {
                finalString += stack.Peek().CrateContent;
            }
            
            Debug.Log($"Final crates are: {finalString}");
        }


        private class Crate
        {
            public char CrateContent { get; private set; }
            public Crate( char crateContent)
            {
                CrateContent = crateContent;
            }
        }

        private void MoveCratesBetweenStacks(int quantity, Stack<Crate> origin, Stack<Crate> target)
        {
            for (int i = 0; i < quantity; i++)
            {
                Crate temp = origin.Pop();
                target.Push(temp);
            }
        }

        private void MoveCratesBetweenStacksWithOrderRetention(int quantity, Stack<Crate> origin, Stack<Crate> target)
        {
            // Create a stack of objects that will get moved into the new stack

            Stack<Crate> cratesToMove = new Stack<Crate>();
            for (int i = 0; i < quantity; i++)
            {
                cratesToMove.Push(origin.Pop());
            }
            
            // then move the stack onto the new stack
            // move all crates from cratesToMove onto target

            for (int i = 0; i < quantity; i++)
            {
                target.Push(cratesToMove.Pop());
            }
        }
    }
}