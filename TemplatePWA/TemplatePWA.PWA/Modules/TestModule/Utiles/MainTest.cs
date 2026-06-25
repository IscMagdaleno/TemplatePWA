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
        public Test_Table RegistroSeleccionado { get; set; }

        public MainTest(IHttpService httpService, MapperHelper mapperHelper, IValidaServicioService validaServicioService)
        {
            _httpService = httpService;
            this.mapperHelper = mapperHelper;
            this.validaServicioService = validaServicioService;
            LstTestTable = new List<Test_Table>();
        }

        public async Task<SeverityMessage> PostGetRegistros()
        {
            var APIUrl = url + "/PostTestTable";

            var model = new DtoTestTable();

            var response = await _httpService.Post<DtoTestTable, Response<List<Test_Table>>>(APIUrl, model);
            var validacion = validaServicioService.ValidadionServicio(response,
                onSuccess: data => LstTestTable = data.ToList());

            return validacion;
        }

        public async Task<SeverityMessage> PostSaveRegistro(DtoaveTest_Table model)
        {
            var APIUrl = url + "/PostSaveTest_Table";

            var response = await _httpService.Post<DtoaveTest_Table, Response<Test_Table>>(APIUrl, model);
            var validacion = validaServicioService.ValidadionServicio(response,
                onSuccess: data => 
                {
                    if (model.iIdTest_Table == 0)
                    {
                        LstTestTable.Add(data);
                    }
                    else
                    {
                        var existing = LstTestTable.FirstOrDefault(x => x.iIdTest_Table == data.iIdTest_Table);
                        if (existing != null)
                        {
                            existing.vchName = data.vchName;
                            existing.vchEmail = data.vchEmail;
                            existing.dtRegistered = data.dtRegistered;
                        }
                    }
                });

            return validacion;
        }
    }
}
