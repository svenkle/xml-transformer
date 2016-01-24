using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
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

        public void Transform(string source, string transform, string destination)
        {
            if (transform.Contains("*"))
            {
                var directory = _fileSystem.Path.GetDirectoryName(transform);
                var files = _fileSystem.Directory
                    .GetFiles(string.IsNullOrEmpty(directory) ? "." : directory, transform)
                    .Where(x => _fileSystem.Path.GetFileName(x) != source);

                Transform(source, files, destination);
            }
            else
            {
                TransformXmlFile(source, transform, destination);
            }
        }
        
        private void Transform(string source, IEnumerable<string> transforms, string destination)
        {
            var sourceXml = _fileSystem.File.ReadAllText(source);

            foreach (var transform in transforms)
            {
                var tranformXml = _fileSystem.File.ReadAllText(transform);
                sourceXml = TranformXml(sourceXml, tranformXml);
            }

            _fileSystem.File.WriteAllText(destination, sourceXml);
        }

        private string TranformXml(string sourceXml, string transformXml)
        {
            var transformXmlBytes = Encoding.UTF8.GetBytes(transformXml);
            using (var stream = new MemoryStream(transformXmlBytes))
            {
                var transformation = new XmlTransformation(stream, _xmlTransformationLogger);
                var sourceDocument = new XmlTransformableDocument
                {
                    PreserveWhitespace = true
                };

                sourceDocument.LoadXml(sourceXml);
                transformation.Apply(sourceDocument);
                return sourceDocument.OuterXml;
            }
        }

        private void TransformXmlFile(string sourceFile, string transformFile, string destinationFile)
        {
            var transformedXml = TranformXml(_fileSystem.File.ReadAllText(sourceFile),
                _fileSystem.File.ReadAllText(transformFile));

            _fileSystem.File.WriteAllText(destinationFile, transformedXml);
        }
    }
}
