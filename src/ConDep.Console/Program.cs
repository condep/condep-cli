﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using ConDep.Dsl.Logging;
using ConDep.Execution.Logging;

namespace ConDep.Console
{
    sealed internal class Program
    {
        private static IHandleConDepCommands _handler;

        static void Main(string[] args)
        {
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
            new Logger().ResolveLogger(new LogResolver());
            Logger.TraceLevel = TraceLevel.Info;
        }

        private static void ExecuteCommand(string[] args)
        {
            CmdHelpWriter helpWriter = null;

            try
            {
                _handler = CmdFactory.Resolve(args);
                helpWriter = _handler.HelpWriter;
                _handler.Execute(helpWriter);
            }
            catch (AggregateException aggEx)
            {
                foreach (var ex in aggEx.InnerExceptions)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    if (helpWriter == null) helpWriter = new CmdHelpWriter(System.Console.Out);
                    helpWriter.WriteException(ex);
                    System.Console.ResetColor();
                    System.Console.WriteLine("For help type ConDep Help <command>");
                }
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                if(helpWriter == null) helpWriter = new CmdHelpWriter(System.Console.Out);
                helpWriter.WriteException(ex);
                System.Console.ResetColor();
                System.Console.WriteLine("For help type ConDep Help <command>");
                Environment.Exit(1);
            }
        }
    }
}
