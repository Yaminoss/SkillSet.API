using System.Xml;
using SkillSet.API.Models;

namespace SkillSet.API.Services
{

    public static class OwnerXmlParser
    {
        public static List<Owner> Parse(string sourceFolderPath, List<string> provinces)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(sourceFolderPath);
            var owners = new List<Owner>();
            XmlNodeList nodes = xml.DocumentElement.SelectNodes("/root/owners/owner");
            foreach (XmlNode node in nodes)
            {
                try
                {
                    var owner = new Owner()
                    {
                        Id = Int16.Parse(node.SelectSingleNode("Id").InnerText),
                        FirstName = node.SelectSingleNode("FirstName")?.InnerText,
                        LastName = node.SelectSingleNode("LastName")?.InnerText,
                        DateOfBirth = DateTime.Parse(node.SelectSingleNode("DateOfBirth")?.InnerText),
                        Address = node.SelectSingleNode("Address")?.InnerText
                    };
                    if (owner.Address != null && provinces.Contains(SplitAddressBySeperator(owner.Address)))
                        owners.Add(owner);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Parsing Failed With error: " + ex.Message);
                }
            }
            return owners;
        }

        public static string SplitAddressBySeperator(string address)
        {
            return address.Split(',')[1].Split(' ')[1];
        }
    }
}
