using Microsoft.AspNetCore.Components;
using System;

namespace TemplatePWA.PWA.Shared.Workspace
{
    public class WorkspaceTab
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Icono { get; set; }
        public string Text { get; set; }
        public Type TipoControl { get; set; }
        public bool Repetir { get; set; }
        public TipoEstadoControl EstadoControl { get; set; }
        public RenderFragment Componente { get; set; }
        public EngramaWorkspaceComponent InstanciaComponente { get; set; }
    }
}
