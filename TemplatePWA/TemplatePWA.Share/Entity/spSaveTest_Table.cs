using EngramaCoreStandar.Dapper.Interfaces;

namespace TemplatePWA.Share.Entity
{
	public class spSaveTest_Table
	{
		public class Request : SpRequest
		{
			public string StoredProcedure { get => "spSaveTest_Table"; }
			public int iIdTest_Table { get; set; }
			public string vchName { get; set; }
			public string vchEmail { get; set; }
			public DateTime? dtRegistered { get; set; }
		}
		public class Result : DbResult
		{
			public bool bResult { get; set; }
			public string vchMessage { get; set; }
			public int iIdTest_Table { get; set; }
		}
	}

}
