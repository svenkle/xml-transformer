namespace Svenkle.XMLTransformer.Services.Interfaces
{
    public interface IXmlTransformService
    {
        void Transform(string sourceFile, string tranformFile, string destinationFile);
    }
}
