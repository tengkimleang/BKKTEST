using Tri_Wall.Shared.Interfaces;

namespace Tri_Wall.WebApp.Client.Services
{
    public class FormFactor : IFormFactor
    {
        public string GetFormFactor()
        {
            return "WebAssembly";
        }
        public string GetPlatform()
        {
            return Environment.OSVersion.ToString();
        }
    }
}
