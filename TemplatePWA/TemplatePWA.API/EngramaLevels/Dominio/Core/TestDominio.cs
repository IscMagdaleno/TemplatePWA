// Ignore Spelling: Dominio

using EngramaCoreStandar.Mapper;
using EngramaCoreStandar.Results;

using TemplatePWA.API.EngramaLevels.Dominio.Interfaces;

using TemplatePWA.API.EngramaLevels.Infrastructure.Interfaces;
using TemplatePWA.Share.Entity;
using TemplatePWA.Share.Objetos;
using TemplatePWA.Share.PostClass;

namespace TemplatePWA.API.EngramaLevels.Dominio.Core
{

	/// <summary>
	/// Class where save all the core code and the logic business,
	/// and convert the data base response to object to get back the user
	/// </summary>
	public class TestDominio : ITestDominio
	{
		private readonly ITestRepository testRepository;
		private readonly MapperHelper mapperHelper;
		private readonly IResponseHelper responseHelper;

		/// <summary>
		/// Initialize the fields receiving the interfaces on the builder
		/// </summary>
		public TestDominio(ITestRepository testRepository,
			MapperHelper mapperHelper,
			IResponseHelper responseHelper)
		{
			this.testRepository = testRepository;
			this.mapperHelper = mapperHelper;
			this.responseHelper = responseHelper;
		}



		public async Task<Response<IEnumerable<TestTable>>> TestTable(PostTestTable DAOmodel)
		{
			try
			{
				var model = mapperHelper.Get<PostTestTable, spGetTestTable.Request>(DAOmodel);

				var result = await testRepository.spGetTestTable(model);
				return responseHelper.Validacion<spGetTestTable.Result, TestTable>(result);
			}
			catch (Exception ex)
			{
				return Response<IEnumerable<TestTable>>.BadResult(ex.Message, new List<TestTable>());
			}
		}
	}
}
