using System;
using Microsoft.Web.XmlTransform;

namespace Svenkle.XMLTransformer.Services
{
    public class XmlTransformationLogger : IXmlTransformationLogger
    {
        public void LogMessage(string message, params object[] messageArgs)
        {
            Console.WriteLine(message, messageArgs);
        }

        public void LogMessage(MessageType type, string message, params object[] messageArgs)
        {
            Console.WriteLine(message, messageArgs);
        }

        public void LogWarning(string message, params object[] messageArgs)
        {
            Console.WriteLine("WARN " + message, messageArgs);
        }

        public void LogWarning(string file, string message, params object[] messageArgs)
        {
            Console.WriteLine("WARN File {0}: ", file);
            Console.WriteLine("WARN " + message, messageArgs);
        }

        public void LogWarning(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            Console.WriteLine("WARN File {0}, line {1}, position {2}: ", file, lineNumber, linePosition);
            Console.WriteLine("WARN " + message, messageArgs);
        }

        public void LogError(string message, params object[] messageArgs)
        {
            Console.WriteLine("ERROR" + message, messageArgs);
        }

        public void LogError(string file, string message, params object[] messageArgs)
        {
            Console.WriteLine("ERROR File {0}: ", file);
            Console.WriteLine("ERROR " + message, messageArgs);
        }

        public void LogError(string file, int lineNumber, int linePosition, string message, params object[] messageArgs)
        {
            Console.WriteLine("ERROR File {0}, line {1}, position {2}: ", file, lineNumber, linePosition);
            Console.WriteLine("ERROR " + message, messageArgs);
        }

        public void LogErrorFromException(Exception ex)
        {
            Console.WriteLine("ERROR " + ex);
        }

        public void LogErrorFromException(Exception ex, string file)
        {
            Console.WriteLine("ERROR File {0}: ", file);
            Console.WriteLine("ERROR " + ex);
        }

        public void LogErrorFromException(Exception ex, string file, int lineNumber, int linePosition)
        {
            Console.WriteLine("ERROR File {0}, line {1}, position {2}: ", file, lineNumber, linePosition);
            Console.WriteLine("ERROR " + ex);
        }

        public void StartSection(string message, params object[] messageArgs)
        {
            Console.WriteLine(message, messageArgs);
        }

        public void StartSection(MessageType type, string message, params object[] messageArgs)
        {
            Console.WriteLine(message, messageArgs);
        }

        public void EndSection(string message, params object[] messageArgs)
        {
            Console.WriteLine(message, messageArgs);
        }

        public void EndSection(MessageType type, string message, params object[] messageArgs)
        {
            Console.WriteLine(message, messageArgs);
        }
    }

}
