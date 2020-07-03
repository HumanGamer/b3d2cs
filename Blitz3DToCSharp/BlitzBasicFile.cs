using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private List<string> _types;
        private List<string> _functions;

        public string[] Includes => _includes.ToArray();

        public string[] Constants => _constants.ToArray();

        public string[] Globals => _globals.ToArray();

        public string[] Arrays => _arrays.ToArray();

        public string[] Types => _types.ToArray();

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
            _types = new List<string>();
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
                    case "include":
                        ParseIncludes(line);
                        break;
                    case "const":
                        ParseVarType(line, _constants);
                        break;
                    case "global":
                        ParseVarType(line, _globals);
                        break;
                    case "dim":
                        ParseArray(line);
                        break;
                    case "type":
                        ParseType(line);
                        break;
                    case "function":
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

            _includes.Add(line.Substring(start, end - start).Replace('\\', '/'));
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

        public void ParseType(string line)
        {
            int start = line.IndexOf(' ');
            if (start == -1)
                throw new InvalidFileException();

            start++;

            int end = line.Length;

            _functions.Add(line.Substring(start, end - start));
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

        #region Converting
        public string ToCSharp(BlitzEnvironment environment)
        {
            /*string fullFile = _lines.Join();
            Tokenizer tokenizer = new Tokenizer(fullFile);
            StringBuilder sb = new StringBuilder();
            try
            {
                while (tokenizer.HasNext())
                {
                    char c = tokenizer.Next();
                    if (c == ';')
                        sb.Append("//");
                }
            } catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message + ":\n" + ex.StackTrace);
            }

            return sb.ToString();*/

            List<string> output = new List<string>();
            foreach (var l in _lines)
            {
                var line = l.Trim();

                if (line.StartsWith(";"))
                {
                    if (line.Length > 1)
                        output.Add("//" + line.Substring(1));
                    else
                        output.Add("//");
                    continue;
                }

                if (line.ToLower().StartsWith("global "))
                {
                    int indexInt = line.IndexOf('%');
                    int indexFloat = line.IndexOf('#');
                    int indexString = line.IndexOf('$');
                    int indexEquals = line.IndexOf('=');

                    BlitzType type;

                    string rest = line.Substring(7);
                    string rest2 = rest;
                    if (indexEquals != -1)
                        rest = rest.Substring(0, rest.IndexOf('=')).Trim();

                    rest2 = rest2.Substring(rest.Length).Trim();

                    if (rest.EndsWith('%'))
                    {
                        type = BlitzType.Integer;
                        rest = rest.Substring(0, rest.IndexOf('%'));
                    }
                    else if (rest.EndsWith('#'))
                    {
                        type = BlitzType.Float;
                        rest = rest.Substring(0, rest.IndexOf('#'));
                    }
                    else if (rest.EndsWith('$'))
                    {
                        type = BlitzType.String;
                        rest = rest.Substring(0, rest.IndexOf('$'));
                    }
                    else
                    {
                        type = BlitzType.Integer;
                    }

                    string typeName = "int";
                    if (type == BlitzType.Float)
                        typeName = "float";
                    else if (type == BlitzType.String)
                        typeName = "string";

                    string[] split = rest2.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length > 0)
                        rest += split[0].Trim();

                    if (split.Length > 1)
                        rest2 = " // " + split[1].Trim();
                    else
                        rest2 = "";

                    output.Add(typeName + " " + rest + ";" + rest2);
                    continue;
                }

                output.Add("");
            }

            StringBuilder sb = new StringBuilder();
            foreach(var line in output)
            {
                sb.Append(line + '\n');
            }

            return sb.ToString();
        }
        #endregion
    }
}
