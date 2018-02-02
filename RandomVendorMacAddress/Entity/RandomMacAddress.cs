using System;

namespace RandomVendorMacAddress.Entity
{
    public class RandomMacAddress : MacAddress
    {
        public RandomMacAddress() : base()
        {
            Address = RandomBytes(6);
        }

        public RandomMacAddress(String separator) : this()
        {
            Separator = separator;
        }
    }
}
