using System;
using System.Collections.Generic;
using System.IO;
using ConDep.Console.Deploy;
using ConDep.Dsl.Config;
using ConDep.Execution.Config;
using ConDep.Execution.Relay;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConDep.Console.Relay
{
    public class CmdRelayHandler : IHandleConDepCommands
    {
        private readonly CmdRelayParser _relayParser;
        private readonly CmdRelayValidator _relayValidator;
        private readonly CmdRelayHelpWriter _helpWriter;
        private readonly CmdDeployParser _deployParser;
        private readonly CmdDeployValidator _deployValidator;

        public CmdRelayHandler(string[] args)
        {
            _relayParser = new CmdRelayParser(args);
            _deployParser = new CmdDeployParser(args);

            _relayValidator = new CmdRelayValidator();
            _deployValidator = new CmdDeployValidator();

            _helpWriter = new CmdRelayHelpWriter(System.Console.Out);
        }

        public void Execute(CmdHelpWriter helpWriter)
        {
            var failed = false;

            try
            {
                var deployOptions = GetDeployOptions(_deployParser, _deployValidator);
                var relayOptions = GetRelayOptions(_relayParser, _relayValidator, deployOptions);
                var relayConfig = GetRelayConfig(relayOptions, deployOptions);
                var artifactManifest = GetArtifactManifest(relayOptions, deployOptions);

                var status = new ConDepStatus();

                var handler = new RelayHandler();
                var result = handler.Relay(artifactManifest, relayConfig, new DeployOptions
                {
                    AssemblyName = deployOptions.AssemblyName,
                    BypassLB = deployOptions.BypassLB,
                    ContinueAfterMarkedServer = deployOptions.ContinueAfterMarkedServer,
                    CryptoKey = deployOptions.CryptoKey,
                    DryRun = deployOptions.DryRun,
                    Environment = deployOptions.Environment,
                    Runbook = deployOptions.Runbook,
                    SkipHarvesting = deployOptions.SkipHarvesting,
                    StopAfterMarkedServer = deployOptions.StopAfterMarkedServer,
                    TraceLevel = deployOptions.TraceLevel.ToString(),
                    WebQAddress = deployOptions.WebQAddress
                });

                status.EndTime = DateTime.Now;

                if (result.Cancelled || result.Success)
                {
                    status.PrintSummary();
                }
                else
                {
                    status.PrintSummary();
                    failed = true;
                }
            }
            finally
            {
                if (failed)
                {
                    Environment.Exit(1);
                }

            }
        }

        private ArtifactManifest GetArtifactManifest(ConDepRelayOptions options, ConDepOptions deployOptions)
        {
            var path = !string.IsNullOrWhiteSpace(options.ArtifactManifestPath) ? options.ArtifactManifestPath : Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "artifacts.json");

            var parser =
                new ArtifactManifestConfigParser(
                    new JsonSerializer<ArtifactManifest>(new JsonConfigCrypto(deployOptions.CryptoKey)));
            var manifest = parser.GetTypedConfig(path);
            return manifest;
        }

        private RelayConfig GetRelayConfig(ConDepRelayOptions relayOptions, ConDepOptions deployOptions)
        {
            if (relayOptions.HasAllOptionsSet())
                return new RelayConfig
                {
                    AccessKey = relayOptions.AccessKey,
                    AccessSecret = relayOptions.AccessSecret,
                    Origin = relayOptions.Origin,
                    RelayId = relayOptions.RelayId
                };

            var path = !string.IsNullOrWhiteSpace(relayOptions.RelayConfigPath) ? relayOptions.RelayConfigPath : Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "relay.json");
            if (!File.Exists(path)) throw new FileNotFoundException("");

            var serializer = new JsonSerializer<RelayConfig>(new JsonConfigCrypto(deployOptions.CryptoKey));
            return serializer.DeSerialize(File.OpenRead(path));
        }

        private ConDepRelayOptions GetRelayOptions(CmdBaseParser<ConDepRelayOptions> relayParser, CmdBaseValidator<ConDepRelayOptions> relayValidator, ConDepOptions deployOptions)
        {
            var relayOptions = relayParser.Parse();
            relayValidator.Validate(relayOptions);
            return relayOptions;
        }

        private ConDepOptions GetDeployOptions(CmdBaseParser<ConDepOptions> deployParser, CmdBaseValidator<ConDepOptions> deployValidator)
        {
            var deployOptions = deployParser.Parse();
            deployValidator.Validate(deployOptions);

            return deployOptions;
        }

        public void WriteHelp()
        {
            _helpWriter.WriteHelp(_relayParser.OptionSet, _deployParser.OptionSet);
        }

        public void Cancel()
        {
            throw new System.NotImplementedException();
        }
    }

    public class ConDepMissingRelayConfigException : Exception
    {
        public ConDepMissingRelayConfigException(IEnumerable<string> missingOptions)
            : base("Missing mandatory options for " + string.Join(", ", missingOptions))
        {
        }
    }

}