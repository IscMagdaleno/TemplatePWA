using EngramaCoreStandar.Results;

using TemplatePWA.Share.DtoModels.TestModule;
using TemplatePWA.Share.Objects.TestModule;

namespace TemplatePWA.API.EngramaLevels.Domain.Interfaces
{
	public interface ITestDomain
	{
		Task<Response<IEnumerable<Test_Table>>> GetTestTableDataType(DtoGetTestTableDataType PostModel);
		Task<Response<Test_Table>> SaveTest_Table(DtoaveTest_Table PostModel);
		Task<Response<IEnumerable<Test_Table>>> TestTable(DtoTestTable DAOmodel);
	}
}
