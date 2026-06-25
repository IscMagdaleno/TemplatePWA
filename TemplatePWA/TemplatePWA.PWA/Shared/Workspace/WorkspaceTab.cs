using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace TemplatePWA.PWA.Shared.Workspace
{
    public class WorkspaceTab
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Icono { get; set; }
        public string Text { get; set; }
        public Type TipoControl { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public TipoEstadoControl EstadoControl { get; set; }
    }
}
