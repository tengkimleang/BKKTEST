using Tri_Wall.Shared.Interfaces;

namespace Tri_Wall.WebApp.Server.Services
{
    public class FormFactor : IFormFactor
    {
        public string GetFormFactor()
        {
            return "Web";
        }
        public string GetPlatform()
        {
            return Environment.OSVersion.ToString();
        }
    }
}
