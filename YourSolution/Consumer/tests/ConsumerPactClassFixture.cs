using System;
using Xunit;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace tests
{

    public class ConsumerPactClassFixture : IDisposable
    {
        // public ConsumerPactClassFixture(IPactBuilder pactBuilder, IMockProviderService mockProviderService)
        // {
        //     this.PactBuilder = pactBuilder;
        //     this.MockProviderService = mockProviderService;

        // }
        public IPactBuilder PactBuilder { get; private set; }

        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort
        {
            get
            {
                return 9222;
            }
        }

        //mock server will be placed http://localhost:9222
        public string MockProviderServiceBaseUri
        {
            get
            {
                return String.Format("http://localhost:{0}", MockServerPort);
            }
        }

        public ConsumerPactClassFixture()
        {
            var pactConfig = new PactConfig{
                SpecificationVersion = "2.0.0",
                PactDir = @"..\..\..\..\..\pacts",
                LogDir = @".\pact_logs"
            };

            PactBuilder = new PactBuilder(pactConfig);
            PactBuilder.ServiceConsumer("Consumer")
                        .HasPactWith("Provider");

            MockProviderService = PactBuilder.MockService(MockServerPort);

        }

        #region IDispoable support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // This will save the pact file once finished.
                    PactBuilder.Build();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion


               


    }
}