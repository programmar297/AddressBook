using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AddressBook.Data;

namespace AddressBook.ViewModels
{
    /// <summary>
    /// View Model to be used for binding the View
    /// </summary>
    public class AddressViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Addr { get; set; }

        public string PhoneNo { get; set; }

        public string JobPosition { get; set; }

        public string Institute { get; set; }

        public bool IsError { get; set; }

        public string ErrorMessage { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public AddressViewModel()
        {

        }
        /// <summary>
        /// Construct view model
        /// </summary>
        /// <param name="address">Address object</param>
        public AddressViewModel(AddressRecord address)
        {
            this.Id = address.Id;
            this.Name = address.Name;
            this.Email = address.Email;
            this.Addr = address.Address;
            this.PhoneNo = address.PhoneNo;
            this.JobPosition = address.JobPosition;
            this.Institute = address.Institute;
        }

        /// <summary>
        /// Converts the ViewModel into Address Record
        /// </summary>
        /// <returns>AddressRecord</returns>
        public AddressRecord ToAddressRecord()
        {
            AddressRecord record = new AddressRecord(Id, Name, Email, Addr, PhoneNo, JobPosition, Institute);            
            return record;
        }

    }
}