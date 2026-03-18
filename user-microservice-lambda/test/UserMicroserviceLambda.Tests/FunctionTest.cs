using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using UserMicroserviceLambda;

namespace UserMicroserviceLambda.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestUserMicroserviceFunction()
        {
            // Test the UserMicroservice Lambda function
            var function = new UserMicroservice();
            var context = new TestLambdaContext();

            var userModel = new UserMicroserviceModel
            {
                UserId = 12345,
                Address = "475 Sansome St, 10th floor",
                City = "San Francisco",
                State = "California",
                ZipCode = 94111,
                Country = "United States"
            };

            var response = function.FunctionHandler(userModel, context);

            Assert.NotNull(response);
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("Processed", response.Body);
        }
    }
}
