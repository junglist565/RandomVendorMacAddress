using Microsoft.Win32;
using RandomVendorMacAddress.Entity;
using RandomVendorMacAddress.Manager;
using System;
using System.IO;
using System.Management;
using System.Threading;

namespace RandomVendorMacAddress
{
    public sealed class MacChanger
    {
        #region Constants
        public const String RANDOM_MODE = "random";
        public const String VENDOR_MODE = "vendor";
        public const String MANUAL_MODE = "manual";
        public const String RESET_MODE = "reset";
        public const String LIST_MODE = "list";

        private const String REGISRTY_KEY = @"SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002bE10318}\";
        private const String ADDRESS_REG_KEY = "NetworkAddress";
        #endregion

        private NetworkManager _networkManager = null;
        private MacAddress _address = null;

        public MacChanger(NetworkManager networkManager, MacAddress address)
        {
            _address = address;
            _networkManager = networkManager;
        }

        public void ChangeMacAddress()
        {
            RegistryKey adapterRegKey = FindAdapterRegistryKey();

            if (adapterRegKey != null)
            {
                adapterRegKey.SetValue(ADDRESS_REG_KEY, _address.Address, RegistryValueKind.String);
                adapterRegKey.Close();

                RestartAdapter();
            }
        }

        public void ResetMacAddress()
        {
            RegistryKey adapterRegKey = FindAdapterRegistryKey();

            if (adapterRegKey != null)
            {
                adapterRegKey.DeleteValue(ADDRESS_REG_KEY, true);
                adapterRegKey.Close();

                RestartAdapter();
            }
        }

        private void RestartAdapter()
        {
            String selectCurrentAdapter =
                String.Format("SELECT * FROM Win32_NetworkAdapter WHERE Description = \"{0}\"", _networkManager.AdapterDescription);
            ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher(new SelectQuery(selectCurrentAdapter));

            foreach (ManagementObject elem in objectSearcher.Get())
            {
                elem.InvokeMethod("Disable", null);
                Thread.Sleep(1000);
                elem.InvokeMethod("Enable", null);
            }
        }

        private RegistryKey FindAdapterRegistryKey()
        {
            const String interfaceIdRegKey = "NetCfgInstanceId";
            RegistryKey baseRegKey = Registry.LocalMachine.OpenSubKey(REGISRTY_KEY, true);
            String[] subkeys = baseRegKey.GetSubKeyNames();
            RegistryKey adapterRegKey = null;
            RegistryKey output = null;

            // Search for correct interface ID
            foreach (String subkey in subkeys)
            {
                adapterRegKey = Registry.LocalMachine.OpenSubKey(REGISRTY_KEY + subkey + Path.DirectorySeparatorChar, true);
                String adapterId = (string) adapterRegKey.GetValue(interfaceIdRegKey);

                if (adapterId == _networkManager.AdapterId)
                {
                    output = adapterRegKey;
                    baseRegKey.Close();
                    break;
                }
            }

            return output;
        }
    }
}
