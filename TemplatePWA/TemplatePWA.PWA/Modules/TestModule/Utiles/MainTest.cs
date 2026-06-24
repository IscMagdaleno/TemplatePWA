using EngramaCoreStandar.Dapper.Results;
using EngramaCoreStandar.Mapper;
using EngramaCoreStandar.Results;
using EngramaCoreStandar.Servicios;

using TemplatePWA.Share.DtoModels.TestModule;
using TemplatePWA.Share.Objects.TestModule;

namespace TemplatePWA.PWA.Modules.TestModule.Utiles
{
	public class MainTest
	{

		private string url = @"api/Test";

		private readonly IHttpService _httpService;
		private readonly MapperHelper mapperHelper;
		private readonly IValidaServicioService validaServicioService;

		public IList<Test_Table> LstTestTable { get; set; }

		public MainTest(IHttpService httpService, MapperHelper mapperHelper, IValidaServicioService validaServicioService)
		{
			_httpService = httpService;
			this.mapperHelper = mapperHelper;
			this.validaServicioService = validaServicioService;
			LstTestTable = new List<Test_Table>();
		}

		public async Task<SeverityMessage> PostTestTable()
		{
			var APIUrl = url + "/PostTestTable";

			var model = new DtoTestTable();

			var response = await _httpService.Post<DtoTestTable, Response<List<Test_Table>>>(APIUrl, model);
			var validacion = validaServicioService.ValidadionServicio(response,
				onSuccess: data => LstTestTable = data.ToList());


			return validacion;
		}


	}
}
