using System.IO.Abstractions;
using Microsoft.Web.XmlTransform;
using Svenkle.XMLTransformer.Services.Interfaces;

namespace Svenkle.XMLTransformer.Services
{
    public class XmlTransformService : IXmlTransformService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IXmlTransformationLogger _xmlTransformationLogger;

        public XmlTransformService(IFileSystem fileSystem, IXmlTransformationLogger xmlTransformationLogger)
        {
            _fileSystem = fileSystem;
            _xmlTransformationLogger = xmlTransformationLogger;
        }

        public bool Transform(string source, string transform, string destination)
        {
            string outputData;

            using (var stream = _fileSystem.File.OpenRead(transform))
            {
                var sourceContent = _fileSystem.File.ReadAllText(source);
                var xmlTransformation = new XmlTransformation(stream, _xmlTransformationLogger);

                var xmlTransformableDocument = new XmlTransformableDocument
                {
                    PreserveWhitespace = true
                };

                xmlTransformableDocument.LoadXml(sourceContent);

                xmlTransformation.Apply(xmlTransformableDocument);

                outputData = xmlTransformableDocument.OuterXml;
            }
            
            _fileSystem.File.WriteAllText(destination, outputData);

            return true;
        }

        public bool Transform(string source, string transform)
        {
            return Transform(source, transform, source);
        }
    }
}
