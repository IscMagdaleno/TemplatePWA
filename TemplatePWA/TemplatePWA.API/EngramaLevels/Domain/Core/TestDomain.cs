using EngramaCoreStandar;
using EngramaCoreStandar.Mapper;
using EngramaCoreStandar.Results;

using TemplatePWA.API.EngramaLevels.Domain.Interfaces;
using TemplatePWA.API.EngramaLevels.Infrastructure.Entity;
using TemplatePWA.API.EngramaLevels.Infrastructure.Interfaces;
using TemplatePWA.Share.DtoModels.TestModule;
using TemplatePWA.Share.Objects.TestModule;

namespace TemplatePWA.API.EngramaLevels.Domain.Core
{

	/// <summary>
	/// Class where save all the core code and the logic business,
	/// and convert the data base response to object to get back the user
	/// </summary>
	public class TestDomain : ITestDomain
	{
		private readonly ITestRepository testRepository;
		private readonly MapperHelper mapperHelper;
		private readonly IResponseHelper responseHelper;

		/// <summary>
		/// Initialize the fields receiving the interfaces on the builder
		/// </summary>
		public TestDomain(ITestRepository testRepository,
			MapperHelper mapperHelper,
			IResponseHelper responseHelper)
		{
			this.testRepository = testRepository;
			this.mapperHelper = mapperHelper;
			this.responseHelper = responseHelper;
		}



		public async Task<Response<IEnumerable<Test_Table>>> TestTable(DtoTestTable DAOmodel)
		{
			try
			{
				var model = mapperHelper.Get<DtoTestTable, spGetTestTable.Request>(DAOmodel);

				var result = await testRepository.spGetTestTable(model);
				return responseHelper.Validacion<spGetTestTable.Result, Test_Table>(result);
			}
			catch (Exception ex)
			{
				return Response<IEnumerable<Test_Table>>.BadResult(ex.Message, new List<Test_Table>());
			}
		}

		public async Task<Response<Test_Table>> SaveTest_Table(DtoaveTest_Table PostModel)
		{
			try
			{
				var model = mapperHelper.Get<DtoaveTest_Table, spSaveTest_Table.Request>(PostModel);
				var result = await testRepository.spSaveTest_Table(model);
				var validation = responseHelper.Validacion<spSaveTest_Table.Result, Test_Table>(result);
				if (validation.IsSuccess)
				{
					PostModel.iIdTest_Table = validation.Data.iIdTest_Table;
					validation.Data = mapperHelper.Get<DtoaveTest_Table, Test_Table>(PostModel);
				}
				return validation;
			}
			catch (Exception ex)
			{
				return Response<Test_Table>.BadResult(ex.Message, new());
			}
		}


		/// <summary>
		/// Demo: send a list of DTParameterType rows to SQL as TVP and filter Test_Table by iParameter (IDs).
		/// Do not use MapperHelper on the full DTO: Share.DTParameterType ≠ Infrastructure.DTParameterType,
		/// and AutoMapper fails on Parameters. Map scalars + TVP rows explicitly.
		/// </summary>
		public async Task<Response<IEnumerable<Test_Table>>> GetTestTableDataType(DtoGetTestTableDataType PostModel)
		{
			try
			{
				// Build Request by hand — avoids AutoMapper mapping Share.DT* → Infrastructure.DT*
				// dtRegistered must be a valid SqlDateTime (≥ 1753); default DateTime is 0001-01-01 and overflows.
				var model = new spGetTestTableDataType.Request
				{
					iIdTest_Table = PostModel.iIdTest_Table,
					vchName = PostModel.vchName,
					vchEmail = PostModel.vchEmail,
					dtRegistered = ToSqlDate(PostModel.dtRegistered),
					Parameters = (PostModel.Parameters ?? Enumerable.Empty<Share.DtoModels.TestModule.DTParameterType>())
						.Select(p => new Infrastructure.Entity.DTParameterType
						{
							iParameter = p.iParameter,
							vchParameter = p.vchParameter ?? string.Empty,
							dtParameter = ToSqlDate(p.dtParameter),
							mParametr = p.mParametr,
							smParameter = p.smParameter,
							bParameter = p.bParameter
						})
						.ToList()
				};

				var result = await testRepository.spGetTestTableDataType(model);
				return responseHelper.Validacion<spGetTestTableDataType.Result, Test_Table>(result);
			}
			catch (Exception ex)
			{
				return Response<IEnumerable<Test_Table>>.BadResult(ex.Message, new List<Test_Table>());
			}
		}

		/// <summary>
		/// Coerces CLR default / out-of-range dates to Engrama SQL-safe minimum (1900-01-01).
		/// </summary>
		private static DateTime ToSqlDate(DateTime value)
		{
			return value < Defaults.SqlMinDate() ? Defaults.SqlMinDate() : value;
		}


	}
}
