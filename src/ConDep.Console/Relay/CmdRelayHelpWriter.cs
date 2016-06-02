using System.Diagnostics;
using System.IO;
using System.Reflection;
using NDesk.Options;

namespace ConDep.Console.Relay
{
    public class CmdRelayHelpWriter : CmdHelpWriter
    {
        public CmdRelayHelpWriter(TextWriter writer) : base(writer)
        {
        }

        public override void WriteHelp(OptionSet relayOptionsSets, OptionSet deployOptionsSet)
        {
            PrintCopyrightMessage();

            var help = @"
Relay ConDep commands to a ConDep Relay Service.

Usage: 
    ConDep relay <conPack> <environment> <runbook> [-relayOptions] [-deployOptions]

  <conPack>         Assembly (ConPack) containing all your runbooks.
                    If no path to ConPack is specified, first current 
                    directory is searched followed by executing directory. 

  <environment>     File containing environment specific configuration 
                    (e.g. Dev, Test etc). By convention ConDep will look 
                    for <env>.env.json or <env>.env.yml|yaml.  

  <runbook>         Runbook to execute. By convention is the class 
                    name for a class that inherit from Runbook.Local
                    or Runbook.Remote.

Note:

  ConDep Relay expects two files to be present or point to them using /r and /a:

    1. relay.json[yaml]       - Configuration data needed to talk to ConDep Relay Service
    2. artifacts.json[yaml]   - Mainifest file for where artifacts can be found

  ...unless you specify all relay information using options
  below, in which case relay.json is not needed.

";
            _writer.Write(help);
            relayOptionsSets.WriteOptionDescriptions(_writer);

            _writer.Write(@"
Deploy options are the same as for ConDep Deploy but are listed here for your convenience:

");

            deployOptionsSet.WriteOptionDescriptions(_writer);

            _writer.Write(@"
Example:
    
    Example below will require the files relay.json and artifacts.json to present:

        ConDep relay MyAssembly.dll dev MyRunbook

    Example below will eliminate the need for relay.json:

        ConDep relay MyAssembly.dll dev MyRunbook ^
        -o condep-relay-mycompany ^
        -i E0312BE5-97E8-4E8B-A21D-6F17ABD5B6EA ^
        -k condep-relay-mycompany-id ^
        -s tO2bAUz/YaaM6SEEV4VSOrmdx+pGdH6OWzFQQgnFmnA=
");
        }

        public override void PrintCopyrightMessage()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            const int versionAreaLength = 29;
            var version = versionInfo.ProductVersion.Substring(0, versionInfo.ProductVersion.LastIndexOf("."));
            var versionText = string.Format("Version {0} ", version);
            var versionWhitespace = string.Join(" ", new string[versionAreaLength - (versionText.Length)]);

            //ASCII art generated at http://www.network-science.de/ascii/ using Standard font
            _writer.Write(@"
Copyright (c) Jon Arild Torresdal
  ____            ____               ____      _             
 / ___|___  _ __ |  _ \  ___ _ __   |  _ \ ___| | __ _ _   _ 
| |   / _ \| '_ \| | | |/ _ \ '_ \  | |_) / _ \ |/ _` | | | |
| |__| (_) | | | | |_| |  __/ |_) | |  _ <  __/ | (_| | |_| |
 \____\___/|_| |_|____/ \___| .__/  |_| \_\___|_|\__,_|\__, |
" + versionWhitespace + versionText + "|_|                         |___/\n\n");
        }
    }

}