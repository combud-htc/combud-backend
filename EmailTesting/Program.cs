using System;
using System.Collections.Generic;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon.Runtime;
using Amazon.Util;

namespace EmailTesting
{
	class Program
	{
		static readonly string senderAddress = "";

		static readonly string receiverAddress = "";

		static readonly string configSet = "xd";

		static readonly string subject = "Amazon SES test (AWS SDK for .NET)";

		// The email body for recipients with non-HTML email clients.
		static readonly string textBody = "Amazon SES Test (.NET)\r\n"
										+ "This email was sent through Amazon SES "
										+ "using the AWS SDK for .NET.";

		// The HTML body of the email.
		static readonly string htmlBody = @"<html>
<head></head>
<body>
  <h1>Amazon SES Test (AWS SDK for .NET)</h1>
  <p>This email was sent with
	<a href='https://aws.amazon.com/ses/'>Amazon SES</a> using the
	<a href='https://aws.amazon.com/sdk-for-net/'>
	  AWS SDK for .NET</a>.</p>
</body>
</html>";

		static void Main(string[] args)
		{

			using (var client = new AmazonSimpleEmailServiceClient("", "", RegionEndpoint.EUWest2))
			{
				var sendRequest = new SendEmailRequest
				{
					Source = senderAddress,
					Destination = new Destination
					{
						ToAddresses =
						new List<string> { receiverAddress }
					},
					Message = new Message
					{
						Subject = new Content(subject),
						Body = new Body
						{
							Html = new Content
							{
								Charset = "UTF-8",
								Data = htmlBody
							},
							Text = new Content
							{
								Charset = "UTF-8",
								Data = textBody
							}
						}
					},

					ConfigurationSetName = configSet
				};
				try
				{
					Console.WriteLine("Sending email using Amazon SES...");
					var response = client.SendEmailAsync(sendRequest).GetAwaiter().GetResult();
					Console.WriteLine("The email was sent successfully.");
				}
				catch (Exception ex)
				{
					Console.WriteLine("The email was not sent.");
					Console.WriteLine("Error message: " + ex.Message);

				}
			}

			Console.Write("Press any key to continue...");
			Console.ReadKey();
		}
	}
}
