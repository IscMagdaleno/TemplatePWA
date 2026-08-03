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
		/// <summary>
		/// Only keys that match [Parameter] properties on TipoControl. Do not put tab-identity flags here.
		/// </summary>
		public Dictionary<string, object> Parameters { get; set; } = new();
		public TipoEstadoControl EstadoControl { get; set; }

		/// <summary>Used for tab dedupe (not passed to DynamicComponent).</summary>
		public object EntityId { get; set; }

		/// <summary>When true, always open a new tab instance.</summary>
		public bool ForceNewTab { get; set; }

		/// <summary>
		/// Identity used to dedupe tabs: TipoControl | EntityId | mode.
		/// </summary>
		public string GetIdentityKey()
		{
			if (ForceNewTab)
			{
				return Id.ToString();
			}

			var typeName = TipoControl?.FullName ?? string.Empty;
			var entityId = EntityId?.ToString() ?? string.Empty;
			var mode = EstadoControl.ToString();

			if (Parameters != null
				&& Parameters.TryGetValue("EstadoControl", out var estadoParam)
				&& estadoParam != null)
			{
				mode = estadoParam.ToString() ?? mode;
			}

			return $"{typeName}|{entityId}|{mode}";
		}
	}
}
