﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.CommandLine.Completions;
using System.CommandLine.Help;
using System.IO;
using System.Reflection;

namespace System.CommandLine
{
    /// <summary>
    /// Represents the main action that the application performs.
    /// </summary>
    /// <remarks>
    /// Use the RootCommand object without any subcommands for applications that perform one action. Add subcommands 
    /// to the root for applications that require actions identified by specific strings. For example, `dir` does not 
    /// use any subcommands. See <see cref="CliCommand"/> for applications with multiple actions.
    /// </remarks>
    public class CliRootCommand : CliCommand
    {
        private static Assembly? _assembly;
        private static string? _executablePath;
        private static string? _executableName;
        private static string? _executableVersion;

        /// <param name="description">The description of the command, shown in help.</param>
        /// <param name="caseSensitive">Whether the option is case sensitive.</param>
        public CliRootCommand(string description = "", bool caseSensitive = true) : base(ExecutableName, description, caseSensitive)
        {
            Options.Add(new HelpOption());
            Options.Add(new VersionOption()); 
            Directives = new ChildSymbolList<CliDirective>(this)
            {
                new SuggestDirective()
            };
        }
     
        /// <summary>
        /// Represents all of the directives that are valid under the root command.
        /// </summary>
        public IList<CliDirective> Directives { get; }

        /// <summary>
        /// Adds a <see cref="CliDirective"/> to the command.
        /// </summary>
        public void Add(CliDirective directive) => Directives.Add(directive);

        internal static Assembly GetAssembly()
            => _assembly ??= (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly());

        /// <summary>
        /// The name of the currently running executable.
        /// </summary>
        public static string ExecutableName
            => _executableName ??= Path.GetFileNameWithoutExtension(ExecutablePath).Replace(" ", "");

        /// <summary>
        /// The path to the currently running executable.
        /// </summary>
        public static string ExecutablePath => _executablePath ??= Environment.GetCommandLineArgs()[0];
        
        internal static string ExecutableVersion => _executableVersion ??= GetExecutableVersion();

        private static string GetExecutableVersion()
        {
            var assembly = GetAssembly();

            var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (assemblyVersionAttribute is null)
            {
                return assembly.GetName().Version?.ToString() ?? "";
            }
            else
            {
                return assemblyVersionAttribute.InformationalVersion;
            }
        }
    }
}
