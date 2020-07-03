using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blitz3DToCSharp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: Blitz3DToCSharp <input.bb> [output dir]");
                return;
            }
            string inputFile = args[0].Replace('\\', '/');
            string outputFolder = "output";
            if (args.Length > 1)
                outputFolder = args[1];

            outputFolder = Path.GetFullPath(outputFolder).Replace('\\', '/');
            if (!outputFolder.EndsWith('/'))
                outputFolder += '/';

            string inputFolder = Path.GetDirectoryName(inputFile).Replace('\\', '/');
            if (!string.IsNullOrEmpty(inputFolder))
                inputFolder += '/';

            Dictionary<string, BlitzBasicFile> files = new Dictionary<string, BlitzBasicFile>();

            BlitzBasicFile mainFile = BlitzBasicFile.Read(inputFile);
            files.Add(inputFile, mainFile);

            List<string> includes = new List<string>();
            includes.AddRange(mainFile.Includes);

            for (int i = 0; i < includes.Count; i++)
            {
                string fileName = inputFolder + includes[i];
                if (files.ContainsKey(fileName))
                    continue;
                BlitzBasicFile file = BlitzBasicFile.Read(fileName);
                files.Add(fileName, file);
                foreach (var include2 in file.Includes)
                {
                    if (!includes.Contains(include2))
                        includes.Add(include2);
                }
            }

            BlitzEnvironment environment = new BlitzEnvironment(files.Values.ToArray());

            Dictionary<string, string> CSharpOutput = new Dictionary<string, string>();
            foreach (var file in files)
            {
                string fileName = Path.ChangeExtension(file.Key, "cs").Replace("-", "");
                CSharpOutput.Add(fileName, file.Value.ToCSharp(environment));
            }

            foreach (var output in CSharpOutput)
            {
                var outputPath = outputFolder + output;
                using (Stream s = File.Open(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (StreamWriter sw = new StreamWriter(s))
                {
                    sw.Write(output.Value);
                }
            }
        }
    }
}
