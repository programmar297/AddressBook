using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AddressBook.Data
{
    /// <summary>
    /// Address record object used to process address information.
    /// </summary>
    public class AddressRecord
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["AddressBookConnectionString"].ConnectionString;
        private int _id = 0;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _address = null;
        private string _phone = null;
        private string _jobpos = null;
        private string _institue = null;
        /// <summary>
        /// Default constructor
        /// </summary>
        public AddressRecord()
        {

        }
        /// <summary>
        /// Address Record constructor to pre-populate the address fields. Use to populate single address object
        /// </summary>
        /// <param name="id">address id</param>
        public AddressRecord(int id)
        {
            GetAddress(id);
        }
        /// <summary>
        /// Address Record constructor to populate address data from a collection of address records
        /// </summary>
        /// <param name="id">address id</param>
        /// <param name="name">name of address holder</param>
        /// <param name="email">email of address holder</param>
        /// <param name="address">physical address</param>
        /// <param name="phone">phone number</param>
        /// <param name="jobposition">job description</param>
        /// <param name="institute">institution</param>
        public AddressRecord(int id, string name, string email, string address, string phone, string jobposition, string institute)
        {
            this._id = id;
            this._name = name;
            this._email = email;
            this._address = address;
            this._phone = phone;
            this._jobpos = jobposition;
            this._institue = institute;
        }
        /// <summary>
        /// Address id: Auto Generated id associated with the record
        /// </summary>
        public int Id
        {
            get { return _id;  }
        }
        /// <summary>
        /// Name of Address holder (required)
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Email Address of Address holder (required)
        /// </summary>
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }
        /// <summary>
        /// Physical address (optional)
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        /// <summary>
        /// Phone Number (optional)
        /// </summary>
        public string PhoneNo
        {
            get { return _phone; }
            set { _phone = value; }
        }
        /// <summary>
        /// Job Position of address holder (optional)
        /// </summary>
        public string JobPosition
        {
            get { return _jobpos; }
            set { _jobpos = value; }
        }
        /// <summary>
        /// Institution of Address holder (optional)
        /// </summary>
        public string Institute
        {
            get { return _institue; }
            set { _institue = value; }
        }
        /// <summary>
        /// Get Address record from database
        /// </summary>
        /// <param name="id">record id</param>
        private void GetAddress(int id)
        {
            DataContext dbContext = new DataContext(connectionString);
            Address address = dbContext.GetTable<Address>().SingleOrDefault(a => a.id == id);
            if(address != null)
            {
                this._id = address.id;
                this._name = address.Name;
                this._email = address.EmailAddress;
                this._address = address.PhysicalAddress;
                this._phone = address.PhoneNumber;
                this._jobpos = address.JobPosition;
                this._institue = address.Institution;
            }
            else
            {
                throw new Exception("Address (id: " + id + " was not found in the database)");
            }
            dbContext.Connection.Close();
            dbContext.Dispose();
        }
        /// <summary>
        /// Saves the Address object in database
        /// </summary>
        public void Save()
        {
            DataContext dbContext = new DataContext(connectionString);
            try
            {
                Address address = null;
                if (this._id > 0)
                    address = dbContext.GetTable<Address>().SingleOrDefault(a => a.id == this._id);
                else
                    address = new Address();

                address.Name = this._name;
                address.EmailAddress = this._email;
                address.PhysicalAddress = this._address;
                address.PhoneNumber = this._phone;
                address.JobPosition = this._jobpos;
                address.Institution = this._institue;
                if (this._id == 0)
                {
                    dbContext.GetTable<Address>().InsertOnSubmit(address);
                }
                dbContext.SubmitChanges();
                this._id = address.id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving address (id: " + this._id + ")", ex);
            }
            finally
            {
                dbContext.Connection.Close();
                dbContext.Dispose();
            }
        }
        /// <summary>
        /// Deletes the address record
        /// </summary>
        public void Delete()
        {
            DataContext dbContext = new DataContext(connectionString);
            try
            {
                Address address = dbContext.GetTable<Address>().SingleOrDefault(a => a.id == this._id);

                if (address == null)
                {
                    throw new Exception("Cannot delete address. Address not found");
                }
                else
                {
                    dbContext.GetTable<Address>().DeleteOnSubmit(address);
                    dbContext.SubmitChanges();
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting address (id: " + this._id + ")", ex);
            }
            finally
            {
                dbContext.Connection.Close();
                dbContext.Dispose();
            }
        }
    }
}