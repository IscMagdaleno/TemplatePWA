using TemplatePWA.Share.Entity;

namespace TemplatePWA.API.EngramaLevels.Infrastructure.Interfaces
{
	public interface ITestRepository
	{
		Task<IEnumerable<spGetTestTable.Result>> spGetTestTable(spGetTestTable.Request DAOmodel);
		Task<spGetTestTableDataType.Result> spGetTestTableDataType(spGetTestTableDataType.Request PostModel);
		Task<spSaveTest_Table.Result> spSaveTest_Table(spSaveTest_Table.Request PostModel);
	}
}
