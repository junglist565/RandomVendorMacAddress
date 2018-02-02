using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RandomVendorMacAddress.Entity
{
    public class MacAddress
    {
        #region Constants
        public const String NULL_SEPARATOR = "";
        public const String DASH_SEPARATOR = "-";
        public const String DOUBLE_DOTS_SEPARATOR = ":";
        public const String DEFAULT_SEPARATOR = NULL_SEPARATOR;

        private const String MAC_ADDRESS_VALIDATION_REGEX =
            @"\b((?<![-:])([0-9A-Fa-f]{2}(?=([-:]))(\3[0-9A-Fa-f]{2}){5}|" + 
            @"[0-9A-Fa-f]{4}(?=([-:]))(\5[0-9A-Fa-f]{4}){2})(?![-:])|[0-9A-Fa-f]{12})\b";
        #endregion
        
        private String _address = null;
        private String _separator = null;

        #region Properties
        public String Address
        {
            get { return _address; }
            protected set
            {
                Regex regex = new Regex(MAC_ADDRESS_VALIDATION_REGEX);
                if (regex.IsMatch(value))
                    _address = value.ToUpper();
                else
                    throw new ArgumentException("Invalid MAC address format");
            }
        }

        public String Separator
        {
            get { return _separator; }
            set
            {
                _separator = value == NULL_SEPARATOR || value == DASH_SEPARATOR || value == DOUBLE_DOTS_SEPARATOR ? value : DEFAULT_SEPARATOR;
                if (_address != null)
                    _address = FormatAddress(_address);
            }
        }
        #endregion

        #region Constructors
        protected MacAddress()
        {
            Separator = DEFAULT_SEPARATOR;
        }

        public MacAddress(String address)
        {
            Address = address;
            Separator = DEFAULT_SEPARATOR;
        }

        public MacAddress(String address, String separator)
        {
            Address = address;
            Separator = separator;
        }
        #endregion

        public override string ToString()
        {
            return Address;
        }

        protected string RandomBytes(int iterations)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            List<String> output = new List<string>();

            for (int i = 0; i < iterations; i++)
            {
                String hex = String.Format("{0:X02}", random.Next(0, 255));
                output.Add(hex);
            }

            return String.Join(Separator, output);
        }

        protected String FormatAddress(String address)
        {
            String output = address;
            String currentSep = address.Contains(DASH_SEPARATOR) ? DASH_SEPARATOR :
                address.Contains(DOUBLE_DOTS_SEPARATOR) ? DOUBLE_DOTS_SEPARATOR : null;

            if (currentSep == null)
                for (int i = 2; i < address.Length; i += 3)
                    output = address.Insert(i, Separator);
            else
                output = address.Replace(currentSep, Separator);

            return output;
        }
    }
}
