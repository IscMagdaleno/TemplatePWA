using EngramaCoreStandar.Dapper.Interfaces;

namespace TemplatePWA.API.EngramaLevels.Infrastructure.Entity
{
	/// <summary>
	/// Entity for dbo.spGetTestTableDataType — SP that receives a Table-Valued Parameter (DTParameterType).
	/// Repository must call GetWithListAsync / GetAllWithListAsync (not GetAsync / GetAllAsync).
	/// Property <see cref="Request.Parameters"/> name must match the SQL parameter @Parameters.
	/// </summary>
	public class spGetTestTableDataType
	{
		public class Request : SpRequest
		{
			public string StoredProcedure { get => "spGetTestTableDataType"; }
			public int iIdTest_Table { get; set; }
			public string vchName { get; set; }
			public string vchEmail { get; set; }
			public DateTime? dtRegistered { get; set; }

			/// <summary>
			/// Maps to @Parameters DTParameterType READONLY. EngramaCore sends this as a TVP.
			/// </summary>
			public IEnumerable<DTParameterType> Parameters { get; set; }
		}

		public class Result : DbResult
		{
			public bool bResult { get; set; }
			public string vchMessage { get; set; }
			public int iIdTest_Table { get; set; }
			public string vchName { get; set; }
			public string vchEmail { get; set; }
			public DateTime? dtRegistered { get; set; }
		}
	}
}
