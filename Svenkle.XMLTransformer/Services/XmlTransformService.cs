using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
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

        public void Transform(string sourceFile, string tranformFile, string destinationFile)
        {
            if (tranformFile.Contains("*"))
            {
                var directory = _fileSystem.Path.GetDirectoryName(tranformFile);
                var files = _fileSystem.Directory
                    .GetFiles(string.IsNullOrEmpty(directory) ? "." : directory, tranformFile)
                    .Where(x => _fileSystem.Path.GetFileName(x) != sourceFile);

                Transform(sourceFile, files, destinationFile);
            }
            else
            {
                Transform(sourceFile, new[] { tranformFile }, destinationFile);
            }
        }

        private void Transform(string sourceFile, IEnumerable<string> tranformFiles, string destinationFile)
        {
            var sourceXmlString = _fileSystem.File.ReadAllText(sourceFile);
            var sourceFileEncoding = GetEncoding(sourceXmlString);
            var intermediateXmlString = sourceXmlString;

            foreach (var transformFile in tranformFiles)
            {
                var sourceDocument = new XmlTransformableDocument();
                var transformXmlString = _fileSystem.File.ReadAllText(transformFile);
                var transformFileEncoding = GetEncoding(transformXmlString);
                var transformXmlBytes = transformFileEncoding.GetBytes(transformXmlString);

                using (var memoryStream = new MemoryStream(transformXmlBytes))
                {
                    var transformation = new XmlTransformation(memoryStream, _xmlTransformationLogger);
                    sourceDocument.LoadXml(intermediateXmlString);
                    transformation.Apply(sourceDocument);
                    intermediateXmlString = sourceDocument.OuterXml;
                }
            }

            var xDocument = XDocument.Parse(intermediateXmlString, LoadOptions.PreserveWhitespace);
            intermediateXmlString = $"{xDocument.Declaration}{Environment.NewLine}{xDocument.Document}";

            _fileSystem.File.WriteAllText(destinationFile, intermediateXmlString, sourceFileEncoding);
        }

        private static Encoding GetEncoding(string xmlString)
        {
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlString);
                return xmlDocument.FirstChild.NodeType == XmlNodeType.XmlDeclaration ? 
                    Encoding.GetEncoding(((XmlDeclaration)xmlDocument.FirstChild).Encoding) : 
                    Encoding.UTF8;
            }
            catch (XmlException)
            {
                return Encoding.UTF8;
            }
        }
    }
}
