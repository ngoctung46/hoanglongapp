using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Interfaces;
using WpfApp1.Model.Base;

namespace WpfApp1.Repos.Base
{
    public abstract class RepoBase<T> : IRepo<T> where T : ModelBase, new()
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseAddress = @"http://localhost:3000/api/";
        private readonly string _api;

        protected RepoBase()
        {
            _api = $"{_baseAddress}{typeof(T).Name.Trim().ToLower()}/";
            _httpClient = CreateHttpClient();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var api = $"{_api}/{id}";
            await _httpClient.DeleteAsync(api);
            return true;
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            var res = await _httpClient.GetAsync(_api);
            if (!res.IsSuccessStatusCode) return default(IEnumerable<T>);
            var resText = res.Content.ReadAsStringAsync().Result;
            var jObject = JObject.Parse(resText);
            var data = jObject["data"]?.ToString();
            return JsonConvert.DeserializeObject<IEnumerable<T>>(data);
        }

        public async Task<String> PostAsync(T entity)
        {
            if (entity == null) return String.Empty;
            var postContent = CreateStringContent(entity);
            var res = await _httpClient.PostAsync(_api, postContent);
            if (!res.IsSuccessStatusCode) return String.Empty;
            var restText = res.Content.ReadAsStringAsync().Result;
            var jObject = JObject.Parse(restText);
            var id = jObject["_id"]?.ToString();
            return id;
        }

        public async Task<bool> PutAsync(T entity)
        {
            if (entity == null) return false;
            var api = $"{_api}/{entity.Id}";
            var updateContent = CreateStringContent(entity);
            var res = await _httpClient.PutAsync(api, updateContent);
            return res.IsSuccessStatusCode;
        }

        internal HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        internal StringContent CreateStringContent(object obj)
        {
            var jObject = JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return new StringContent(jObject, Encoding.UTF8, "application/json");
        }
    }
}