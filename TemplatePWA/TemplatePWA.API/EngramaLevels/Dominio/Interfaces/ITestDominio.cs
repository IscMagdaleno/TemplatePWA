
using EngramaCoreStandar.Results;

using TemplatePWA.Share.Objetos;
using TemplatePWA.Share.PostClass;

namespace TemplatePWA.API.EngramaLevels.Dominio.Interfaces
{
	public interface ITestDominio
	{
		Task<Response<Test_Table>> GetTestTableDataType(PostGetTestTableDataType PostModel);
		Task<Response<Test_Table>> SaveTest_Table(PostSaveTest_Table PostModel);
		Task<Response<IEnumerable<Test_Table>>> TestTable(PostTestTable DAOmodel);
	}
}
