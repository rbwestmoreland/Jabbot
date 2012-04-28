using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Web.Hosting;

namespace Jabbot.Core.Sprockets
{
    public static class Container
    {
        private const string ExtensionsFolder = "";//"Sprockets";
        private static CompositionContainer CompositionContainer { get; set; }
        public static IEnumerable<ISprocket> Sprockets { get; set; }

        static Container()
        {
            var container = CreateCompositionContainer();
            Sprockets = container.GetExportedValues<ISprocket>();
        }

        private static CompositionContainer CreateCompositionContainer()
        {
            if (CompositionContainer == null)
            {
                string extensionsPath = GetExtensionsPath();
                var catalog = default(ComposablePartCatalog);

                if (Directory.Exists(extensionsPath))
                {
                    //catalog = new AggregateCatalog(
                    //            new AssemblyCatalog(typeof(ISprocket).Assembly),
                    //            new DirectoryCatalog(extensionsPath, "*.dll"));
                    catalog = new AggregateCatalog(new DirectoryCatalog(extensionsPath, "*.dll"));
                }
                else
                {
                    catalog = new AssemblyCatalog(typeof(ISprocket).Assembly);
                }

                CompositionContainer = new CompositionContainer(catalog);
            }

            return CompositionContainer;
        }

        private static string GetExtensionsPath()
        {
            string rootPath = null;

            if (HostingEnvironment.IsHosted)
            {
                rootPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "bin");
            }
            else
            {
                rootPath = Directory.GetCurrentDirectory();
            }

            return Path.Combine(rootPath, ExtensionsFolder);
        }
    }
}
