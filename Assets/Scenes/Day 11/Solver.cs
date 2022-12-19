using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        [SerializeField] private int _numberOfRounds = 20;
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

            List<Monkey> GameMonkeys = CreateMonkeysFromString(monkeys);

            for (int i = 0; i < _numberOfRounds; i++)
            {
                foreach (Monkey monkey in GameMonkeys)
                {
                    monkey.ThrowItems(GameMonkeys);
                }
            }

            foreach (Monkey monkey in GameMonkeys)
            {
                Debug.Log($"Monkey has this many inspections {monkey.Inspections}");
            }

            GameMonkeys = GameMonkeys.OrderByDescending(monkey => monkey.Inspections).ToList();
            
            long inspectionsFactor = GameMonkeys[0].Inspections * GameMonkeys[1].Inspections;
            
            
            Debug.Log($"Top two monkeys created {inspectionsFactor} inspectionsFactor");
        }

        List<Monkey> CreateMonkeysFromString(string[] monkeyStrings)
        {
            int numberOfMonkeys = monkeyStrings.Length;

            List<Monkey> allMonkeys = new List<Monkey>(numberOfMonkeys);
       
            foreach(string monkeyLine in monkeyStrings)
            {
                string[] lineSeparators = new string[] { "\r\n" , "\n", "\r" };
                
                string[] allLines = monkeyLine.Split(lineSeparators, System.StringSplitOptions.None);

                //Trim the lines
                allLines = allLines.Select(line => line.Trim()).ToArray();
                
                //Monkey ID
                int monkeyID = int.Parse(Regex.Match(allLines[0], @"\d+").ToString());
                
                
                //Create the list of items
                List<Item> items = new List<Item>();
                string[] startingItems = allLines[1].Split(' ').Skip(2).ToArray();
                foreach (string itemValue in startingItems)
                {
                    string value = Regex.Match(itemValue, @"\d+").ToString();
                    items.Add(new Item(int.Parse(value)));
                }

                //Create the Operation
                
                //Assumption about string
                //Operation: new = old [] []
                string[] operationStrings = allLines[2].Split(' ').Skip(4).ToArray();
                string mathOperation = operationStrings[0]; //some math option
                string mathTarget = operationStrings[1]; //can be number or old

                //operation takes an item and does some math to it
                Func<Item,long> operation = (item) =>
                {
                    long value = mathTarget == "old" ? item.Worry : int.Parse(mathTarget);
                    switch (mathOperation)
                    {
                        case "+":
                            return (item.Worry + value) % Monkey.moduloFactor; //this action had to be discovered by trial and error
                        case "*":
                            return (item.Worry * value) % Monkey.moduloFactor;
                        default:
                            return -1;
                    }
                };
                
                //Create the test value
                string[] testStrings = allLines[3].Split(' ').Skip(3).ToArray();
                int divisor = int.Parse(testStrings[0]);
                
                //Test Positive Monkey
                string[] positiveMonkeyStrings = allLines[4].Split(' ').Skip(5).ToArray();
                int positiveMonkey = int.Parse(positiveMonkeyStrings[0]);
                
                //Test Negative Monkey
                string[] negativeMonkeyStrings = allLines[5].Split(' ').Skip(5).ToArray();
                int negativeMonkey = int.Parse(negativeMonkeyStrings[0]);

                allMonkeys.Add(new Monkey(
                    items,
                    operation,
                    divisor,
                    positiveMonkey,
                    negativeMonkey));
            }

            return allMonkeys;
        }

        class Monkey
        {
            private List<Item> _items = new List<Item>();
            public List<Item> Items
            {
                get { return _items; }
            }
            private Func<Item,long> _operation;
            private int _testValue;
            private int _trueTarget;
            private int _falseTarget;
            private int _inspections;
            public long Inspections => _inspections;

            public static int moduloFactor = 1;

            public Monkey(List<Item> monkeyItems, Func<Item,long> operation, int testValue, int trueTarget,
                int falseTarget)
            {
                _items = monkeyItems;
                _operation = operation;
                _testValue = testValue;
                _trueTarget = trueTarget;
                _falseTarget = falseTarget;
                _inspections = 0;
                MultiplyModuloFactor(_testValue);
            }
            
            public void MultiplyModuloFactor(int moduloFactor)
            {
                Monkey.moduloFactor = Monkey.moduloFactor * moduloFactor;
            }

            public void ThrowItems(List<Monkey> monkeys)
            {
                // For every item, do the operation, inspect (reduce), test, and then give the item to another monkey
                foreach (Item item in _items)
                {
                    Throw(item,monkeys);
                }

                _items.Clear();
            }

            private void Throw(Item item, List<Monkey> monkeys)
            {
                //Do operation
                long newWorry = _operation(item);
                
                item.Worry = newWorry;

                //Test
                if (TestItem(item))
                {
                    GiveItemToMonkey(item, monkeys[_trueTarget]);
                }
                else
                {
                    GiveItemToMonkey(item, monkeys[_falseTarget]);
                }

                _inspections++;
            }

            private bool TestItem(Item item)
            {
                return Mathf.Floor(item.Worry % _testValue) == 0;
            }

            private void GiveItemToMonkey(Item item, Monkey monkey)
            {
                monkey.Items.Add(item);
            }
        }

        struct Item
        {
            public long Worry;

            public Item(long worry)
            {
                Worry = worry;
            }
        }
        
    }
}