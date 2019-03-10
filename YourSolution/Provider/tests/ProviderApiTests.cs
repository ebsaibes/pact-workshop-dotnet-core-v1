using System;
using Xunit;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using tests.XUnitHelpers;
using Xunit.Abstractions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace tests
{
    public class ProviderApiTests : IDisposable
    {
        private string _providerUri { get; }
        private string _pactServiceUri { get; }

        private IWebHost _webHost { get; }
        private ITestOutputHelper _outputHelper { get; }

        public ProviderApiTests(ITestOutputHelper output)
        {
            _outputHelper = output;
            _providerUri = "http://localhost:9000";
            _pactServiceUri = "http://localhost:9001";

            _webHost = WebHost.CreateDefaultBuilder()
                .UseUrls(_pactServiceUri)
                .UseStartup<TestStartup>()
                .Build();

            _webHost.Start();
        }

        [Fact]
        public void TestOutput(){
            var name = "emil";
            _outputHelper.WriteLine("This is my name:{0}",name);
        }

        [Fact]
        public void EnsureProviderApiHonorsPactWithConsumer()
        {
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_outputHelper)
                },
                Verbose = true
            };

            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier.ProviderState($"{_pactServiceUri}/provider-states")
                .ServiceProvider("Provider", _providerUri)
                .HonoursPactWith("Consumer")
                .PactUri(@"..\..\..\..\..\pacts\consumer-provider.json")
                .Verify();
        }


        //implements IDisposable to make sure the server stops when the test is done
        #region disposable support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _webHost.StopAsync().GetAwaiter().GetResult();
                    _webHost.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }



        #endregion
    }
}
