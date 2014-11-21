using System.IO;
using NDesk.Options;

namespace ConDep.Console.Encrypt
{
    public class CmdEncryptHelpWriter : CmdHelpWriter
    {
        public CmdEncryptHelpWriter(TextWriter writer) : base(writer)
        {
        }

        public override void WriteHelp(OptionSet optionSet)
        {
            PrintCopyrightMessage();

            var help = @"
Encrypt sensitive data in json config files and return 
key for decryption. Use this key when using condep deploy,
so ConDep can decrypt the environment file before execution.

Usage: ConDep encrypt [-options]

If no options is specified, all ConDep *.env.json files 
in the current directory will be encrypted.

where options include:

";
            _writer.Write(help);

            optionSet.WriteOptionDescriptions(_writer);
            _writer.WriteLine();
        }
    }
}