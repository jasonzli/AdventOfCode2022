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
            Directory activeDirectory = null;
            int line = 0;
            foreach (string command in inputs)
            {
                line++;
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

                        if (commandString[2] == "/")
                        {
                            activeDirectory = new Directory("/", null);
                            system.Add("/", activeDirectory);
                            continue;
                        }
                        
                        if (commandString[2] == "..") // change active to parent
                        {
                            Debug.Log($"Line {line}: {command} - {activeDirectory.Parent.Name}");
                            activeDirectory = activeDirectory.Parent;
                            continue;
                        }
                        
                        // Change to new directory
                        string directoryName = commandString[2];
                        if (!system.ContainsKey(directoryName))
                        {
                            Directory newDirectory = new Directory(directoryName, activeDirectory);
                            system.Add(directoryName, newDirectory);
                            activeDirectory?.AddSubdirectory(newDirectory);
                        }
                        
                        activeDirectory = system[directoryName];
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
                    if (!system.ContainsKey(discoveredName))
                    {
                        //create a new directory at this key
                        system.Add(discoveredName, new Directory(discoveredName,activeDirectory));
                    }
                    activeDirectory.AddSubdirectory(system[discoveredName]);
                }
            }

            // Print the sum of all directories whose size is at most 100000
            Debug.Log($"Total size: {system.Values.Where(x => x.DirectorySize() <= 100000).Sum(x => x.DirectorySize())}");
        }

        public class Directory
        {
            public Directory Parent { get; private set; }
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