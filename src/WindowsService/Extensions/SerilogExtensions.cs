﻿using Serilog;
using Serilog.Configuration;
using System.Reflection;

namespace Havagan.Example.CoreWindowsServiceOnFullFramework
{
    internal static class SerilogExtensions
    {
        /// <summary>
        /// Global application name.
        /// </summary>
        private static readonly string _applicationName = new Lazy<string>(() => Program.ApplicationName).Value;

        /// <summary>
        /// Global application assembly.
        /// </summary>
        private static readonly AssemblyName _assembly = new Lazy<AssemblyName>(() => Assembly.GetEntryAssembly()?.GetName()).Value;

        /// <summary>
        /// Global application assembly name.
        /// </summary>
        private static readonly string _assemblyName = new Lazy<string>(() => _assembly?.Name?.ToString()).Value;

        /// <summary>
        /// Global application assembly version.
        /// </summary>
        private static readonly string _assemblyVersion = new Lazy<string>(() => _assembly?.Version?.ToString()).Value;

        /// <summary>
        /// Global service trace ID.
        /// </summary>
        private static readonly string _serviceTraceId = new Lazy<string>(() => Guid.NewGuid().ToString()).Value;

        /// <summary>
        /// Enriches the Serilog logger with global properties.
        /// </summary>
        /// <param name="enrich"></param>
        /// <returns></returns>
        internal static LoggerConfiguration WithGlobalProperties(this LoggerEnrichmentConfiguration enrich)
        {
            if (enrich is null)
            {
                throw new ArgumentNullException(nameof(enrich));
            }

            enrich.WithProperty("ServiceTraceId", _serviceTraceId);
            enrich.WithProperty("Application", _applicationName);
            enrich.WithProperty("ExecutingAssemblyName", _assemblyName);

            return enrich.WithProperty("ExecutingAssemblyVersion", _assemblyVersion);
        }
    }
}
