using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace AddressBook.Data
{
    public static class AddressData
    {
        //Get Connection string from web.config
        private static string connectionString = ConfigurationManager.ConnectionStrings["AddressBookConnectionString"].ConnectionString;

        /// <summary>
        /// Get All address records from database
        /// </summary>
        /// <returns>IList object of AddressRecords</returns>
        public static IList<AddressRecord> GetAllRecords()
        {
            DataContext dbContext = new DataContext();
            IList<AddressRecord> result = null;

            result = dbContext.GetTable<Address>().Select(a => new AddressRecord(a.id, a.Name, a.EmailAddress, a.PhysicalAddress, a.PhoneNumber, a.JobPosition, a.Institution)
            ).ToList();

            dbContext.Connection.Close();
            dbContext.Dispose();
            return result;
        }
        /// <summary>
        /// Get All address records from database
        /// </summary>
        /// <param name="search">Search term</param>
        /// <returns></returns>
        public static IList<AddressRecord> GetAllRecords(string search)
        {
            DataContext dbContext = new DataContext();
            IList<AddressRecord> result = null;
            search = search.ToLower();
            result = dbContext.GetTable<Address>()
                                   .Where(s => s.Name.ToLower().Contains(search) || s.EmailAddress.Contains(search) || s.PhysicalAddress.ToLower().Contains(search)
                                            || s.PhoneNumber.ToLower().Contains(search) || s.JobPosition.ToLower().Contains(search) || s.Institution.Contains(search))
                                   .Select(a => new AddressRecord(a.id, a.Name, a.EmailAddress, a.PhysicalAddress, a.PhoneNumber, a.JobPosition, a.Institution)
                                   ).ToList();

            dbContext.Connection.Close();
            dbContext.Dispose();
            return result;
        }
    }
}