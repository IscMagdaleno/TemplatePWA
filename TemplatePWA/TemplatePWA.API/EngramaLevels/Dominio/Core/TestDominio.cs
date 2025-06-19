// Ignore Spelling: Dominio

using EngramaCoreStandar;
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



		public async Task<Response<IEnumerable<Test_Table>>> TestTable(PostTestTable DAOmodel)
		{
			try
			{
				var model = mapperHelper.Get<PostTestTable, spGetTestTable.Request>(DAOmodel);

				var result = await testRepository.spGetTestTable(model);
				return responseHelper.Validacion<spGetTestTable.Result, Test_Table>(result);
			}
			catch (Exception ex)
			{
				return Response<IEnumerable<Test_Table>>.BadResult(ex.Message, new List<Test_Table>());
			}
		}

		public async Task<Response<Test_Table>> SaveTest_Table(PostSaveTest_Table PostModel)
		{
			try
			{
				var model = mapperHelper.Get<PostSaveTest_Table, spSaveTest_Table.Request>(PostModel);
				var result = await testRepository.spSaveTest_Table(model);
				var validation = responseHelper.Validacion<spSaveTest_Table.Result, Test_Table>(result);
				if (validation.IsSuccess)
				{
					PostModel.iIdTest_Table = validation.Data.iIdTest_Table;
					validation.Data = mapperHelper.Get<PostSaveTest_Table, Test_Table>(PostModel);
				}
				return validation;
			}
			catch (Exception ex)
			{
				return Response<Test_Table>.BadResult(ex.Message, new());
			}
		}


		public async Task<Response<Test_Table>> GetTestTableDataType(PostGetTestTableDataType PostModel)
		{
			try
			{
				var model = mapperHelper.Get<PostGetTestTableDataType, spGetTestTableDataType.Request>(PostModel);

				//business logic
				var lista = new List<DTParameterType>();

				lista.Add(new DTParameterType
				{
					vchParameter = "valor",
					dtParameter = Defaults.SqlMinDate()
				});
				model.Parameters = lista;


				var result = await testRepository.spGetTestTableDataType(model);
				var validation = responseHelper.Validacion<spGetTestTableDataType.Result, Test_Table>(result);
				if (validation.IsSuccess)
				{
					PostModel.vchEmail = validation.Data.vchEmail;
					validation.Data = mapperHelper.Get<PostGetTestTableDataType, Test_Table>(PostModel);
				}
				return validation;
			}
			catch (Exception ex)
			{
				return Response<Test_Table>.BadResult(ex.Message, new());
			}
		}


	}
}
