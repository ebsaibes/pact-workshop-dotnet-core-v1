using System;
using Xunit;
using PactNet;
using PactNet.Mocks.MockHttpService;
using Consumer;
using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace tests
{
    public class ConsumerPactTests : IClassFixture<ConsumerPactClassFixture>
    {
        private IMockProviderService _mockProviderService;
        private string _mockProviderServiceBaseUri;

        public ConsumerPactTests(ConsumerPactClassFixture fixture)
        {
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
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/api/provider",
                    Query = "validDateTime=lolz"
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 400,
                    Headers = new Dictionary<string, object>
                    {
                        {"Content-type","application/json; charset=utf-8"}
                    },
                    Body = new
                    {
                        message = invalidRequestMessage
                    }
                });

            var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi("lolz", _mockProviderServiceBaseUri).GetAwaiter().GetResult();
            var resultBodyText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            Assert.Contains(invalidRequestMessage, resultBodyText);
        }

        [Fact]
        public void ValidDateRequired(){
            var invalidRequestMessage = "validDateTime is required";
            _mockProviderService
                .Given("There is data")
                .UponReceiving("An invalid Get Request with no date")
                .With(
                    new ProviderServiceRequest{
                        Method = HttpVerb.Get,
                        Path = "/api/provider",
                        Query = "validDateTime="
                    }
                )
                .WillRespondWith(
                    new ProviderServiceResponse{
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
                var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi("",_mockProviderServiceBaseUri).GetAwaiter().GetResult();
                var resultBodyText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                Assert.Contains(invalidRequestMessage, resultBodyText);
        } 


        [Fact]
        public void HandlesNoDataCorrectly(){
            var invalidRequestMessage = "There is no datafile";
            _mockProviderService
                .Given("There is no data")
                .UponReceiving("A valid date was sent but there is no datafile")
                .With(
                    new ProviderServiceRequest{
                        Method = HttpVerb.Get,
                        Path = "/api/provider",
                        Query = "validDateTime=04/04/2018"
                    }
                )
                .WillRespondWith(
                    new ProviderServiceResponse{
                        Status = 404
                    });
                var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi("04/04/2018",_mockProviderServiceBaseUri).GetAwaiter().GetResult();
                var resultStatus = (int)result.StatusCode;

                Assert.Equal(404, resultStatus);
        }

        [Fact]
        public void ItChecksDateParsing()
        {
            var expectedDateString = "10/08/2019";
            var expectedDateParsed = DateTime.Parse(expectedDateString).ToString("dd-MM-yyyy HH:mm:ss");

            string description = $"A valid date: {expectedDateString} was sent";
            _mockProviderService.Given("There is data")
                                .UponReceiving("A Valid Request is made but not parsed correctly")
                                .With(new ProviderServiceRequest
                                {
                                    Method = HttpVerb.Get,
                                    Path = "/api/provider",
                                    Query = $"validDateTime={expectedDateString}"
                                })
                                .WillRespondWith(new ProviderServiceResponse{
                                    Status = 200,
                                    Headers = new Dictionary<string, object>
                                    {
                                        {"Content-Type","application/json; charset=utf-8"}
                                    },
                                    Body = new
                                    {
                                        test = "YES",
                                        validDateTime = expectedDateParsed
                                    }
                                });

            var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi(expectedDateString, _mockProviderServiceBaseUri).GetAwaiter().GetResult();
            var resultBodyText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            Assert.Contains(expectedDateParsed, resultBodyText);

        }


        [Fact]
        public void ItParsesADateCorrectly()
        {
            var expectedDateString = "04/05/2018";
            var expectedDateParsed = DateTime.Parse(expectedDateString).ToString("dd-MM-yyyy HH:mm:ss");

            // Arrange
            _mockProviderService.Given("There is data")
                                .UponReceiving("A valid GET request for Date Validation")
                                .With(new ProviderServiceRequest 
                                {
                                    Method = HttpVerb.Get,
                                    Path = "/api/provider",
                                    Query = $"validDateTime={expectedDateString}"
                                })
                                .WillRespondWith(new ProviderServiceResponse {
                                    Status = 200,
                                    Headers = new Dictionary<string, object>
                                    {
                                        { "Content-Type", "application/json; charset=utf-8" }
                                    },
                                    Body = new 
                                    {
                                        test = "NO",
                                        validDateTime = expectedDateParsed
                                    }
                                });

            // Act
            var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi(expectedDateString, _mockProviderServiceBaseUri).GetAwaiter().GetResult();
            var resultBody = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            // Assert
            Assert.Contains(expectedDateParsed, resultBody);
        }

        // [Fact]
        // public void FakeTest()
        // {
        //     var invalidRequestMessage = "emil";
        //     _mockProviderService
        //         .Given("There is data")
        //         .UponReceiving("An invalid Date Request with My Name ")
        //         .With(
        //             new ProviderServiceRequest{
        //                 Method = HttpVerb.Get,
        //                 Path = "/api/provider",
        //                 Query = "validDateTime=emil"
        //             }
        //         )
        //         .WillRespondWith(
        //             new ProviderServiceResponse{
        //                 Status = 200,
        //                     Headers = new Dictionary<string,object>
        //                     {
        //                         {"Content-type","application/json; charset=utf-8"}
        //                     },
        //                     Body = new
        //                     {
        //                         message = "emil"
        //                     }
        //             });
        //         var result = ConsumerApiClient.ValidateDateTimeUsingProviderApi("emil",_mockProviderServiceBaseUri).GetAwaiter().GetResult();
        //         var resultBodyText = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        //         Assert.Contains(invalidRequestMessage, resultBodyText);
        // } 
    }
}
