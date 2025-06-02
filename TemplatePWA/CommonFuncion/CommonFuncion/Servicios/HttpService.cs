
using CommonFuncion.Extensions;
using CommonFuncion.Logger;

using Serilog;

using System.Net;
using System.Text;
using System.Text.Json;

namespace CommonFuncion.Servicios
{
	public class HttpService : IHttpService
	{
		private readonly HttpClient httpClient;
		private readonly ILoggerHelper loggerHelper;

		private JsonSerializerOptions defaultJsonSerializerOptions =>
			new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
		public HttpService(HttpClient httpClient, ILoggerHelper _LoggerHelper)
		{
			this.httpClient = httpClient;
			loggerHelper = _LoggerHelper;
		}

		public async Task<HttpResponseWrapper<T>> Get<T>(string url)
		{
			var responseHTTP = await httpClient.GetAsync(url);
			if (responseHTTP.IsSuccessStatusCode)
			{
				var response = await Deserialize<T>(responseHTTP, defaultJsonSerializerOptions);
				var resultado = new HttpResponseWrapper<T>(response, true, responseHTTP);
				return resultado;
			}
			else
			{
				var resultado = new HttpResponseWrapper<T>(default, false, responseHTTP);
				return resultado;
			}
		}

		public async Task<HttpResponseWrapper<object>> Post<T>(string url, T data)
		{
			var dataJson = JsonSerializer.Serialize(data);
			var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync(url, stringContent);

			var resultado = new HttpResponseWrapper<object>(null, response.IsSuccessStatusCode, response);
			return resultado;
		}

		public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data)
		{

			loggerHelper.Info(url);

			var dataJson = JsonSerializer.Serialize(data);

			var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
			loggerHelper.Info(dataJson);

			var response = await httpClient.PostAsync(url, stringContent);
			if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
			{
				var responseDeserialized = await Deserialize<TResponse>(response, defaultJsonSerializerOptions);
				var resultado = new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);
				return resultado;
			}
			else
			{
				Log.Warning(url + response.StatusCode);

				if (response.IsNull())
				{
					response = new HttpResponseMessage();
				}
				var resultado = new HttpResponseWrapper<TResponse>(default, false, response);

				return resultado;

			}


		}

		public async Task<HttpResponseWrapper<TResponse>> Post<TResponse>(string url)
		{

			var stringContent = new StringContent("", Encoding.UTF8, "application/json");

			loggerHelper.Info(stringContent.ToString());

			var response = await httpClient.PostAsync(url, stringContent);
			if (response.IsSuccessStatusCode)
			{
				var responseDeserialized = await Deserialize<TResponse>(response, defaultJsonSerializerOptions);
				var resultado = new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);

				return resultado;
			}
			else
			{

				if (response.IsNull())
				{
					response = new HttpResponseMessage();
				}
				var resultado = new HttpResponseWrapper<TResponse>(default, false, response);
				return resultado;
			}
		}

		private async Task<T> Deserialize<T>(HttpResponseMessage httpResponse, JsonSerializerOptions options)
		{
			var responseString = await httpResponse.Content.ReadAsStringAsync();

			var resultado = JsonSerializer.Deserialize<T>(responseString, options);
			return resultado;
		}

	}
}
