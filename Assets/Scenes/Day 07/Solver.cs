using System;
using System.Collections.Generic;
using System.Linq;
using SharedUtility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes.Day_07
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
            Dictionary<string, Directory> system = new Dictionary<string, Directory>();

            Dictionary<string, Directory> allDirectories = new Dictionary<string, Directory>();
            system.Add("/", new Directory("/", null));
            Directory activeDirectory = system["/"];
            
            foreach (string command in inputs)
            {
                
                string[] commandString = command.Split(' ');
                if (commandString[0] == "$") //command
                {
                    
                    if (commandString[1] == "ls")
                    {
                        // Do nothing because we're about to add a bunch of stuff
                        continue;
                    }
                    
                    if (commandString[1] == "cd") // move or create
                    {

                        if (commandString[2] == "..") // change active to parent
                        {
                            if (activeDirectory.Parent == null)
                            {
                                activeDirectory = system["/"];
                            }
                            else
                            {
                                activeDirectory = activeDirectory.Parent;
                            }

                            continue;
                        }

                        if (commandString[2] == "/")
                        {
                            activeDirectory = system["/"];
                            continue;
                        }
                        
                        // Change to new directory
                        string directoryName = commandString[2];
                        activeDirectory = activeDirectory.SubDirectories.Find(directory => directory.Name == directoryName);
                        
                    }
                }

                if (int.TryParse(commandString[0], out int size)) //file declared
                {
                    File newFile = new File(commandString[1], size);
                    activeDirectory.AddFile(newFile);
                }

                if (commandString[0] == "dir")
                {
                    string discoveredName = commandString[1];
                    // if unable to find the directory in the subdirectories create it and add it to the subdirectories
                    if (activeDirectory.SubDirectories.Find(directory => directory.Name == discoveredName) == null)
                    {
                        Directory newDirectory = new Directory(discoveredName, activeDirectory);
                        allDirectories.TryAdd(newDirectory.FullName, newDirectory);
                        activeDirectory.AddSubdirectory(newDirectory);
                    }
                }
            }

            // Print the sum of all directories whose size is at most 100000
            Debug.Log($"Total size: {system["/"].DirectorySize()}");
            
            // Print the sum of all directories from allDirectories whose size is at most 100000
            Debug.Log($"Total size: {allDirectories.Values.Where(directory => directory.DirectorySize() <= 100000).Sum(directory => directory.DirectorySize())}");


            int totalSpace = 70000000;
            int neededSpace = 30000000;
            int usedSpace = system["/"].DirectorySize();
            int freeSpace = totalSpace - usedSpace;
            int spaceDeficit = Math.Abs(neededSpace - freeSpace);
            Directory smallestViableDirectory = allDirectories.Values
                .Where(directory => directory.DirectorySize() >= spaceDeficit)
                .OrderBy(directory => directory.DirectorySize()).First();
            
            // Find the smallest directory that can be deleted to free up enough space
            Debug.Log($"Smallest directory that can be deleted to free up enough space is {smallestViableDirectory.FullName} with size {smallestViableDirectory.DirectorySize()}");

        }

        public class Directory
        {
            public Directory Parent { get; private set; }
            public string FullName =>$"{Parent?.Name} {Name}".Trim();
            public string Name { get; private set; }
            public List<Directory> SubDirectories { get; private set; }
            public List<File> Files { get; private set; }

            public Dictionary<string, int> DirectorySizes { get; private set; } = new Dictionary<string, int>();

            public Directory(string name, Directory parent)
            {
                Parent = parent;
                Name = name;
                SubDirectories = new List<Directory>();
                Files = new List<File>();
            }
            

            public void AddSubdirectory(Directory subdirectory)
            {
                if (SubDirectories.Contains(subdirectory))
                {
                    Debug.Log($"Directory {subdirectory.Name} already exists in {Name}");
                    return;
                }
                
                SubDirectories.Add(subdirectory);
            }

            public void AddFile(File file)
            {
                if (Files.Contains(file))
                {
                    Debug.Log($"File {file.Name} already exists in {Name}");
                    return;
                }
                Files.Add(file);
            }

            public int DirectorySize()
            {
                if (DirectorySizes.ContainsKey(Name))
                {
                    return DirectorySizes[Name];
                }
                int size = Files.Sum(file => file.Size);
                
                foreach (Directory subDirectory in SubDirectories)
                {
                    size += subDirectory.DirectorySize();
                }
                
                DirectorySizes.Add(Name, size);

                return size;
            }
        }

        public class File
        {
            public string Name { get; private set; }
            public int Size { get; private set; }

            public File(string name, int size)
            {
                Name = name;
                Size = size;
            }
        }
    }
}