using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TrainData {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("--- Downloading train data ---");

            WebClient webclient = new WebClient();
            webclient.UploadStringCompleted += (obj, arguments) => {
                if (arguments.Cancelled == true) {
                    Console.Write("Request cancelled.");
                }
                else if (arguments.Error != null) {
                    Console.WriteLine(arguments.Error.Message);
                    Console.Write("Request failed.");
                }
                else {
                    formatResponse(arguments.Result);
                    Console.Write("Data downloaded.");
                }
                Console.WriteLine("\n");
                Console.WriteLine("Type 'Q' to exit.");
            };

            try {
                Uri address = new Uri("http://api.trafikinfo.trafikverket.se/v1.3/data.xml");
                string requestBody = "<REQUEST>" +
                                        "<LOGIN authenticationkey='848990896d3840c9add7b889010d322e'/>" +
                                        "<QUERY objecttype='TrainAnnouncement'>" +
                                            "<FILTER>" +
                                                "<IN name='ActivityType' value='Ankomst'/>" +
                                            "</FILTER>" +
                                            "<EXCLUDE>Deleted</EXCLUDE>" +
                                        "</QUERY>" +
                                    "</REQUEST>";

                webclient.Headers["Content-Type"] = "text/xml";
                Console.WriteLine("Fetching data ... (press 'C' to cancel)");
                webclient.UploadStringAsync(address, "POST", requestBody);
            }
            catch (UriFormatException) {
                Console.WriteLine("Bad url! Type 'Q' to exit.");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine("An error occured! Type 'Q' to exit.");
            }

            char keychar = ' ';
            while (keychar != 'Q') {
                keychar = Char.ToUpper(Console.ReadKey().KeyChar);
                if (keychar == 'C') {
                    webclient.CancelAsync();
                }
            }
            Console.WriteLine("\n");
        }

        private static string formatResponse(string xml) {
            XDocument doc = XDocument.Parse(xml);
            XmlWriterSettings xmlsettings = new XmlWriterSettings();
            xmlsettings.Indent = true;
            xmlsettings.IndentChars = "   ";
            xmlsettings.OmitXmlDeclaration = true;
            var sb = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(sb, xmlsettings)) {
                doc.WriteTo(xmlWriter);
            }
            doc.Save("result.xml");
            return sb.ToString();
        }
    }
}