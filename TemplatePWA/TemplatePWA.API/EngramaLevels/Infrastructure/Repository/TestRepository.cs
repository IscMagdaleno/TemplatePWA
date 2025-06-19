using EngramaCoreStandar.Dapper;
using EngramaCoreStandar.Extensions;

using TemplatePWA.API.EngramaLevels.Infrastructure.Interfaces;
using TemplatePWA.Share.Entity;

namespace TemplatePWA.API.EngramaLevels.Infrastructure.Repository
{
	/// <summary>
	/// Class to keep all the methods that will consult the database 
	/// </summary>
	public class TestRepository : ITestRepository
	{

		private readonly IDapperManagerHelper _managerHelper;

		/// <summary>
		/// constructor to initialize all the class and receive the other interfaces how we will to work
		/// </summary>
		/// <param name="managerHelper"></param>
		public TestRepository(IDapperManagerHelper managerHelper)
		{
			_managerHelper = managerHelper;
		}


		/// <summary>
		/// Method to consult the database using the engrama tools (Dapper and other class that we build)
		/// </summary>
		/// <param name="DAOmodel"></param>
		/// <returns></returns>
		public async Task<IEnumerable<spGetTestTable.Result>> spGetTestTable(spGetTestTable.Request DAOmodel)
		{
			var respuesta = await _managerHelper.GetAllAsync<spGetTestTable.Result, spGetTestTable.Request>(DAOmodel, "");
			if (respuesta.Ok)
			{
				return respuesta.Data;

			}
			return new List<spGetTestTable.Result>() { new() { bResult = false, vchMessage = respuesta.Msg } };

		}


		public async Task<spSaveTest_Table.Result> spSaveTest_Table(spSaveTest_Table.Request PostModel)
		{
			var result = await _managerHelper.GetAsync<spSaveTest_Table.Result, spSaveTest_Table.Request>(PostModel);
			if (result.Ok)
			{
				return result.Data;
			}
			return new() { bResult = false, vchMessage = $"[{(result.Ex.NotNull() ? result.Ex.Message : "")}] - [{result.Msg}]" };
		}

		public async Task<spGetTestTableDataType.Result> spGetTestTableDataType(spGetTestTableDataType.Request PostModel)
		{
			var result = await _managerHelper.GetWithListAsync<spGetTestTableDataType.Result, spGetTestTableDataType.Request>(PostModel);
			if (result.Ok)
			{
				return result.Data;
			}
			return new() { bResult = false, vchMessage = $"[{(result.Ex.NotNull() ? result.Ex.Message : "")}] - [{result.Msg}]" };
		}

	}
}
