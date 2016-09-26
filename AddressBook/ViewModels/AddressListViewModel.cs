using AddressBook.Data;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddressBook.ViewModels
{
    public class AddressListViewModel
    {
        public IPagedList<AddressRecord> AddressList { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string SearchValue { get; set; }

    }
}