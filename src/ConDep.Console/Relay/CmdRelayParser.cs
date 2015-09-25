using System.Diagnostics;
using System.IO;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using NDesk.Options;

namespace ConDep.Console.Relay
{
    public class CmdRelayParser : CmdBaseParser<ConDepRelayOptions>
    {
        private OptionSet _optionSet;
        private ConDepOptions _options = new ConDepOptions();
        private readonly ConDepRelayOptions _relayOptions = new ConDepRelayOptions();

        public CmdRelayParser(string[] args) : base(args)
        {
            _optionSet = new OptionSet()
                {
                    {"r=|relayConfig=", "Path to a relay configuration file. If not provided <relay.json> is expected in execution path.", v => _relayOptions.RelayConfigPath = v},
                    {"a=|artifactManifest=", "Path to artifacts manifest file. If not provided <artifacts.json> is expected in execution path.", v => _relayOptions.ArtifactManifestPath = v},
                    {"o=|relayOrigin=", "Name of relay origin. If a <relay.json> file exist, this value will override its value.", v => _relayOptions.Origin = v},
                    {"i=|relayId=", "The relay id. If a <relay.json> file exist, this value will override its value.", v=> _relayOptions.RelayId = v },
                    {"k=|accessKey=", "The access key. If a <relay.json> file exist, this value will override its value.", v=> _relayOptions.AccessKey = v },
                    {"s=|accessSecret=", "The key secret. If a <relay.json> file exist, this value will override its value.", v => _relayOptions.AccessSecret = v }
                };
        }

        public override OptionSet OptionSet
        {
            get { return _optionSet; }
        }

        public override ConDepRelayOptions Parse()
        {
            try
            {
                var deployParser = new CmdDeployParser(_args);
                _options = deployParser.Parse();
                OptionSet.Parse(_args);
            }
            catch (OptionException oe)
            {
                throw new ConDepCmdParseException("Unable to successfully parse arguments.", oe);
            }
            return _relayOptions;
        }

        public override void WriteOptionsHelp(TextWriter writer)
        {
            OptionSet.WriteOptionDescriptions(writer);
        }
    }
}