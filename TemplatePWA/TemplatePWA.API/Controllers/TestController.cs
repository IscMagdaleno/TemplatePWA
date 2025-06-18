using Microsoft.AspNetCore.Mvc;

using TemplatePWA.API.EngramaLevels.Dominio.Interfaces;
using TemplatePWA.Share.PostClass;

namespace TemplatePWA.API.Controllers
{
	/// <summary>
	/// Test controller to show how Engrama work
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	public class TestController : ControllerBase
	{
		private readonly ITestDominio testDominio;

		public TestController(ITestDominio testDominio)
		{
			this.testDominio = testDominio;
		}


		/// <summary>
		/// Test method to show you how to connect the API with the database using Engrama.
		/// </summary>
		/// <param name="postModel"></param>
		/// <returns></returns>
		[HttpPost("PostTestTable")]
		public async Task<IActionResult> PostTestTable([FromBody] PostTestTable postModel)
		{
			var result = await testDominio.TestTable(postModel);
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
		public async Task<IActionResult> PostSaveTest_Table([FromBody] PostSaveTest_Table postModel)
		{
			var result = await testDominio.SaveTest_Table(postModel);
			if (result.IsSuccess)
			{
				return Ok(result);
			}
			return BadRequest(result);
		}


	}
}
