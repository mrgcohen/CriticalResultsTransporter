
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace CriticalResults
{
	class ProgramConsole
	{
		static void Main(string[] args)
		{
			Console.WriteLine("*****************************");
			Console.WriteLine("Critical Results Notify Agent");
			Console.WriteLine("*****************************");

			string response = "";
			if (args.Count() == 1)
			{
				response = args[0];
				Console.WriteLine("Option {0} passed by command line.", response);
			}
			do
			{
				switch(response)
				{
					case "h":
						RunHybridAgent();
						break;
                    //case "s":
                    //    SendMessageViaASPNetTransport();
                    //    break;
					case "w":
						SendMessageViaWCFTransport();
						break;
					case "u":
						SP2I2_AddExamAndPatientKeys.Run();
						break;
					default:
						Console.WriteLine("Response did not match any valid options.");
						break;
				}
				Console.WriteLine("h - Run HYBRID notify agent.");
				//Console.WriteLine("s - Send a message via ASP.Net Transport");
				Console.WriteLine("w - Send a message via ITransport/WCF");
				Console.WriteLine("-Enter- to exit");
				response = Console.ReadLine();
			} while (!string.IsNullOrEmpty(response));
		}


		private static void SendMessageViaWCFTransport()
		{
			TransportServiceReference.TransportServiceClient client = new CriticalResults.TransportServiceReference.TransportServiceClient();
			Console.WriteLine("Client Address: {0}", client.Endpoint.Address);

			//client.Endpoint.Address = new EndpointAddress("http://localhost:3193/PagingService.svc");
			client.Endpoint.Address = new EndpointAddress("http://ancr.partners.org/CriticalResultsTransporter/PartnersServices/PagingService.svc");
			Console.WriteLine("Client Address: {0}", client.Endpoint.Address);

			Console.Write("To: ");
			string address = Console.ReadLine();
			Console.Write("Body: ");
			string body = Console.ReadLine();

			bool ok = client.RequestNotification(Guid.NewGuid().ToString(), address, body);
			Console.WriteLine("Request sent.  Request was successful: {0}", ok);
		}

		static void RunHybridAgent()
		{
			HybridAgent agent = new HybridAgent();
		}

	}
}