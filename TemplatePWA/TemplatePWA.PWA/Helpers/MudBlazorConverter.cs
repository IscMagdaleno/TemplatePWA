using EngramaCoreStandar.Dapper.Results;

using MudBlazor;

namespace TemplatePWA.PWA.Helpers
{
	public static class MudBlazorConverter
	{

		public static Severity ConvertSeverity(SeverityTag severityTag)
		{
			return severityTag switch
			{
				SeverityTag.Success => Severity.Success,
				SeverityTag.Warning => Severity.Warning,
				SeverityTag.Error => Severity.Error,
				SeverityTag.Info => Severity.Info,
				_ => Severity.Normal,
			};
		}
	}
}
