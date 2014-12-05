// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Microsoft.Framework.DependencyInjection.ServiceLookup;
using Microsoft.Framework.Runtime;

namespace Microsoft.AspNet.Mvc
{
    public class RazorPreCompileModule : ICompileModule
    {
        private readonly IServiceProvider _appServices;

        public RazorPreCompileModule(IServiceProvider services)
        {
            _appServices = services;
        }

        protected virtual string FileExtension { get; } = ".cshtml";

        public virtual void BeforeCompile(IBeforeCompileContext context)
        {
            var appEnv = _appServices.GetRequiredService<IApplicationEnvironment>();

            var folderPath = Path.Combine(
                    Path.GetTempPath(),
                    "ProjectKLogs");

            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, appEnv.ApplicationName) + ".pre.log";

            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 1024, false))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                WriteMessage(writer, "Start Pre compilation");

                var setup = new RazorViewEngineOptionsSetup(appEnv);
                var sc = new ServiceCollection();
                sc.ConfigureOptions(setup);
                sc.AddMvc();

                WriteMessage(writer, "Create Pre Compiler");

                var viewCompiler = new RazorPreCompiler(BuildFallbackServiceProvider(sc, _appServices));

                WriteMessage(writer, "Compile Views");

                viewCompiler.CompileViews(context);

                WriteMessage(writer, "Compile Views completed");
            }
        }

        private void WriteMessage(StreamWriter writer, string message)
        {
            writer.WriteLine("{0}, {1}, {2}", DateTime.Now.ToString("hh:mm:ss:ffff"), "PreCompile", message);
            writer.Flush();
        }

        public void AfterCompile(IAfterCompileContext context)
        {
        }

        // TODO: KILL THIS
        private static IServiceProvider BuildFallbackServiceProvider(IEnumerable<IServiceDescriptor> services, IServiceProvider fallback)
        {
            var sc = HostingServices.Create(fallback);
            sc.Add(services);

            // Build the manifest
            var manifestTypes = services.Where(t => t.ServiceType.GetTypeInfo().GenericTypeParameters.Length == 0
                    && t.ServiceType != typeof(IServiceManifest)
                    && t.ServiceType != typeof(IServiceProvider))
                    .Select(t => t.ServiceType).Distinct();
            sc.AddInstance<IServiceManifest>(new ServiceManifest(manifestTypes, fallback.GetRequiredService<IServiceManifest>()));
            return sc.BuildServiceProvider();
        }

        private class ServiceManifest : IServiceManifest
        {
            public ServiceManifest(IEnumerable<Type> services, IServiceManifest fallback = null)
            {
                Services = services;
                if (fallback != null)
                {
                    Services = Services.Concat(fallback.Services).Distinct();
                }
            }

            public IEnumerable<Type> Services { get; private set; }
        }
    }
}

namespace Microsoft.Framework.Runtime
{
    [AssemblyNeutral]
    public interface ICompileModule
    {
        void BeforeCompile(IBeforeCompileContext context);

        void AfterCompile(IAfterCompileContext context);
    }

    [AssemblyNeutral]
    public interface IAfterCompileContext
    {
        CSharpCompilation CSharpCompilation { get; set; }

        IList<Diagnostic> Diagnostics { get; }
    }
}