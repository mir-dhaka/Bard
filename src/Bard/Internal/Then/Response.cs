using System.Net;
using Bard.Infrastructure;
using Bard.Internal.When;

namespace Bard.Internal.Then
{
    internal class Response : IResponse, ITime
    {
        private readonly ApiResult _apiResult;
        private readonly LogWriter _logWriter;
        private readonly ShouldBe _shouldBe;
        private readonly Headers _headers;
        private int? _maxElapsedTime;

        internal Response(EventAggregator eventAggregator, ApiResult apiResult, IBadRequestProvider badRequestProvider,
            LogWriter logWriter)
        {
            _apiResult = apiResult;
            _logWriter = logWriter;
            _shouldBe = new ShouldBe(apiResult, badRequestProvider, logWriter);
            _headers = new Headers(apiResult, logWriter);
            eventAggregator.Subscribe(_shouldBe);
        }

        public IShouldBe ShouldBe => _shouldBe;

        public IHeaders Headers => _headers;

        bool IResponse.Log
        {
            get => _shouldBe.Log;
            set => _shouldBe.Log = value;
        }

        public void StatusCodeShouldBe(HttpStatusCode statusCode)
        {
            ShouldBe.StatusCodeShouldBe(statusCode);
        }

        public T Content<T>()
        {
            return _shouldBe.Content<T>();
        }

        public void WriteResponse()
        {
            _logWriter.WriteHttpResponseToConsole(_apiResult);
        }

        public ITime Time => this;

        int? IResponse.MaxElapsedTime
        {
            get => _maxElapsedTime;
            set
            {
                _maxElapsedTime = value;
                _headers.MaxElapsedTime = value;
                _shouldBe.MaxElapsedTime = value;
            }
        }

        public void LessThan(int milliseconds)
        {
            _logWriter.LogHeaderMessage($"THEN THE RESPONSE SHOULD BE LESS THAN {milliseconds} MILLISECONDS");
            _logWriter.WriteHttpResponseToConsole(_apiResult);

            _apiResult.AssertElapsedTime(milliseconds);
        }
    }
}