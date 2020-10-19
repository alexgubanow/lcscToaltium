using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace lcsclib
{
    public class SVGCommand
    {
        public char CMD { get; private set; }
        public double[] ARG { get; private set; }

        public SVGCommand(char command, params double[] arguments)
        {
            CMD = command;
            ARG = arguments;
        }

        public static bool TryParse(string SVGpath, out List<SVGCommand> cmdList)
        {
            cmdList = new List<SVGCommand>();
            string separators = @"(?=[MZLHVCSQTAmzlhvcsqta])";
            var tokens = Regex.Split(SVGpath, separators).Where(t => !string.IsNullOrEmpty(t)).ToArray();
            if (tokens.Count() == 0)
            {
                throw new Exception("failed to find any svg cmds in given string");
            }
            for (int i = 0; i < tokens.Count(); i++)
            {
                var cmd = tokens[i].Split(' ');
                if (cmd[0].Length > 1)
                {
                    throw new Exception("failed to get svg cmd");
                }
                if (cmd.Length == 0)
                {
                    throw new Exception("failed to split svg parameters for given cmd");
                }
                List<double> args = new List<double>();
                for (int j = 1; j < cmd.Length; j++)
                {
                    if (double.TryParse(cmd[j], out double arg))
                    {
                        args.Add(arg);
                    }
                }
                cmdList.Add(new SVGCommand(cmd[0][0], args.ToArray()));
            }
            if (cmdList.Last().CMD == 'Z')
            {
                cmdList.Remove(cmdList.Last());
            }
            return true;
        }
    }
}
