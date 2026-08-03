using Microsoft.AspNetCore.Mvc;

using TemplatePWA.API.EngramaLevels.Domain.Interfaces;
using TemplatePWA.Share.DtoModels.TestModule;

namespace TemplatePWA.API.Controllers
{
	/// <summary>
	/// Test controller to show how Engrama work
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class TestController : ControllerBase
	{
		private readonly ITestDomain testDomain;

		public TestController(ITestDomain testDomain)
		{
			this.testDomain = testDomain;
		}


		/// <summary>
		/// Test method to show you how to connect the API with the database using Engrama.
		/// </summary>
		/// <param name="postModel"></param>
		/// <returns></returns>
		[HttpPost("PostTestTable")]
		public async Task<IActionResult> PostTestTable([FromBody] DtoTestTable postModel)
		{
			var result = await testDomain.TestTable(postModel);
			if (result.IsSuccess)
			{
				return Ok(result);
			}
			return BadRequest(result);
		}


		/// <summary>
		/// Guardar informacion de pruebas
		/// </summary>
		/// <param name="postModel"></param>
		/// <returns></returns>
		[HttpPost("PostSaveTest_Table")]
		public async Task<IActionResult> PostSaveTest_Table([FromBody] DtoaveTest_Table postModel)
		{
			var result = await testDomain.SaveTest_Table(postModel);
			if (result.IsSuccess)
			{
				return Ok(result);
			}
			return BadRequest(result);
		}

		/// <summary>
		/// Demo Engrama: envía una lista (TVP DTParameterType) al SP spGetTestTableDataType.
		/// Body.Parameters[].iParameter = iIdTest_Table a filtrar; lista vacía o sin IDs &gt; 0 = todos.
		/// </summary>
		[HttpPost("PostGetTestTableDataType")]
		public async Task<IActionResult> PostGetTestTableDataType([FromBody] DtoGetTestTableDataType postModel)
		{
			var result = await testDomain.GetTestTableDataType(postModel);
			if (result.IsSuccess)
			{
				return Ok(result);
			}
			return BadRequest(result);
		}


	}
}
