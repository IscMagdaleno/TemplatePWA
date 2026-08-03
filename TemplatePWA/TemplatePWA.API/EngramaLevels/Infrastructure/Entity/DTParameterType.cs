namespace TemplatePWA.API.EngramaLevels.Infrastructure.Entity
{
	/// <summary>
	/// C# mirror of dbo.DTParameterType (User-Defined Table Type).
	/// Used as IEnumerable&lt;DTParameterType&gt; on SP Request properties that map to @… READONLY TVPs.
	/// Column names must match the SQL type exactly (including typos like mParametr).
	/// </summary>
	public class DTParameterType
	{
		public int iParameter { get; set; }
		public string vchParameter { get; set; }
		public DateTime dtParameter { get; set; }
		public decimal mParametr { get; set; }
		public int smParameter { get; set; }
		public int bParameter { get; set; }
	}
}
