using System;

namespace InWorldz.Halcyon.OpenSim.ImpExp
{
    /// <summary>
    /// Interface to an object that can resolve assets from 
    /// their GUIDs
    /// </summary>
    public interface IAssetResolver
    {
        byte[] ResolveAsset(Guid assetId);
    }
}