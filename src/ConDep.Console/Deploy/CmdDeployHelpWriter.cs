using System.IO;
using NDesk.Options;

namespace ConDep.Console
{
    public class CmdDeployHelpWriter : CmdHelpWriter
    {
        public CmdDeployHelpWriter(TextWriter writer) : base(writer)
        {
        }

        public override void WriteHelp(OptionSet optionSet)
        {
            PrintCopyrightMessage();

            var help = @"
Deploy files and infrastructure to remote servers and environments

Usage: ConDep deploy <assembly> <environment> <application> [-options]

  <assembly>        Assembly containing deployment setup
                    If no path to assembly is specified, first current 
                    directory is searched followed by executing directory. 

  <environment>     File containing environment specific configuration 
                    (e.g. Dev, Test etc). By convention ConDep will look 
                    for <env>.env.json or <env>.env.yml|yaml.  

  <artifact>        Artifact to execute. By convention is the same name
                    as the Artifact class.

where options include:

";
            _writer.Write(help);
            optionSet.WriteOptionDescriptions(_writer);

            var help2 = @"
Note: 

    If configuration file is encrypted and no key specified 
    (using /k or /f) ConDep will search the user home folder for 
    .<assemblyName>.key by convention.

Example:

    ConDep deploy MyAssembly.dll dev MyArtifact
";
            _writer.Write(help2);
        }
    }
}