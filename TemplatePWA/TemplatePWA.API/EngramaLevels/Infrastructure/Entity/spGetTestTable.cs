
using EngramaCoreStandar.Dapper.Interfaces;

namespace TemplatePWA.API.EngramaLevels.Infrastructure.Entity
{
/// <summary>
/// Entity for dbo.spGetTestTable — list all rows. Use GetAllAsync (no TVP).
/// </summary>
public class spGetTestTable
{
	public class Request : SpRequest
	{
		public string StoredProcedure { get => "spGetTestTable"; }
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
