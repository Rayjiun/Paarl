using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paarl.Utility;

namespace Paarl
{
    internal static class LOD
    {
        private static readonly List<string> bulletCollision = new()
        {
            "None",
            "High",
            "Medium",
            "Low",
            "Lowest"
        };

        private static readonly List<string> manualLODs = new()
        {
            "lowestLod",
            "lowLod",
            "mediumLod"
        };

        private static readonly int xmodelBlockSize = 214;

        /// <summary>
        /// Determines how many LODs the model should have
        /// </summary>
        /// <param name="index">Index to continue reading the file from</param>
        /// <param name="content">File content</param>
        /// <returns></returns>
        private static int DetermineLODLevel(int index = 0, string[]? content = null)
        {
            if(index == 0 || content == null)
            {
                return 4;
            }

            for (int i = index; i < index + xmodelBlockSize; i++)
            {
                string line = content[i];

                if (line.Contains("filename"))
                {
                    string modelString = line.Split("\"")[3];
                    string root = Location.GetBlackOpsRoot()!;

                    if (modelString.Contains("_custom\\")) // Agreed upon custom root folder. I could potentially read the converted bin dir but fuck that.
                    {
                        modelString = modelString.Replace("..\\\\", "");
                        modelString = Path.Combine(root, modelString);
                    }
                    else
                    {
                        modelString = Path.Combine(root, "model_export\\", modelString);
                    }

                    if(!File.Exists(modelString))
                    {
                        Print.WriteWarning($"{modelString} does not exist");
                        break;
                    }

                    FileInfo xmodel = new(modelString);

                    // TODO: Read vert count of models by reading the xmodel_bin format instead
                    if(xmodel.Length > 0 && xmodel.Length < 500000)
                    {
                        return 4;
                    }
                    else if (xmodel.Length > 500000 && xmodel.Length < 1000000)
                    {
                        return 5;
                    }
                    else
                    {
                        return 6;
                    }
                }
            }

            return 4;
        }

        /// <summary>
        /// Checks if the model already has manual LODs
        /// </summary>
        /// <param name="index">Index to continue reading the file from</param>
        /// <param name="content">File content</param>
        /// <returns></returns>
        private static bool HasManualLODs(int index, string[] content)
        {
            for (int i = index; i < index + xmodelBlockSize; i++)
            {
                string line = content[i];

                if (manualLODs.Any(line.Contains))
                {
                    if (line.ToLower().Contains("xmodel_bin"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Parses the GDT file
        /// </summary>
        /// <param name="file">GDT file</param>
        public static void Parse(string file)
        {
            string[] content = File.ReadAllLines(file);
            int LODLevel = DetermineLODLevel();

            Stopwatch watch = new();
            watch.Start();

            for (int i = 0; i < content.Length; i++)
            {
                string line = content[i];

                if (line.Contains("xmodel.gdf"))
                {
                    if (HasManualLODs(i, content))
                    {
                        Print.WriteInfo("Model has manual LODs, skip parse");
                        i += xmodelBlockSize;
                        continue;
                    }

                    LODLevel = DetermineLODLevel(i, content);
                }

                if (line.Contains("LODBiasPercent")) // Just to make the LODs not pop as visibly, this isn't good for performance but it doesn't hurt much for the quality increase it provides.
                {
                    if(line.Contains("0.0"))
                    {
                        line = line.Replace("0.0", "-1.0");
                    }
                    else
                    {
                        line = line.Replace("0", "-1.0");
                    }
                }
                else if(line.Contains("Percent"))
                {
                    continue;
                }

                if (line.Contains("autogen") && line.Contains("Lod"))
                {
                    if (line.Contains("Low") || line.Contains("Medium") || line.Contains("High")) // These are LOD 1-3, we always want these enabled
                    {
                        line = line.Replace("0", "1");
                    }
                    else
                    {
                        bool enableLODLevel = false;
                        for (int x = 0; x < line.Length; x++)
                        {
                            char c = line[x];
                            if (char.IsDigit(c))
                            {
                                enableLODLevel = LODLevel >= char.GetNumericValue(c);
                                break; // Break out the moment it's a digit, because otherwise we'll hit the 0/1 bool later on
                            }
                        }

                        if (enableLODLevel)
                        {
                            line = line.Replace("0", "1");
                        }
                    }
                }
                else if(line.Contains("BulletCollisionLOD"))
                {
                    string keyLOD = "";
                    foreach (string LOD in bulletCollision)
                    {
                        if(line.Contains(LOD))
                        {
                            keyLOD = LOD;
                        }
                    }

                    // We could add an else to make it the 2nd highest LOD for bullet collision, but if it's already above this it'll more than likely suffice.
                    if (keyLOD != "")
                    {
                        string finalLOD = "LOD" + LODLevel;
                        if (LODLevel == 4)
                        {
                            finalLOD = bulletCollision[LODLevel];
                        }

                        line = line.Replace(keyLOD, finalLOD);
                    }
                }

                content[i] = line; // Push our altered string into the GDT to write out later
            }

            File.WriteAllLines(file, content);

            watch.Stop();
            Print.WriteSuccess($"Finished parsing {Path.GetFileName(file)}");
            Print.WriteInfo($"Time elapsed: {watch.Elapsed}");
        }
    }
}
