﻿namespace Svenkle.XMLTransformer.Services.Interfaces
{
    public interface IXmlTransformService
    {
        void Transform(string source, string transform, string destination);
    }
}
