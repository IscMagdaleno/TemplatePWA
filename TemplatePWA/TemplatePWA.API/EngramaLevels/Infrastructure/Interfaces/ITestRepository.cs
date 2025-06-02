using TemplatePWA.Share.Entity;

namespace TemplatePWA.API.EngramaLevels.Infrastructure.Interfaces
{
	public interface ITestRepository
	{
		Task<IEnumerable<spGetTestTable.Result>> spGetTestTable(spGetTestTable.Request DAOmodel);
	}
}
