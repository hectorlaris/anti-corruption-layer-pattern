using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CdkUserMicroservice
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            // Get AWS Account ID from environment or CLI
            string? accountId = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT");
            string? region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION");

            // If environment variables are not set, try to get from AWS CLI
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(region))
            {
                try
                {
                    var processInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "aws",
                        Arguments = "sts get-caller-identity --query Account --output text",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    using (var process = System.Diagnostics.Process.Start(processInfo))
                    {
                        if (process != null)
                        {
                            accountId = process.StandardOutput.ReadToEnd().Trim();
                            region = "us-east-1";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error getting AWS account: {ex.Message}");
                    Console.Error.WriteLine("Please set CDK_DEFAULT_ACCOUNT and CDK_DEFAULT_REGION environment variables");
                    throw;
                }
            }

            new CdkUserMicroserviceStack(app, "CdkUserMicroserviceStack", new StackProps
            {
                Env = new Amazon.CDK.Environment
                {
                    Account = accountId,
                    Region = region,
                }
            });
            app.Synth();
        }
    }
}
