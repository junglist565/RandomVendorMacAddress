using RandomVendorMacAddress.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace RandomVendorMacAddress.Manager
{
    public sealed class VendorDAO
    {
        #region Queries
        private const String SELECT_MAX_ID = "SELECT MAX(id) AS max FROM vendors";
        private const String SELECT_RANDOM_VENDOR = "SELECT * FROM vendors WHERE vendors.id = @id";
        private const String SELECT_VENDORS_SEARCH_QUERY =
            "SELECT *, COUNT(vendors.mac) AS n_macs\n" +
            "FROM vendors\n" +
            "WHERE vendors.name LIKE \"%{0}%\"\n" +
            "GROUP BY vendors.name\n" +
            "ORDER BY n_macs DESC";
        private const String SELECT_VENDORS_BY_NAME = "SELECT * FROM vendors WHERE vendors.name = @name";
        #endregion

        private SQLiteConnection _con = null;

        public VendorDAO(SQLiteConnection con)
        {
            _con = con;
        }

        public long MaxVendorID()
        {
            QueryHelper query = new QueryHelper(_con);
            DataTable maxIdDataTable = query.DoSimpleSelect(SELECT_MAX_ID, "max-id");
            return (long)maxIdDataTable.Rows[0]["max"];
        }
        
        public Vendor RandomVendorByName(String vendorCompanyName)
        {
            String[] keys = { "@name" };
            QueryHelper query = new QueryHelper(_con);
            DataTable resultTable = query.DoSimpleSelectWithParams(
                SELECT_VENDORS_BY_NAME,
                "vendors-list-by-name",
                keys,
                vendorCompanyName);

            if (resultTable.Rows.Count == 0)
                throw new ArgumentException("Company name not found");

            List<Vendor> resultList = new List<Vendor>();

            foreach (DataRow row in resultTable.Rows)
                resultList.Add(new Vendor((long)row["id"], row["name"].ToString(), row["mac"].ToString()));

            int listCount = resultList.Count;
            Random random = new Random((int)DateTime.Now.Ticks);

            return listCount > 1 ? resultList[random.Next(listCount - 1)] : resultList[0];
        }

        public Vendor RandomVendor()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            long randomVendorId = random.Next(1, (int)MaxVendorID());
            String[] keys = { "@id" };

            QueryHelper query = new QueryHelper(_con);
            DataTable randomVendorDataTable = query.DoSimpleSelectWithParams(
                SELECT_RANDOM_VENDOR,
                "random-vendor",
                keys,
                randomVendorId);
            DataRow record = randomVendorDataTable.Rows[0];

            return new Vendor((long)record["id"], record["name"].ToString(), record["mac"].ToString());
        }

        public Dictionary<long, Vendor> SearchQueryResultList(String searchQuery)
        {
            if (searchQuery.Length < 4)
                throw new ArgumentException("Specify at least 4 characters for your searching query");

            QueryHelper query = new QueryHelper(_con);
            DataTable vendorsResult = query.DoSimpleSelect(
                String.Format(SELECT_VENDORS_SEARCH_QUERY, searchQuery),
                "vendors-search-query");

            Dictionary<long, Vendor> output = new Dictionary<long, Vendor>();
            foreach (DataRow elem in vendorsResult.Rows)
            {
                Vendor vendor = new Vendor((long)elem["id"], elem["name"].ToString(), elem["mac"].ToString());
                vendor.MacsOwned = (long)elem["n_macs"];
                output.Add(vendor.Id, vendor);
            }

            return output;
        }
    }
}
