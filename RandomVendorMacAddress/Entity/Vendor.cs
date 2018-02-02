using System;

namespace RandomVendorMacAddress.Entity
{
    public class Vendor : MacAddress
    {
        private long _id = 0;
        private String _companyName = "";
        private long _n_macs = 0;

        #region Properties
        public long Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        public String CompanyName
        {
            get { return _companyName; }
            private set { _companyName = value; }
        }

        public long MacsOwned
        {
            get { return _n_macs; }
            set { _n_macs = value; }
        }
        #endregion

        #region Constructors
        public Vendor(long id, String companyName, String macIdentifier) : base()
        {
            Id = id;
            CompanyName = companyName;

            macIdentifier = FormatAddress(macIdentifier);
            Address = macIdentifier + RandomBytes(3);
        }

        public Vendor(long id, String name, String macIdentifier, String separator) : this(id, name, macIdentifier)
        {
            Separator = separator;
        }
        #endregion

        public override string ToString()
        {
            return String.Format("{0} (Company Name: {1})", Address, CompanyName);
        }
    }
}
