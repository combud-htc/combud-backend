using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon.Runtime;
using Amazon.Util;

namespace Api
{
	public class Helper
	{
		public static async Task<bool> SendEmail(string sender, string recipient, string subject, string body, string accedId, string accessKey)
		{	try
			{
				using (AmazonSimpleEmailServiceClient client = new AmazonSimpleEmailServiceClient(accedId, accessKey, RegionEndpoint.EUWest2))
				{
					SendEmailRequest req = new SendEmailRequest();
					
					req.Source = sender;
					req.Destination = new Destination(new List<string>() { recipient });
					req.Message = new Message();
					Content html = new Content(body);
					req.Message.Body = new Body();
					req.Message.Body.Html = html;
					req.Message.Body.Text = new Content("Please enable html");
					req.Message.Subject = new Content(subject);

					SendEmailResponse res = await client.SendEmailAsync(req).ConfigureAwait(false);
					return true;
				}
			} catch
			{
				return false;
			}
		}

		public static bool IsValidEmail(string email)
		{
			try
			{
				MailAddress addr = new MailAddress(email);
				return addr.Address == email && email.Length <= 50;
			}
			catch
			{
				return false;
			}
		}

		public static string RandomString(int length)
		{
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public static bool IsClose(Post p, float lat, float lon, int maxdist)
		{
			if ((p.Latitude == lat) && (p.Longitude == lon)) return true;
			double theta = p.Longitude - lon;
			double dist = Math.Sin(deg2rad(p.Latitude)) * Math.Sin(deg2rad(lat)) + Math.Cos(deg2rad(p.Latitude)) * Math.Cos(deg2rad(lat)) * Math.Cos(deg2rad(theta));
			dist = rad2deg(Math.Acos(dist));
			dist = dist * 60 * 1.1515;
			return dist <= maxdist;
		}

		public static double deg2rad(double deg) => (deg * Math.PI / 180.0);
		public static double rad2deg(double rad) => (rad / Math.PI * 180.0);
	}
}
