using System;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Web.XmlTransform;
using Svenkle.XMLTransformer.Services;
using Svenkle.XMLTransformer.Services.Interfaces;
using XmlTransformationLogger = Svenkle.XMLTransformer.Services.XmlTransformationLogger;

namespace Svenkle.XMLTransformer
{
    internal static class Program
    {
        private static readonly IXmlTransformationLogger XmlTransformationLogger = new XmlTransformationLogger();
        private static readonly IFileSystem FileSystem = new FileSystem();
        private static readonly IXmlTransformService XmlTransformService = new XmlTransformService(FileSystem, XmlTransformationLogger);

        private static int Main(string[] args)
        {
            var source = args.ElementAtOrDefault(0);
            var transform = args.ElementAtOrDefault(1);
            var destination = args.ElementAtOrDefault(2);

            if (source == null || transform == null)
            {
                Console.WriteLine("Invalid command line parameters");
                Console.WriteLine("Examples");
                Console.WriteLine("Transformer.exe Source.config Transform.config");
                Console.WriteLine("Transformer.exe Source.config Transform.*.config");
                Console.WriteLine("Transformer.exe Source.config Transform.config Destination.config");
                Console.WriteLine("Transformer.exe Source.config Transform.*.config Destination.config");
                return -1;
            }

            try
            {
                XmlTransformService.Transform(source, transform, destination);
                return 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return -1;
            }
        }
    }
}
