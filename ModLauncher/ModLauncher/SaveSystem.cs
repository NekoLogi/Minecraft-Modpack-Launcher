using System.Xml;

namespace ModLauncher
{
    class SaveSystem
    {
        // Save Client Settings.
        public static void SaveSettings()
        {
            try
            {
                var document = new XmlDocument();
                var main_node = document.CreateElement("Settings");
                document.AppendChild(main_node);

                var ram_node = document.CreateElement("RAM");
                ram_node.InnerText = Settings.RAM.ToString();
                main_node.AppendChild(ram_node);

                var address_node = document.CreateElement("IP_Address");
                address_node.InnerText = Settings.IP_ADDRESS;
                main_node.AppendChild(address_node);

                var port_node = document.CreateElement("Port");
                port_node.InnerText = Settings.PORT.ToString();
                main_node.AppendChild(port_node);

                document.Save("Settings.xml");

            }
            catch (System.Exception e)
            {
                Settings.GetError(e.Message);
            }
        }

        // Load Client Settings.
        public static void LoadSettings()
        {
            using (var xml = new XmlTextReader("Settings.xml"))
            {
                try
                {
                    while (xml.Read())
                    {
                        if (xml.NodeType == XmlNodeType.Element && xml.Name == "RAM")
                        {
                            Settings.RAM = int.Parse(xml.ReadString());
                        }
                        else if (xml.NodeType == XmlNodeType.Element && xml.Name == "IP_Address")
                        {
                            Settings.IP_ADDRESS = xml.ReadString();
                        }
                        else if (xml.NodeType == XmlNodeType.Element && xml.Name == "Port")
                        {
                            Settings.PORT = int.Parse(xml.ReadString());
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Settings.GetError(e.Message);
                }
            }
        }
    }
}
