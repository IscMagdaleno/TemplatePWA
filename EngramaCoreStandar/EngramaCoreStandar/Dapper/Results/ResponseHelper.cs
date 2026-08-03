using EngramaCoreStandar.Dapper.Interfaces;
using EngramaCoreStandar.Extensions;
using EngramaCoreStandar.Mapper;

using System.Collections.Generic;
using System.Linq;

namespace EngramaCoreStandar.Results
{
	public class ResponseHelper : IResponseHelper
	{
		private readonly MapperHelper _mapperHelper;

		public ResponseHelper(MapperHelper mapperHelper)
		{
			_mapperHelper = mapperHelper;
		}

		public Response<IEnumerable<TResultado>> Validacion<TProducto, TResultado>(IEnumerable<TProducto> ModeloValidar)
				 where TProducto : DbResult
		{
			var response = new Response<IEnumerable<TResultado>>();
			response.Data = new List<TResultado>();

			if (ModeloValidar.IsEmpty().False())
			{
				var validacion = ValidateErrors(ModeloValidar);
				if (validacion.Ok)
				{
					response.IsSuccess = true;
					response.Message = ModeloValidar.FirstOrDefault().vchMessage;
					response.Data = _mapperHelper.GetEnumerable<TProducto, TResultado>(ModeloValidar);
				}
				else
				{
					response.IsSuccess = false;
					response.Message = validacion.Msg;
				}
			}

			return response;
		}

		public Response<TResultado> Validacion<TProducto, TResultado>(TProducto ModeloValidar)
					 where TProducto : DbResult
			where TResultado : new()

		{
			var response = new Response<TResultado>();
			response.Data = new TResultado();

			if (ModeloValidar.IsNull().False())
			{
				var validacion = ValidateError(ModeloValidar);
				if (validacion.Ok)
				{
					response.IsSuccess = true;
					response.Message = ModeloValidar.vchMessage;
					response.Data = _mapperHelper.Get<TProducto, TResultado>(ModeloValidar);
				}
				else
				{
					response.IsSuccess = false;
					response.Message = validacion.Msg;
				}

			}

			return response;

		}

		public Response<TResultado> ValidacionSinMapper<TProducto, TResultado>(TProducto ModeloValidar)
				 where TProducto : DbResult
		where TResultado : new()
		{
			var response = new Response<TResultado>();
			response.Data = new TResultado();

			if (ModeloValidar.IsNull().False())
			{
				var validacion = ValidateError(ModeloValidar);
				if (validacion.Ok)
				{
					response.IsSuccess = true;
					response.Message = ModeloValidar.vchMessage;
				}
				else
				{
					response.IsSuccess = false;
					response.Message = validacion.Msg;
				}

			}

			return response;

		}

		public Response<IEnumerable<TResultado>> ValidacionSinMapper<TProducto, TResultado>(IEnumerable<TProducto> ModeloValidar)
				 where TProducto : DbResult
		where TResultado : new()
		{
			var response = new Response<IEnumerable<TResultado>>();
			response.Data = new List<TResultado>();

			if (ModeloValidar.IsNull().False())
			{
				var validacion = ValidateErrors(ModeloValidar);
				if (validacion.Ok)
				{
					response.IsSuccess = true;
					response.Message = ModeloValidar.FirstOrDefault().vchMessage;
				}
				else
				{
					response.IsSuccess = false;
					response.Message = validacion.Msg;
				}

			}

			return response;

		}



		private static Result ValidateErrors<T>(IEnumerable<T> data)
		 where T : DbResult
		{
			foreach (var item in data)
			{
				var validateErrorResult = ValidateError(item);

				if (validateErrorResult.Ok.False())
				{
					return Result.Fail(validateErrorResult.Msg);
				}
			}

			return Result.Success();
		}

		private static Result ValidateError<T>(T obj)
			 where T : DbResult
		{

			if (obj.bResult.False())
			{
				return Result.Fail(obj.vchMessage);

			}
			return Result.Success();


		}
	}
}
