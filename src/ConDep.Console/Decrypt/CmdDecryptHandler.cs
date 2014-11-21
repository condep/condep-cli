using System.Collections.Generic;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Security;

namespace ConDep.Console.Decrypt
{
    public class CmdDecryptHandler : IHandleConDepCommands
    {
        private CmdDecryptParser _parser;
        private CmdDecryptValidator _validator;
        private CmdDecryptHelpWriter _helpWriter;

        public CmdDecryptHandler(string[] args)
        {
            _parser = new CmdDecryptParser(args);
            _validator = new CmdDecryptValidator();
            _helpWriter = new CmdDecryptHelpWriter(System.Console.Out);
        }

        public void Execute(CmdHelpWriter helpWriter)
        {
            var options = _parser.Parse();
            _validator.Validate(options);

            _helpWriter.PrintCopyrightMessage();

            var crypto = new JsonPasswordCrypto(options.Key);

            var configParser = new EnvConfigParser();
            var configFiles = new List<string>();

            if (!string.IsNullOrWhiteSpace(options.Env))
            {
                configFiles.Add(configParser.GetConDepConfigFile(options.Env, options.Dir));
            }
            else
            {
                configFiles.AddRange(configParser.GetConDepConfigFiles(options.Dir));    
            }
            foreach (var file in configFiles)
            {
                System.Console.Out.WriteLine("\tDecrypting file [{0}] ...", file);
                configParser.DecryptFile(file, crypto);
                System.Console.Out.WriteLine("\tFile decrypted.");
                System.Console.WriteLine();
            }
        }

        public void WriteHelp()
        {
            _helpWriter.WriteHelp(_parser.OptionSet);
        }

        public void Cancel()
        {
            
        }
    }
}