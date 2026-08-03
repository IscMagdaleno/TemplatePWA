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
                    RegistroSeleccionado = data;
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
                        else
                        {
                            LstTestTable.Add(data);
                        }
                    }
                });

            return validacion;
        }

        /// <summary>
        /// Calls PostGetTestTableDataType — sends a list of IDs as TVP (DTParameterType).
        /// Pass selectedIds to filter; empty list returns all rows (SP demo behavior).
        /// </summary>
        public async Task<SeverityMessage> PostGetByDataType(IEnumerable<int> selectedIds)
        {
            var APIUrl = url + "/PostGetTestTableDataType";

            var model = new DtoGetTestTableDataType
            {
                // SQL @dtRegistered is DATETIME — never send DateTime default (0001-01-01)
                dtRegistered = DateTime.Parse("1900-01-01"),
                Parameters = (selectedIds ?? Enumerable.Empty<int>())
                    .Where(id => id > 0)
                    .Select(id => new DTParameterType
                    {
                        iParameter = id,
                        vchParameter = string.Empty,
                        dtParameter = DateTime.Parse("1900-01-01")
                    })
                    .ToList()
            };

            var response = await _httpService.Post<DtoGetTestTableDataType, Response<List<Test_Table>>>(APIUrl, model);
            var validacion = validaServicioService.ValidadionServicio(response,
                onSuccess: data => LstTestTable = data.ToList());

            return validacion;
        }
    }
}
