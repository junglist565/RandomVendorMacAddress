using RandomVendorMacAddress.Entity;
using RandomVendorMacAddress.Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Security.Principal;

namespace RandomVendorMacAddress
{
    class Program
    {
        private const String ESCAPE_DEFAULT_MESSAGE = "Press any key to exit";

        private static bool _reset = false;
        private static bool _list = false;
        private static String _target = null;
        private static MacAddress _newAddress = null;

        private static void Usage()
        {
            String message =
                "run.exe [mode] [target]\n\n" +
                "Available modes:\n" +
                "random:\tSet a random MAC address to specified target\n" +
                "vendor:\tGet an existing random vendor ID for MAC address.\n\t" +
                "If you specify a search option with '=' character " +
                "(ex: vendor=\"cisco\"), you can choose a Vendor from the resulting list of your query.\n\t" +
                "If the research doesn't match any result, it will be set a random vendor\n" +
                "manual:\tSets a manual MAC address to specified target\n\t" +
                "In this case you must specify the address by '=' character (ex: manual=00:11:22:33:44:55)\n" +
                "reset:\tRestore the original address for specified target\n" +
                "list:\tDisplay all available adapters\n\n" + 
                "Target name is case-sensitive!";
            TerminateExecution(message);
        }

        private static void GetOpts(string[] argv)
        {
            if (argv.Length == 0)
                Usage();

            String[] @params = argv[0].Split('=');

            switch(@params[0])
            {
                case MacChanger.RANDOM_MODE:
                    _newAddress = new RandomMacAddress();
                    break;

                case MacChanger.VENDOR_MODE:
                    ManageVendorMode(@params);
                    break;

                case MacChanger.MANUAL_MODE:
                    if (@params.Length > 1)
                    {
                        try { _newAddress = new MacAddress(@params[1]); }
                        catch (ArgumentException ex) { TerminateExecution(ex.Message); }
                    }
                    else
                        Usage();
                    break;

                case MacChanger.RESET_MODE:
                    _reset = true;
                    break;

                case MacChanger.LIST_MODE:
                    _list = true;
                    break;

                default:
                    Usage();
                    break;
            }

            if (!_list)
                if (argv.Length > 1)
                    _target = argv[1];
                else
                    Usage();
        }

        private static void ManageVendorMode(string[] args)
        {
            const String errorMessage =
                "Can't connect to DB\nCheck if \"macvendors.db\" file exists or is not corrupted\n" +
                "You may run \"create_db.exe\" script for creating a new DB\n";
            SQLiteAccess dbAccess = new SQLiteAccess("macvendors.db");

            dbAccess.OpenConnection();

            VendorDAO vendorDAO = new VendorDAO(dbAccess.Connection);
            if (args.Length > 1)
            {
                String searchQuery = args[1].Replace("\"", "");

                Dictionary<long, Vendor> resultList = null;

                try { resultList = vendorDAO.SearchQueryResultList(searchQuery); }
                catch (SQLiteException) { TerminateExecution(errorMessage); }
                catch (ArgumentException ex) { TerminateExecution(ex.Message); }

                if (resultList.Count == 0)
                {
                    Console.WriteLine("No vendors matching your query. It will be set a random vendor");
                    Console.WriteLine("Press any key...");
                    Console.ReadLine();
                    try { _newAddress = vendorDAO.RandomVendor(); }
                    catch (SQLiteException) { TerminateExecution(errorMessage); }
                }
                else
                {
                    Console.WriteLine(String.Format("Found {0} result(s) for \"{1}\"\n", resultList.Count, searchQuery));
                    try { _newAddress = ChosenVendor(resultList, vendorDAO); }
                    catch (SQLiteException) { TerminateExecution(errorMessage); }
                }
            }
            else
                try { _newAddress = vendorDAO.RandomVendor(); }
                catch (SQLiteException) { TerminateExecution(errorMessage); }

            dbAccess.Dispose();
        }

        private static Vendor ChosenVendor(Dictionary<long, Vendor> resultList, VendorDAO vendorDAO)
        {
            Vendor output = null;

            // Print results on console
            foreach (Vendor elem in resultList.Values)
            {
                String format = "Vendor ID [{0}] ##### Company Name [{1}]\nMAC's owned: {2}\n";
                Console.WriteLine(String.Format(format, elem.Id, elem.CompanyName, elem.MacsOwned));
            }

            Console.WriteLine("Type a vendor ID...");

            do
            {
                try
                {
                    int id = Int32.Parse(Console.ReadLine());
                    output = vendorDAO.RandomVendorByName(resultList[id].CompanyName);
                }
                catch (Exception ex) when (ex is KeyNotFoundException || ex is FormatException)
                {
                    Console.WriteLine("Please enter a valid ID!");
                }
            } while (output == null);

            return output;
        }

        private static void TerminateExecution(String message)
        {
            Console.WriteLine(message);
            Console.WriteLine(ESCAPE_DEFAULT_MESSAGE);
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void DisplayInterfacesList()
        {
            List<String> interfaces = NetworkManager.ListInterfaces();
            foreach (String elem in interfaces)
                Console.WriteLine(elem);

            Console.WriteLine();
        }

        private static void ChangeMacAddress()
        {
            NetworkManager networkManager = null;

            // Assign target
            try { networkManager = new NetworkManager(_target); }
            catch (ArgumentException ex) { TerminateExecution(ex.Message); }

            // Display info message
            String infoMessage = _reset ?
                String.Format(
                    "Adapter [{0}] MAC address will be reset to default",
                    networkManager.AdapterDescription) :
                String.Format(
                    "You're going to change adapter [{0}] MAC address.\nCurrent address: {1}, new address: {2}\n",
                    networkManager.AdapterDescription,
                    networkManager.CurrentMacAddress,
                    _newAddress.ToString());

            Console.WriteLine(infoMessage);
            if (!networkManager.IsAdapterUp)
                Console.WriteLine("WARNING: Specified interface is down, therefore changing MAC addres won't have any effect");
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();

            // Change MAC address for specified interface
            MacChanger macChanger = new MacChanger(networkManager, _newAddress);
            if (_reset)
            {
                try { macChanger.ResetMacAddress(); }
                catch (ArgumentException) { TerminateExecution("Specified adapter is still set to its default address"); }

                Console.WriteLine("MAC address reset to default");
            }
            else
            {
                macChanger.ChangeMacAddress();
                Console.WriteLine("MAC address successfully changed");
                Console.WriteLine(String.Format("Your new address: {0}", _newAddress.ToString()));
            }
        }

        static void Main(string[] args)
        {
            if (!IsAdministrator())
                TerminateExecution("Need to run as administrator");

            GetOpts(args);

            if (_list)
                DisplayInterfacesList();
            else
                ChangeMacAddress();

            Console.WriteLine(ESCAPE_DEFAULT_MESSAGE);
            Console.ReadLine();
        }
    }
}
