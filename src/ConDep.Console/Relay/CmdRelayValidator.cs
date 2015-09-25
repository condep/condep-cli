using System.IO;

namespace ConDep.Console.Relay
{
    public class CmdRelayValidator : CmdBaseValidator<ConDepRelayOptions>
    {
        public override void Validate(ConDepRelayOptions options)
        {
            ValidateRelaySettings(options);
            ValidateArtifactManifest(options);
        }

        private void ValidateArtifactManifest(ConDepRelayOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.ArtifactManifestPath))
            {
                if (!File.Exists(options.RelayConfigPath)) throw new FileNotFoundException("Artifact manifest path specified not found.", options.ArtifactManifestPath);
            }

            var path = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "artifacts.json");
            if (!File.Exists(path)) throw new FileNotFoundException("Artifact manifest [artifacts.json] not found in execution path.", path);
        }

        private void ValidateRelaySettings(ConDepRelayOptions options)
        {
            if (!RelaySettingsDefinedInParams(options))
            {
                ValidateRelayConfigFilePath(options);
            }
        }

        private void ValidateRelayConfigFilePath(ConDepRelayOptions options)
        {
            if (!string.IsNullOrWhiteSpace(options.RelayConfigPath))
            {
                if(!File.Exists(options.RelayConfigPath)) throw new FileNotFoundException("Relay config path specified not found.", options.RelayConfigPath);
            }

            var path = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "relay.json");
            if(!File.Exists(path)) throw new FileNotFoundException("Relay config [relay.json] not found in execution path.", path);
        }

        private bool RelaySettingsDefinedInParams(ConDepRelayOptions options)
        {
            return
                !string.IsNullOrWhiteSpace(options.AccessKey) &&
                !string.IsNullOrWhiteSpace(options.AccessSecret) &&
                !string.IsNullOrWhiteSpace(options.Origin) &&
                !string.IsNullOrWhiteSpace(options.RelayId);
        }
    }
}