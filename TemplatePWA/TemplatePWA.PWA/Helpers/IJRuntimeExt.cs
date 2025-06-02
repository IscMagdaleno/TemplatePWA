using Microsoft.JSInterop;

namespace TemplatePWA.PWA.Helpers
{
	public static class IJRuntimeExt
	{
		public static ValueTask<object> SetLocalStorage(this IJSRuntime js, string key, string content)
		=> js.InvokeAsync<object>("localstorage.setItem", key, content);

		public static ValueTask<object> GetLocalStorage(this IJSRuntime js, string key)
		=> js.InvokeAsync<object>("localstorage.getItem", key);

		public static ValueTask<object> RemoveItem(this IJSRuntime js, string key)
			=> js.InvokeAsync<object>(
				"localStorage.removeItem",
				key);
	}
}
