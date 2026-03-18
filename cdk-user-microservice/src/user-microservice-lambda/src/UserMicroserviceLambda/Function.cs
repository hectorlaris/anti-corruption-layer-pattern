using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace UserMicroserviceLambda
{
    public class UserMicroservice
    {
        private HttpStatusCode retValue = HttpStatusCode.Accepted;
        public APIGatewayProxyResponse FunctionHandler(UserMicroserviceModel reqObj, ILambdaContext context)
        {
            try
            {
                //Process the data. e.g. save to database (not shown below)
                context.Logger.Log($"UserId: {reqObj.UserId}");
                context.Logger.Log($"Address: {reqObj.Address}");
                context.Logger.Log($"City: {reqObj.City}");
                context.Logger.Log($"State: {reqObj.State}");
                context.Logger.Log($"ZipCode: {reqObj.ZipCode}");
                context.Logger.Log($"Country: {reqObj.Country}");
                retValue = HttpStatusCode.OK;
                // End Lambda processing logic 
                var response = CreateResponse(retValue);
                return response;
            }
            catch (Exception ex)
            {
                context.Logger.Log($"Error: {ex.Message}");
                context.Logger.Log($"StackTrace: {ex.StackTrace}");
                retValue = HttpStatusCode.InternalServerError;
                var response = CreateResponse(retValue);
                response.Body = $"{{\"error\": \"{ex.Message}\"}}";
                return response;
            }
        }
        
        private APIGatewayProxyResponse CreateResponse(HttpStatusCode httpStatusCode)
        {
            int statusCode = (int)httpStatusCode;
            
            var response = new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = "Processed",
                Headers = new Dictionary<string, string>
                { 
                    { "Content-Type", "application/json" }, 
                    { "Access-Control-Allow-Origin", "*" } 
                }
            };
            return response;
        }
    }
}
