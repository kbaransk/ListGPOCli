using System;
using System.Xml;
using Microsoft.GroupPolicy;

namespace ListGPOCli {

	class Program {

		public static void Main(string[] args) {
			// Pobranie biezacej domeny i jej GPO
			GPDomain dom = new GPDomain();
			GpoCollection gpos = dom.GetAllGpos();
			foreach (Gpo gpo in gpos) {
				// iteracja po GPO i generacja raportow XML-owych dla GPO
				string xml = gpo.GenerateReport(ReportType.Xml);
				XmlDocument xd = new XmlDocument();
				xd.LoadXml(xml);
				XmlNamespaceManager xnm = new XmlNamespaceManager(xd.NameTable);
				xnm.AddNamespace("q1", "http://www.microsoft.com/GroupPolicy/Settings/Security");
				
				// Wyszuwanie tych polityk, ktore dotycza LmCompatibilityLevel
				XmlNodeList nl = xd.SelectNodes("//*[q1:KeyName[text()[contains(.,'LmCompatibilityLevel')]]]", xnm);
				if (nl.Count > 0) {
					// Wypisanie danych polityki i jej ustawien odpowiedzialnych za LmCompatibilityLevel
					Console.WriteLine("\n " + gpo.DisplayName);
					for (int i = 0; i< nl.Count; i++) {
						// Dla kazdego pasujacego elementu wypisanie info
						XmlNode node = nl.Item(i);
						String keyName = node.SelectSingleNode("//q1:KeyName", xnm).InnerText;
						String name = node.SelectSingleNode("//q1:Name", xnm).InnerText;
						String displayString = node.SelectSingleNode("//q1:DisplayString", xnm).InnerText;
						Console.WriteLine("Policy:         " + name);
						Console.WriteLine("Policy Setting: " + displayString);
					}
				}
				else {
					// Wyswietlenie polityki, ktora nie zawierala ustawienia
					Console.WriteLine("\n #" + gpo.DisplayName + " - nie zawiera");
				}
			}
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}