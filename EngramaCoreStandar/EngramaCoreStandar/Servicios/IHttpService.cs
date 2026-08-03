using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EngramaCoreStandar.Servicios
{
	public interface IHttpService
	{
		Task<HttpResponseWrapper<T>> Get<T>(string url);
		Task<HttpResponseWrapper<object>> Post<T>(string url, T data);
		Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data);
		Task<HttpResponseWrapper<TResponse>> Post<TResponse>(string url);
		Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T data, Dictionary<string, string> headers = null);
		Task<HttpResponseWrapper<TResponse>> PostWithFile<TResponse>(string url, StreamContent fileContent, string fieldName = "file", string fileName = null, Dictionary<string, string> headers = null);
		Task<HttpResponseWrapper<TResponse>> PostWithImage<TResponse>(string url, StreamContent imageContent, string fileName = null, string fieldName = "image", Dictionary<string, string> headers = null);
		Task<HttpResponseWrapper<TResponse>> PostDataContentVideo<TResponse>(string url, StreamContent? fileContent, string name);
		Task<HttpResponseWrapper<TResponse>> Put<T, TResponse>(string url, T data);
		Task<HttpResponseWrapper<object>> Delete(string url);
	}
}



