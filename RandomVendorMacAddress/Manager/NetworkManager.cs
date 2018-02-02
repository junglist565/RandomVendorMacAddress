using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace RandomVendorMacAddress.Manager
{
    public class NetworkManager
    {
        private NetworkInterface _adapter = null;
        private bool _connected = false;
        private String _adapterName = "";
        private String _adapterDescription = "";
        private String _macAddress = "";
        private String _adapterId = "";

        #region Properties
        public NetworkInterface Adapter
        {
            get { return _adapter; }
            private set { _adapter = value; }
        }

        public bool IsAdapterUp
        {
            get { return _connected; }
            private set { _connected = value; }
        }

        public String AdapterName
        {
            get { return _adapterName; }
            private set { _adapterName = value; }
        }

        public String AdapterDescription
        {
            get { return _adapterDescription; }
            private set { _adapterDescription = value; }
        }

        public String CurrentMacAddress
        {
            get { return _macAddress; }
            private set { _macAddress = value; }
        }

        public String AdapterId
        {
            get { return _adapterId; }
            private set { _adapterId = value; }
        }
        #endregion

        public NetworkManager(String interfaceName)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            // Search the correct adapter name
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Name == interfaceName)
                {
                    Adapter = adapter;
                    IsAdapterUp = adapter.OperationalStatus == OperationalStatus.Up;
                    AdapterDescription = Adapter.Description;
                    AdapterName = interfaceName;
                    CurrentMacAddress = Adapter.GetPhysicalAddress().ToString();
                    AdapterId = Adapter.Id;
                    break;
                }
            }

            if (_adapter == null)
                throw new ArgumentException("Adapter not found");
        }

        public static List<String> ListInterfaces()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            String outputFormat = "Name: [{0}] | Description: [{1}] | Status: {2}";
            List<String> output = new List<String>();

            foreach (NetworkInterface adapter in adapters)
            {
                String connected = adapter.OperationalStatus == OperationalStatus.Up ? "Up" : "Down";
                output.Add(String.Format(outputFormat, adapter.Name, adapter.Description, connected));
            }

            return output;
        }
    }
}
