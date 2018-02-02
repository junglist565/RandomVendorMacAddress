using RandomVendorMacAddress;
using System;
using System.Data.SQLite;
using System.IO;

namespace FillDatabase
{
    class Program
    {
        readonly static string[] SEPARATOR = { "::" };

        static void Main(string[] args)
        {
            SQLiteAccess dbAccess = new SQLiteAccess("macvendors.db");
            String[] keys = { "@name", "@mac" };
            String querySql = String.Format("INSERT INTO vendors (name, mac) VALUES ({0}, {1})", keys[0], keys[1]);
            String[] vendorsFileContent = File.ReadAllLines("mac-addresses-vendors.dat");

            Console.WriteLine("Connecting...");

            dbAccess.OpenConnection();

            Console.WriteLine("Connection successfully estabilished");
            Console.WriteLine("Creating vendors table...\n");

            QueryHelper query = new QueryHelper(dbAccess.Connection);
            String createTable =
                "CREATE TABLE vendors(\n" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT,\n" +
                "name TEXT NOT NULL,\n" +
                "mac TEXT NOT NULL);";

            try
            {
                query.ExecuteNonQuery(createTable);
            }
            catch (SQLiteException)
            {
                Console.WriteLine("DB file already exists");
                Console.ReadLine();
                Environment.Exit(0);
            }

            int index = 0;
            foreach (String elem in vendorsFileContent)
            {
                String vendorName = elem.Split(SEPARATOR, StringSplitOptions.None)[0];
                String vendorAddress = elem.Split(SEPARATOR, StringSplitOptions.None)[1];
                try
                {
                    query.InsertData(querySql, keys, vendorName, vendorAddress);
                    Console.WriteLine(String.Format(
                        "{0}/{1} Inserted Vendor: {2}, MAC Address: {3}", 
                        ++index, 
                        vendorsFileContent.Length, 
                        vendorName, 
                        vendorAddress));
                }
                catch (Exception ex)
                {
                    LogManager.LogException(ex);
                    break;
                }
            }

            dbAccess.CloseConnection();

            Console.Read();
        }
    }
}
