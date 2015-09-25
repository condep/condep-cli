using System.Collections.Generic;
using ConDep.Execution.Config;

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

            var configFiles = new List<string>();

            if (!string.IsNullOrWhiteSpace(options.Env))
            {
                configFiles.Add(ConfigHandler.GetConDepConfigFile(options.Env, options.Dir));
            }
            else
            {
                configFiles.AddRange(ConfigHandler.GetConDepConfigFiles(options.Dir));    
            }

            foreach (var file in configFiles)
            {
                var crypto = ConfigHandler.ResolveConfigCrypto(file, options.Key);

                System.Console.Out.WriteLine("\tDecrypting file [{0}] ...", file);
                crypto.DecryptFile(file);
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