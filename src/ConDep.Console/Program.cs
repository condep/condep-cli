using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using ConDep.Dsl.Logging;

namespace ConDep.Console
{
    sealed internal class Program
    {
        private static IHandleConDepCommands _handler;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveNewtonsoftJson;

            System.Console.OutputEncoding = Encoding.GetEncoding(1252);
            var exitCode = 0;
            System.Console.CancelKeyPress += Console_CancelKeyPress;

            try
            {
                ConfigureLogger();
                ExecuteCommand(args);
            }
            catch (Exception ex)
            {
                exitCode = 1;
                Logger.Error("ConDep reported a fatal error:");
                Logger.Error("Message: " + ex.Message);
                Logger.Verbose("Stack trace:\n" + ex.StackTrace);
            }
            Environment.ExitCode = exitCode;
        }

        //Runtime replacement for:
        //<assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
        //<bindingRedirect oldVersion="0.0.0.0-6.0.0.0" newVersion="6.0.0.0" />
        private static Assembly ResolveNewtonsoftJson(object sender, ResolveEventArgs args)
        {
            var requestedName = new AssemblyName(args.Name);

            if (requestedName.Name == "Newtonsoft.Json")
            {
                if (requestedName.Version.Major >= 0 && requestedName.Version.Major <= 6)
                {
                    return Assembly.LoadFrom("Newtonsoft.Json.dll");
                }
            }
            return null;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Logger.Warn("I'm exiting now because you force my hand!");
            if (_handler != null)
            {
                Logger.Warn("Cancelling handler!");
                _handler.Cancel();
            }
        }

        private static void ConfigureLogger()
        {
            new LogConfigLoader().Load();
            new Logger().AutoResolveLogger();
            Logger.TraceLevel = TraceLevel.Info;
        }

        private static void ExecuteCommand(string[] args)
        {
            var helpWriter = new CmdHelpWriter(System.Console.Out);

            try
            {
                _handler = CmdFactory.Resolve(args);
                _handler.Execute(helpWriter);
            }
            catch (AggregateException aggEx)
            {
                foreach (var ex in aggEx.InnerExceptions)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    helpWriter.WriteException(ex);
                    System.Console.ResetColor();
                    System.Console.WriteLine("For help type ConDep Help <command>");
                }
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                helpWriter.WriteException(ex);
                System.Console.ResetColor();
                System.Console.WriteLine("For help type ConDep Help <command>");
                Environment.Exit(1);
            }
        }
    }
}
