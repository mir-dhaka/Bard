using System;
using System.Net;
using System.Net.Http;
using Fluent.Testing.Library.Infrastructure;
using Newtonsoft.Json;
using Shouldly;

namespace Fluent.Testing.Library.Then
{
    public abstract class ShouldBeBase : IShouldBeBase
    {
        private HttpResponseMessage? _httpResponse;
        private string _httpResponseString = string.Empty;

        public void StatusCodeShouldBe(HttpStatusCode statusCode)
        {
            if(_httpResponse == null)
                throw new Exception($"{nameof(_httpResponse)} property has not been set.");
            
            _httpResponse?.StatusCode.ShouldBe(statusCode,
                $"Status code mismatch, response was {_httpResponse.StatusCode}");
        }

        public void SetResponse(HttpResponseMessage httpResponse, string httpContent)
        {
            _httpResponse = httpResponse;
            _httpResponseString = httpContent;
        }
        
        public void Ok()
        {
            StatusCodeShouldBe(HttpStatusCode.OK);
        }

        public void NoContent()
        {
            StatusCodeShouldBe(HttpStatusCode.NoContent);
        }

        public T Ok<T>()
        {
            Ok();

            var content = Content<T>();

            content.ShouldNotBeNull($"Couldn't deserialize the result to a {typeof(T)}. Result was: {_httpResponseString}.");

            return content;
        }

        public void Created()
        {
            StatusCodeShouldBe(HttpStatusCode.Created);
        }

        public T Created<T>()
        {
            Created();

            var content = Content<T>();

            content.ShouldNotBeNull($"Couldn't deserialize the result to a {typeof(T)}. Result was: {_httpResponseString}.");

            return content;
        }

        public void Forbidden()
        {
            StatusCodeShouldBe(HttpStatusCode.Forbidden);
        }

        public void NotFound()
        {
            StatusCodeShouldBe(HttpStatusCode.NotFound);
        }
        
        public T Content<T>()
        {
            T content = default!;

            try
            {
                if (_httpResponseString != null)
                    content = JsonConvert.DeserializeObject<T>(_httpResponseString, new JsonSerializerSettings
                    {
                        ContractResolver = new ResolvePrivateSetters()
                    });
            }
            catch (Exception)
            {
                // ok..
            }

            return content ?? throw new Exception($"Unable to serialize to {typeof(T).FullName}");
        }
    }
}