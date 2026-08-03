namespace TemplatePWA.Share.DtoModels.TestModule
{
	/// <summary>
	/// DTO for endpoint PostGetTestTableDataType — body includes a list mapped to SQL TVP DTParameterType.
	/// </summary>
	public class DtoGetTestTableDataType
	{
		public int iIdTest_Table { get; set; }
		public string vchName { get; set; } = string.Empty;
		public string vchEmail { get; set; } = string.Empty;
		public DateTime dtRegistered { get; set; }

		/// <summary>
		/// Rows sent to dbo.DTParameterType. Use iParameter as Test_Table.iIdTest_Table to filter.
		/// </summary>
		public IEnumerable<DTParameterType> Parameters { get; set; } = new List<DTParameterType>();
	}

	/// <summary>
	/// Share mirror of dbo.DTParameterType / Infrastructure.Entity.DTParameterType.
	/// </summary>
	public class DTParameterType
	{
		public int iParameter { get; set; }
		public string vchParameter { get; set; } = string.Empty;
		public DateTime dtParameter { get; set; }
		public decimal mParametr { get; set; }
		public int smParameter { get; set; }
		public int bParameter { get; set; }
	}
}
