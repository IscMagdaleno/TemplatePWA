using EngramaCoreStandar.Dapper.Results;
using EngramaCoreStandar.Mapper;
using EngramaCoreStandar.Results;
using EngramaCoreStandar.Servicios;

using TemplatePWA.Share.Objetos;
using TemplatePWA.Share.PostClass;

namespace TemplatePWA.PWA.Areas.TestModul.Utiles
{
	public class DataTest
	{

		private string url = @"api/Test";

		private readonly IHttpService _httpService;
		private readonly MapperHelper mapperHelper;
		private readonly IValidaServicioService validaServicioService;

		public IList<TestTable> LstTestTable { get; set; }

		public DataTest(IHttpService httpService, MapperHelper mapperHelper, IValidaServicioService validaServicioService)
		{
			_httpService = httpService;
			this.mapperHelper = mapperHelper;
			this.validaServicioService = validaServicioService;
			LstTestTable = new List<TestTable>();
		}

		public async Task<SeverityMessage> PostTestTable()
		{
			var APIUrl = url + "/PostTestTable";

			var model = new PostTestTable();

			var response = await _httpService.Post<PostTestTable, Response<List<TestTable>>>(APIUrl, model);
			var validacion = validaServicioService.ValidadionServicio(response,
				onSuccess: data => LstTestTable = data.ToList());


			return validacion;
		}


	}
}
