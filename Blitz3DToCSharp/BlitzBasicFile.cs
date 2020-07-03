using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Blitz3DToCSharp
{
    public class BlitzBasicFile
    {
        #region Variables And Properties
        private string[] _lines;
        private List<string> _includes;
        private List<string> _constants;
        private List<string> _globals;
        private List<string> _arrays;
        private List<string> _functions;

        public string[] Includes => _includes.ToArray();

        public string[] Constants => _constants.ToArray();

        public string[] Globals => _globals.ToArray();

        public string[] Arrays => _arrays.ToArray();

        public string[] Functions => _functions.ToArray();
        #endregion

        #region Initialization
        public BlitzBasicFile(string contents)
        {
            _lines = contents.Replace("\r\n", "\n").Trim().Split('\n');

            _includes = new List<string>();
            _constants = new List<string>();
            _globals = new List<string>();
            _arrays = new List<string>();
            _functions = new List<string>();

            Parse();
        }

        public static BlitzBasicFile Read(string path)
        {
            return new BlitzBasicFile(File.ReadAllText(path));
        }
        #endregion

        #region Parsing
        private void Parse()
        {
            foreach (var l in _lines)
            {
                var line = l.Trim();

                if (line.StartsWith(";"))
                    continue;

                string[] splitLine = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (splitLine.Length < 1)
                    continue;

                string keyword = splitLine[0].ToLower();

                switch(keyword)
                {
                    case "Include":
                        ParseIncludes(line);
                        break;
                    case "Const":
                        ParseVarType(line, _constants);
                        break;
                    case "Global":
                        ParseVarType(line, _globals);
                        break;
                    case "Dim":
                        ParseArray(line);
                        break;
                    case "Function":
                        ParseFunction(line);
                        break;
                }
            }
        }

        public void ParseIncludes(string line)
        {
            bool singleQuotes = false;
            int start = line.IndexOf("\"");
            if (start == -1)
            {
                singleQuotes = true;
                start = line.IndexOf("'");
            }

            if (start == -1)
                throw new InvalidFileException();

            start++;

            int end = singleQuotes ? line.LastIndexOf("'") : line.LastIndexOf("\"");
            if (end == -1)
                throw new InvalidFileException();

            _includes.Add(line.Substring(start, end - start).ToLower().Replace('\\', '/'));
        }

        public void ParseVarType(string line, List<string> list)
        {
            int start = line.IndexOf(' ');
            if (start == -1)
                throw new InvalidFileException();

            start++;

            int end = line.LastIndexOf("=");
            if (end == -1)
                end = line.Length;

            list.Add(line.Substring(start, end - start));
        }

        public void ParseArray(string line)
        {
            int start = line.IndexOf(' ');
            if (start == -1)
                throw new InvalidFileException();

            start++;

            int end = line.LastIndexOf("(");
            if (end == -1)
                throw new InvalidFileException();

            _arrays.Add(line.Substring(start, end - start));
        }

        public void ParseFunction(string line)
        {
            int start = line.IndexOf(' ');
            if (start == -1)
                throw new InvalidFileException();

            start++;

            int end = line.LastIndexOf("(");
            if (end == -1)
                throw new InvalidFileException();

            _functions.Add(line.Substring(start, end - start));
        }

        #endregion

        public string ToCSharp(BlitzEnvironment environment)
        {
            return "";
        }
    }
}
