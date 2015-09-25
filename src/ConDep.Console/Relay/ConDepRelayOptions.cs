namespace ConDep.Console.Relay
{
    public class ConDepRelayOptions
    {
        public string Origin { get; set; }
        public string RelayId { get; set; }
        public string AccessKey { get; set; }
        public string AccessSecret { get; set; }
        public string RelayConfigPath { get; set; }
        public string ArtifactManifestPath { get; set; }

        public bool HasAllOptionsSet()
        {
            return
                !string.IsNullOrWhiteSpace(Origin) &&
                !string.IsNullOrWhiteSpace(RelayId) &&
                !string.IsNullOrWhiteSpace(AccessKey) &&
                !string.IsNullOrWhiteSpace(AccessSecret);
        }
    }
}