using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using NDesk.Options;

namespace ConDep.Console
{
    public class CmdDeployParser : CmdBaseParser<ConDepOptions>
    {
        const int MIN_ARGS_REQUIRED = 3;

        private readonly ConDepOptions _options = new ConDepOptions();
        private readonly OptionSet _optionSet;

        public CmdDeployParser(string[] args) : base(args)
        {
            _optionSet = new OptionSet()
                {
                    {"t=|traceLevel=", "The level of verbosity on output. Valid values are Off, Info, Warning, Error, Verbose. Default is Info.\n", v =>
                        {
                            var traceLevel = ConvertStringToTraceLevel(v);
                            Logger.TraceLevel = traceLevel;
                            _options.TraceLevel = traceLevel;
                        }},
                    {"k=|cryptoKey=", "Key used to decrypt passwords and other sensitive data in ConDep config files.", v=> _options.CryptoKey = v },
                    {"K=|keyFile=", "A file with the .key extension containing a key used to decrypt password and other sensitive data in ConDep config files. The .key file have to contain the decryption key only. If a full path is sent in that can be resolved from current directory, ConDep will use that. If not it will search current folder followed by users home folder.", v=> ResolveCryptoKey(v)},
                    {"q=|webQ=", "Will use ConDep's Web Queue to queue the deployment, preventing multiple deployments to execute at the same time. Useful when ConDep is triggered often from CI environments. Expects the url for the WebQ as its value.\n", v => _options.WebQAddress = v },
                    {"d|deployOnly", "Deploy all except infrastructure\n", v => _options.DeployOnly = v != null},
                    {"b|bypassLB", "Don't use configured load balancer during execution.\n", v => _options.BypassLB = v != null},
                    {"s|sams|stopAfterMarkedServer", "Will only deploy to server marked as StopServer in json config, or first server if no server is marked. After execution, run ConDep with the continueAfterMarkedServer switch to continue deployment to remaining servers.\n", v => _options.StopAfterMarkedServer = v != null},
                    {"c|cams|continueAfterMarkedServer", "Will continue deployment to remaining servers. Used after ConDep has previously executed with the stopAfterMarkedServer switch.\n", v => _options.ContinueAfterMarkedServer = v != null},
                    {"dryrun", "Will output the execution sequence without actually executing it.", v => _options.DryRun = v != null}
                };

        }

        private string ResolveCryptoKey(string keyFile)
        {
            if (string.IsNullOrEmpty(keyFile)) return "";
            if (!keyFile.EndsWith(".key", true, CultureInfo.InvariantCulture)) throw new FileNotFoundException("Key file must have .key extension.");

            if (File.Exists(keyFile))
            {
                return File.OpenText(keyFile).ReadToEnd().Trim();
            }

            var currentDirPath = Path.Combine(Directory.GetCurrentDirectory(), keyFile);
            if (File.Exists(currentDirPath))
            {
                return File.OpenText(currentDirPath).ReadToEnd().Trim();
            }

            var homeFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), keyFile);
            if (File.Exists(homeFolderPath))
            {
                return File.OpenText(currentDirPath).ReadToEnd().Trim();
            }

            throw new FileNotFoundException(string.Format("Could not find file {0}. Searched the following locations: {1}, {2}", keyFile, currentDirPath, homeFolderPath), keyFile);
        }

        public override OptionSet OptionSet
        {
            get { return _optionSet; }
        }

        public override ConDepOptions Parse()
        {
            if (_args.Length < MIN_ARGS_REQUIRED)
                throw new ConDepCmdParseException(string.Format("The Deploy command requires at least {0} arguments.", MIN_ARGS_REQUIRED));

            _options.AssemblyName = _args[0];
            _options.Environment = _args[1];
            _options.Application = _args[2];

            try
            {
                OptionSet.Parse(_args);
            }
            catch (OptionException oe)
            {
                throw new ConDepCmdParseException("Unable to successfully parse arguments.", oe);
            }
            return _options;
        }

        private static TraceLevel ConvertStringToTraceLevel(string traceLevel)
        {
            if (string.IsNullOrWhiteSpace(traceLevel)) return TraceLevel.Info;

            switch (traceLevel.ToLower())
            {
                case "off": return TraceLevel.Off;
                case "warning": return TraceLevel.Warning;
                case "error": return TraceLevel.Error;
                case "verbose": return TraceLevel.Verbose;
                default: return TraceLevel.Info;
            }
        }

        public override void WriteOptionsHelp(TextWriter writer)
        {
            OptionSet.WriteOptionDescriptions(writer);
        }
    }
}