using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenCover.Framework.Model;
using SharedUtility;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.Experimental.GraphView;
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
            //for whatever reason this line break thing is broken as fuck
            // Didn't work with split but will work with regex or whatever reason
            string separator = @"\n\n";

            string inputString = inputs[0];
            string[] monkeys = Regex.Split(inputString, separator);
            
        }

        List<Monkey> CreateMonkeysFromString(string[] monkeyStrings)
        {
            foreach(string monkeyLine in monkeyStrings)
            {
                string[] lineSeparators = new string[] { "\r\n" , "\n", "\r" };
                
                string[] allLines = monkeyLine.Split(lineSeparators, System.StringSplitOptions.None);

                //Trim the lines
                allLines = allLines.Select(line => line.Trim()).ToArray();
                
                //Create the list of items
                List<Item> items = new List<Item>();
                string[] startingItems = allLines[1].Split(' ').Skip(2).ToArray();
                foreach (string itemValue in startingItems)
                {
                    string value = Regex.Match(itemValue, @"\d+").ToString();
                    items.Add(new Item(int.Parse(itemValue)));
                }
                
                //Create the Operation
                
                //Assumption about string
                //Operation: new = old [] []
                string[] operationStrings = allLines[2].Split(' ').Skip(4).ToArray();
                string mathOperation = operationStrings[0]; //some math option
                string mathTarget = operationStrings[1]; //can be number or old
            }
        }

        class Monkey
        {
            private List<Item> _items = new List<Item>();
            public List<Item> Items
            {
                get { return _items; }
            }
            private Func<int> _operation;
            private Func<bool> _test;
            private Monkey _trueTarget;
            private Monkey _falseTarget;

            private static int moduloFactor = 1;

            public Monkey(List<Item> monkeyItems, Func<int> operation, Func<bool> test, Monkey trueTarget,
                Monkey falseTarget)
            {
                _items = monkeyItems;
                _operation = operation;
                _test = test;
                _trueTarget = trueTarget;
                _falseTarget = falseTarget;
            }

            public void MultiplyModuloFactor(int moduloFactor)
            {
                moduloFactor = Monkey.moduloFactor * moduloFactor;
            }

            public ThrowItems()
            {
                // For every item, do the operation, inspect (reduce), test, and then give the item to another monkey
                foreach (Item item in _items)
                {
                    Throw(Item);
                }
            }

            private Throw(Item item)
            {
                //Do operation
                item.Worry = _operation();
                
                //Inspect 
                item.Worry = (int) Mathf.Floor( (float)item.Worry / 3.0f );
                
                //Test
                if (_test())
                {
                    GiveItemToMonkey(item, _trueTarget);
                }
                else
                {
                    GiveItemToMonkey(item, _falseTarget);
                }

            }

            private void GiveItemToMonkey(Item item, Monkey monkey)
            {
                monkey.Items.Add(item);
                _items.Remove(item);
            }
        }

        struct Item
        {
            public int Worry;

            public Item(int worry)
            {
                Worry = worry;
            }
        }
        
    }
}