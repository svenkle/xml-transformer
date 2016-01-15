namespace Svenkle.XMLTransformer.Services.Interfaces
{
    public interface IXmlTransformService
    {
        bool Transform(string source, string transform, string destination);
        bool Transform(string source, string transform);
    }
}
