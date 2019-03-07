using System;
using Xunit;
using PactNet;
using PactNet.Mocks.MockHttpService;
using Consumer;
using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace tests
{
    public class ConsumerPactTests: IClassFixture<ConsumerPactClassFixture>
    {
        private IMockProviderService  _mockProviderService;
        private string _mockProviderServiceBaseUri;

        public ConsumerPactTests(ConsumerPactClassFixture fixture){
            _mockProviderService = fixture.MockProviderService;
            _mockProviderService.ClearInteractions();
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
        }
        
        [Fact]
        public void ItHandlesInvalidDateParam()
        {
            /*
            All test cases follow the same 3 steps
            1) Mock out an interaction withe provider api
            2) Interaction with the mocked out interaction using the Consumer [HttpDelete]
            3) Aseert the result is what we expected
            
            
            public IActionResult Delete(inputType id)
            {
                //TODO: Implement Realistic Implementation
                return Ok();
            }
            */

            var invalidRequestMessage = "validDateTime is not a date or time";
            _mockProviderService.Given("There is data")
                .UponReceiving("A invalid Get request for Date Validation with invalid data")
                .With(new ProviderServiceRequest{
                    Method = HttpVerb.Get,
                    Path = "/api/provider",
                    Query = "validDateTime=lolz"
                })
                .WillRespondWith(new ProviderServiceResponse {
                    Status = 400,
                    Headers = new Dictionary<string,object>
                    {
                        {"Content-type","application/json; charset=utf-8"}
                    },
                    Body = new
                    {
                        message = invalidRequestMessage
                    }
                });

                var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi("lolz",_mockProviderServiceBaseUri).GetAwaiter().GetResult();
                var resultBodyText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                Assert.Contains(invalidRequestMessage, resultBodyText);
        }
    }
}
