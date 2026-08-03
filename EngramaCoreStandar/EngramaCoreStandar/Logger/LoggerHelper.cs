using EngramaCoreStandar.Dapper.Interfaces;
using EngramaCoreStandar.Extensions;
using EngramaCoreStandar.Results;

using System;
using System.Collections.Generic;
using System.Linq;


namespace EngramaCoreStandar.Logger
{
	public class LoggerHelper : ILoggerHelper
	{
		public LoggerHelper()
		{

		}

		/// <summary>
		/// Mensaje informativo en consola
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void Info(string Mensaje)
		{
			Console.WriteLine(Mensaje);
		}


		/// <summary>
		/// Inserta log en consola la validación del resultado encapsulado en DataResult de la BD con interfaz DbResult
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void ValidateResult<T>(DataResult<T> Model) where T : class, DbResult, new()
		{

			if (Model.Ok)
			{
				if (Model.Data.bResult)
				{
					Console.WriteLine($"[{Model.Data.GetTypeValue(Model.Data.GetType().Name)}] - Resultado correcto");
				}
				else
				{
					Console.WriteLine($"[{Model.Data.GetType().Name}] - Alerta -[{Model.Data.vchMessage}]");
				}
			}
			else
			{
				Console.WriteLine($"[ ERROR - {Model.Msg} ]  ");
			}
		}

		/// <summary>
		/// Inserta log en consola la validación de la lista resultado encapsulado en DataResult de la BD con interfaz DbResult
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void ValidateResultLst<T>(DataResult<IEnumerable<T>> Model) where T : class, DbResult, new()
		{
			if (Model.Ok)
			{
				var validaLsita = Model.Data.ValidaResult();

				if (validaLsita.Item1)
				{
					Console.WriteLine($"[{Model.Data.GetType().Name}] - Resultado correcto");
				}
				else
				{
					Console.WriteLine($"[{Model.Data.GetType().Name}] - Alerta -[{validaLsita.Item2}]");
				}
			}
			else
			{
				Console.WriteLine($"[ ERROR - {Model.Msg}]  ");
			}
		}

		/// <summary>
		/// Inserta log en consola la validación del modelo resultado de la BD con interfaz DbResult
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		public void ValidateSPResult<T>(T? Model) where T : class, DbResult, new()
		{
			if (Model.IsNull())
			{
				Console.WriteLine($"[{typeof(T).Name}] - Error -- Modelo es Nulo]");
				return;
			}

			if (Model.bResult)
			{
				Console.WriteLine($"[{Model}] - Resultado correcto");
			}
			else
			{
				Console.WriteLine($"[{Model.GetType().Name}] - Alerta -[{Model.vchMessage}]");
			}
		}

		/// <summary>
		/// Valida si la lista que se espera por resultado contiene datos 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		/// <param name="Name"></param>
		public void ValidaLista<T>(IEnumerable<T> Model, string Name)
		{
			if (Model.IsNull())
			{
				Console.WriteLine($"[{Name}] - Alerta - Lista Nula]");
			}
			else if (Model.ToList().Count == 0)
			{
				Console.WriteLine($"[{Name}] - Alerta - Lista vacía]");
			}
			else
			{
				Console.WriteLine($"[{Name}] - Info - [{Model.Count()}]]");
			}
		}

		/// <summary>
		/// Valida si el objeto que se espera por resultado contiene datos 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="Model"></param>
		/// <param name="Name"></param>
		public void ValidaObj<T>(T Model, string Name)
		{
			if (Model.IsNull())
			{
				Console.WriteLine($"[{Name}] - Alerta - Objeto Nulo]");
			}
			else
			{
				Console.WriteLine($"[{Name}] - Info - [{Model.ToString()}]]");
			}
		}
	}
}