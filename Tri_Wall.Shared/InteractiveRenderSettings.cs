using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tri_Wall.Shared
{
    public static class InteractiveRenderSettings
    {
        public static IComponentRenderMode? InteractiveServer { get; set; } = RenderMode.InteractiveServer;
        public static IComponentRenderMode? InteractiveAuto { get; set; } = RenderMode.InteractiveAuto;
        public static IComponentRenderMode? InteractiveWebAssembly { get; set; } = RenderMode.InteractiveWebAssembly;

        public static void ConfigureBlazorHybridRenderModes()
        {
            InteractiveServer = null;
            InteractiveAuto = null;
            InteractiveWebAssembly = null;
        }
    }
}
