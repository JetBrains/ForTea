using JetBrains.Build;
using JetBrains.ForTea.RiderPlugin.BuildScript;
using JetBrains.Rider.Backend.Install;

namespace JetBrains.ForTea.RiderPlugin.Install
{
  public static class AdvertiseRiderBundledPlugin
  {
    [BuildStep]
    public static RiderBundledProductArtifact ShipForTeaWithRider()
    {
      return new RiderBundledProductArtifact(
        ForTeaInRiderProduct.ProductTechnicalName,
        ForTeaInRiderProduct.ThisSubplatformName,
        ForTeaInRiderProduct.DotFilesFolder,
        allowCommonPluginFiles: false);
    }
  }
}