
using EngramaCoreStandar.Extensions;
using EngramaCoreStandar.Logger;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EngramaCoreStandar.Servicios
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
			try
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
			catch (Exception ex)
			{

				loggerHelper.Info(ex.Message);

				var resultado = new HttpResponseWrapper<T>(default, false, new HttpResponseMessage());

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
			var response = new HttpResponseMessage();
			try
			{

				var dataJson = JsonSerializer.Serialize(data);

				var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
				loggerHelper.Info(dataJson);

				response = await httpClient.PostAsync(url, stringContent);
				if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
				{
					var responseDeserialized = await Deserialize<TResponse>(response, defaultJsonSerializerOptions);
					var resultado = new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);
					return resultado;
				}
				else
				{
					Console.WriteLine(url + response.StatusCode);

					if (response.IsNull())
					{
						response = new HttpResponseMessage();
					}
					var resultado = new HttpResponseWrapper<TResponse>(default, false, response);

					return resultado;

				}
			}
			catch (Exception ex)
			{

				loggerHelper.Info(ex.Message);
				if (response.IsNull())
				{
					response = new HttpResponseMessage();
				}
				var resultado = new HttpResponseWrapper<TResponse>(default, false, response);

				return resultado;
			}


		}

		public async Task<HttpResponseWrapper<TResponse>> PostDataContentVideo<TResponse>(string url, StreamContent? fileContent, string name)
		{
			loggerHelper.Info(url);

			using var formData = new MultipartFormDataContent();
			formData.Add(fileContent, "video", name);

			var response = await httpClient.PostAsync(url, formData);
			if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
			{
				var responseDeserialized = await Deserialize<TResponse>(response, defaultJsonSerializerOptions);
				return new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);
			}
			else
			{
				Console.WriteLine(url + response.StatusCode);
				if (response.IsNull()) response = new HttpResponseMessage();
				return new HttpResponseWrapper<TResponse>(default, false, response);
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



		public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data, Dictionary<string, string> headers = null)
		{
			loggerHelper.Info(url);

			var dataJson = JsonSerializer.Serialize(data);
			var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
			loggerHelper.Info(dataJson);

			// Add headers to the request
			if (headers != null)
			{
				foreach (var header in headers)
				{
					if (httpClient.DefaultRequestHeaders.NotAny(e => e.Key == header.Key))
					{
						httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
					}
				}
			}

			var response = await httpClient.PostAsync(url, stringContent);

			if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
			{
				var responseDeserialized = await Deserialize<TResponse>(response, defaultJsonSerializerOptions);
				var resultado = new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);
				return resultado;
			}
			else
			{
				Console.WriteLine(url + response.StatusCode);

				if (response.IsNull())
				{
					response = new HttpResponseMessage();
				}

				var resultado = new HttpResponseWrapper<TResponse>(default, false, response);
				return resultado;
			}
		}


		public async Task<HttpResponseWrapper<TResponse>> PostWithFile<TResponse>(string url, StreamContent? fileContent, string fieldName = "file", string fileName = null, Dictionary<string, string> headers = null)
		{
			loggerHelper.Info(url);

			using var formData = new MultipartFormDataContent();
			var contentName = fileName ?? $"{fieldName}_{DateTime.Now:yyyyMMddHHmmss}";
			formData.Add(fileContent, fieldName, contentName);

			if (headers != null)
			{
				foreach (var header in headers)
				{
					if (httpClient.DefaultRequestHeaders.NotAny(e => e.Key == header.Key))
						httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
				}
			}

			var response = await httpClient.PostAsync(url, formData);

			if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
			{
				var responseDeserialized = await Deserialize<TResponse>(response, defaultJsonSerializerOptions);
				return new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);
			}
			else
			{
				Console.WriteLine(url + response.StatusCode);
				if (response.IsNull()) response = new HttpResponseMessage();
				return new HttpResponseWrapper<TResponse>(default, false, response);
			}
		}

		public async Task<HttpResponseWrapper<TResponse>> PostWithImage<TResponse>(string url, StreamContent imageContent, string fileName = null, string fieldName = "image", Dictionary<string, string> headers = null)
		{
			loggerHelper.Info(url);

			using var formData = new MultipartFormDataContent();
			var contentName = fileName ?? $"image_{DateTime.Now:yyyyMMddHHmmss}.jpg";
			formData.Add(imageContent, fieldName, contentName);

			if (headers != null)
			{
				foreach (var header in headers)
				{
					if (httpClient.DefaultRequestHeaders.NotAny(e => e.Key == header.Key))
						httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
				}
			}

			var response = await httpClient.PostAsync(url, formData);

			if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
			{
				var responseDeserialized = await Deserialize<TResponse>(response, defaultJsonSerializerOptions);
				return new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);
			}
			else
			{
				Console.WriteLine(url + response.StatusCode);
				if (response.IsNull()) response = new HttpResponseMessage();
				return new HttpResponseWrapper<TResponse>(default, false, response);
			}
		}



		public async Task<HttpResponseWrapper<TResponse>> Put<T, TResponse>(string url, T data)
		{
			loggerHelper.Info(url);
			try
			{
				var dataJson = JsonSerializer.Serialize(data);
				var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
				var response = await httpClient.PutAsync(url, stringContent);

				if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
				{
					var responseDeserialized = await Deserialize<TResponse>(response, defaultJsonSerializerOptions);
					return new HttpResponseWrapper<TResponse>(responseDeserialized, true, response);
				}

				return new HttpResponseWrapper<TResponse>(default, false, response);
			}
			catch (Exception ex)
			{
				loggerHelper.Info(ex.Message);
				return new HttpResponseWrapper<TResponse>(default, false, new HttpResponseMessage());
			}
		}

		public async Task<HttpResponseWrapper<object>> Delete(string url)
		{
			loggerHelper.Info(url);
			try
			{
				var response = await httpClient.DeleteAsync(url);
				return new HttpResponseWrapper<object>(null, response.IsSuccessStatusCode, response);
			}
			catch (Exception ex)
			{
				loggerHelper.Info(ex.Message);
				return new HttpResponseWrapper<object>(null, false, new HttpResponseMessage());
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
