using System;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Web.XmlTransform;
using Svenkle.XMLTransformer.Services;
using Svenkle.XMLTransformer.Services.Interfaces;

namespace Svenkle.XMLTransformer
{
    internal static class Program
    {
        private static readonly IXmlTransformationLogger XmlTransformationLogger = new Services.XmlTransformationLogger();
        private static readonly IFileSystem FileSystem = new FileSystem();
        private static readonly IXmlTransformService XmlTransformService = new XmlTransformService(FileSystem, XmlTransformationLogger);

        private static int Main(string[] args)
        {
            if (args.Length == 3)
            {
                try
                {
                    XmlTransformService.Transform(args[0], args[1], args[2]);
                    return 0;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    return -1;
                }
            }

            Console.WriteLine("Invalid command line parameters");
            Console.WriteLine("Examples");
            Console.WriteLine("Transformer.exe Source.config Transform.config Destination.config");
            Console.WriteLine("Transformer.exe Source.config Transform.*.config Destination.config");
            return -1;
        }
    }
}
