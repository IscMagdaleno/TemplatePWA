namespace TemplatePWA.Share.DtoModels.TestModule
{
	public class DtoGetTestTableDataType
	{
		public string vchEmail { get; set; }
		public DateTime dtRegistered { get; set; }
		public IEnumerable<DTParameterType> Parameters { get; set; }
	}

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
