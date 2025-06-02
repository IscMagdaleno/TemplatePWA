
using EngramaCoreStandar.Results;

using TemplatePWA.Share.Objetos;
using TemplatePWA.Share.PostClass;

namespace TemplatePWA.API.EngramaLevels.Dominio.Interfaces
{
	public interface ITestDominio
	{
		Task<Response<IEnumerable<TestTable>>> TestTable(PostTestTable DAOmodel);
	}
}
