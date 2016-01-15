﻿using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Svenkle.XMLTransformer.Services;
using Svenkle.XMLTransformer.Services.Interfaces;
using Xunit;

namespace Svenkle.XMLTransformer.Tests
{
    public class XmlTransformServiceFacts
    {
        private readonly IFileSystem _fileSystem;
        private readonly IXmlTransformService _xmlTransformService;
        private const string Configuration = "<?xml version=\"1.0\" encoding=\"utf-8\"?><configuration><appSettings><add key=\"Sample\" value=\"Default\"/></appSettings></configuration>";
        private const string Transform = "<?xml version=\"1.0\" encoding=\"utf-8\"?><configuration xmlns:xdt=\"http://schemas.microsoft.com/XML-Document-Transform\"><appSettings><add key=\"Sample\" value=\"Debug\" xdt:Transform=\"Replace\"/></appSettings></configuration>";
        private const string MalformedConfiguration = "<<?xml version=\"1.0\" encoding=\"utf-8\"?><configuration><appSettings><add key=\"Sample\" value=\"Default\"/></appSettings></configuration>";
        private const string MalformedTransform = "<<?xml version=\"1.0\" encoding=\"utf-8\"?><configuration xmlns:xdt=\"http://schemas.microsoft.com/XML-Document-Transform\"><appSettings><add key=\"Sample\" value=\"Debug\" xdt:Transform=\"Replace\"/></appSettings></configuration>";
        private const string IncompleteTransform = "<?xml version=\"1.0\" encoding=\"utf-8\"?><configuration><appSettings><add key=\"Sample\" value=\"Debug\" xdt:Transform=\"Replace\"/></appSettings></configuration>";

        public XmlTransformServiceFacts()
        {
            _fileSystem = new MockFileSystem();
            _xmlTransformService = new XmlTransformService(_fileSystem, null);
        }

        public class TheTransformMethod : XmlTransformServiceFacts
        {
            [Fact]
            public void ThrowsAFileNotFoundExceptionIfTheSourceFileDoesntExist()
            {
                // Prepare
                var transformFile = _fileSystem.Path.GetRandomFileName();
                var destinationFile = _fileSystem.Path.GetRandomFileName();
                _fileSystem.File.WriteAllText(transformFile, Configuration);

                // Act & Assert
                Assert.Throws<FileNotFoundException>(() => _xmlTransformService.Transform(string.Empty, transformFile, destinationFile));
            }

            [Fact]
            public void ThrowsAFileNotFoundExceptionIfTheTransformFileDoesntExist()
            {
                // Prepare
                var sourceFile = _fileSystem.Path.GetRandomFileName();
                var destinationFile = _fileSystem.Path.GetRandomFileName();
                _fileSystem.File.WriteAllText(sourceFile, Configuration);

                // Act & Assert
                Assert.Throws<FileNotFoundException>(() => _xmlTransformService.Transform(sourceFile, string.Empty, destinationFile));
            }

            [Fact]
            public void ThrowsAnArgumentExceptionIfTheDestinationFileIsNullOrEmpty()
            {
                // Prepare
                var sourceFile = _fileSystem.Path.GetRandomFileName();
                var transformFile = _fileSystem.Path.GetRandomFileName();
                _fileSystem.File.WriteAllText(sourceFile, Configuration);
                _fileSystem.File.WriteAllText(transformFile, Transform);

                // Act & Assert
                Assert.Throws<ArgumentException>(() => _xmlTransformService.Transform(sourceFile, transformFile, string.Empty));
            }

            [Fact]
            public void PerformsValidTransformsSuccessfully()
            {
                // Prepare
                var sourceFile = _fileSystem.Path.GetRandomFileName();
                var transformFile = _fileSystem.Path.GetRandomFileName();
                var destinationFile = _fileSystem.Path.GetRandomFileName();
                _fileSystem.File.WriteAllText(sourceFile, Configuration);
                _fileSystem.File.WriteAllText(transformFile, Transform);

                // Act
                var result = _xmlTransformService.Transform(sourceFile, transformFile, destinationFile);
                var document = XDocument.Parse(_fileSystem.File.ReadAllText(destinationFile));

                // Assert
                Assert.True(result);
                Assert.Equal(document.Descendants("appSettings").Descendants("add").First().Attribute("value").Value, "Debug");
            }

            [Fact]
            public void ThrowsXmlExceptionWhenSourceFileIsMalformed()
            {
                // Prepare
                var sourceFile = _fileSystem.Path.GetRandomFileName();
                var transformFile = _fileSystem.Path.GetRandomFileName();
                var destinationFile = _fileSystem.Path.GetRandomFileName();
                _fileSystem.File.WriteAllText(sourceFile, MalformedConfiguration);
                _fileSystem.File.WriteAllText(transformFile, Transform);

                // Act & Assert
                Assert.Throws<XmlException>(() => _xmlTransformService.Transform(sourceFile, transformFile, destinationFile));
            }

            [Fact]
            public void ThrowsXmlExceptionWhenTransformFileIsMalformed()
            {
                // Prepare
                var sourceFile = _fileSystem.Path.GetRandomFileName();
                var transformFile = _fileSystem.Path.GetRandomFileName();
                var destinationFile = _fileSystem.Path.GetRandomFileName();
                _fileSystem.File.WriteAllText(sourceFile, Configuration);
                _fileSystem.File.WriteAllText(transformFile, MalformedTransform);

                // Act & Assert
                Assert.Throws<XmlException>(() => _xmlTransformService.Transform(sourceFile, transformFile, destinationFile));
            }

            [Fact]
            public void ThrowsXmlExceptionWhenTransformFileIsMissingXdtDeclaration()
            {
                // Prepare
                var sourceFile = _fileSystem.Path.GetRandomFileName();
                var transformFile = _fileSystem.Path.GetRandomFileName();
                var destinationFile = _fileSystem.Path.GetRandomFileName();
                _fileSystem.File.WriteAllText(sourceFile, Configuration);
                _fileSystem.File.WriteAllText(transformFile, IncompleteTransform);

                // Act & Assert
                Assert.Throws<XmlException>(() => _xmlTransformService.Transform(sourceFile, transformFile, destinationFile));
            }
        }
    }
}
