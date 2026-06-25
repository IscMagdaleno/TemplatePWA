using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace TemplatePWA.PWA.Shared.Common
{
    public class MenuItemModel
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public Color Color { get; set; }
        public EventCallback Action { get; set; }
    }
}
