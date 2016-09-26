using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AddressBook.Data;
using AddressBook.ViewModels;
using PagedList;

namespace AddressBook.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Index Method
        /// </summary>
        /// <param name="page">Page Number</param>
        /// <param name="size">Number of records on Page</param>
        /// <param name="search">Search term</param>
        /// <param name="msg">Message to be displayed</param>
        /// <param name="error">If page contains error</param>
        /// <returns>Index View form Home</returns>
        public ActionResult Index(int? page, int? size, string search = "", string msg = "", bool error = false)
        {
            AddressListViewModel viewModel = new AddressListViewModel();
            int cur_page = page ?? 1;
            int cur_size = size ?? 10;
            if (string.IsNullOrWhiteSpace(search))
                viewModel.AddressList = AddressData.GetAllRecords().ToPagedList(cur_page, cur_size);
            else
                viewModel.AddressList = AddressData.GetAllRecords(search).ToPagedList(cur_page, cur_size);
            viewModel.PageNumber = cur_page;
            viewModel.PageSize = cur_size;
            viewModel.SearchValue = search;
            ViewData["error"] = error;
            ViewData["msg"] = msg.Replace("\n", "<br />");
            return View(viewModel);
        }
        /// <summary>
        /// Action Method for AJAX Search
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="size">Number of records</param>
        /// <param name="search">Search term</param>
        /// <returns></returns>
        public ActionResult Search(int? page, int? size, string search = "")
        {
            int cur_page = page ?? 1;
            int cur_size = size ?? 10;
            IList<AddressRecord> addressList = null;
            if (string.IsNullOrWhiteSpace(search))
                addressList = AddressData.GetAllRecords();
            else
                addressList = AddressData.GetAllRecords(search);
            IPagedList<AddressRecord> result = addressList != null && addressList.Any() ? addressList.ToPagedList(cur_page, cur_size) : null;
            ViewData["page"] = cur_page;
            ViewData["size"] = cur_size;
            ViewData["search"] = search;
            return PartialView("_ListView", result);
        }
        /// <summary>
        /// View for adding new address
        /// </summary>
        /// <returns>Dialog view</returns>
        public ActionResult Add()
        {
            AddressViewModel viewModel = new AddressViewModel();
            return PartialView("_Dialog", viewModel);
        }
        /// <summary>
        /// View for editing an existing address
        /// </summary>
        /// <param name="id">Database record id</param>
        /// <returns>Dialog view</returns>
        public ActionResult Edit(int id)
        {
            AddressViewModel viewModel = null;
            try
            {
                AddressRecord address = new AddressRecord(id);
                viewModel = new AddressViewModel(address);
            }
            catch(Exception ex)
            {
                viewModel = new AddressViewModel();
                viewModel.IsError = true;
                viewModel.ErrorMessage = ex.Message;
            }
            return PartialView("_Dialog", viewModel);
        }
        /// <summary>
        /// Check for Duplicate record
        /// </summary>
        /// <param name="id">Address ID (DB Generated ID)</param>
        /// <param name="name">Name saved in DB</param>
        /// <param name="email">Email saved in DB</param>
        /// <returns>JSON object specifying error</returns>
        [HttpPost]
        public JsonResult CheckDuplicate(int id, string email)
        {
            bool available = false;
            string field = "";
            IList<AddressRecord> addressList = AddressData.GetAllRecords();
            bool emailavailable = false;
            if (id > 0)
            {                
                emailavailable = addressList.Where(a => a.Id != id &&  a.Email.ToLower().Equals(email.ToLower())).Any();
            }
            else
            {               
                emailavailable = addressList.Where(a => a.Email.ToLower().Equals(email.ToLower())).Any();
            }
            

            return Json(new { Success = !emailavailable }, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// Save the incoming data from Address dialog
        /// </summary>
        /// <param name="viewModel">Address View Model object</param>
        /// <returns>JSON data</returns>
        [HttpPost]
        public JsonResult Save(AddressViewModel viewModel)
        {
            bool saved = false;
            string mode;
            string message = "";
            AddressRecord record = null;            
            if (viewModel.Id == 0)
            {               
                mode = "insert";
            }
            else
            {
                mode = "edit";
            }
            try
            {
                record = viewModel.ToAddressRecord();
                record.Save();
                saved = true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { Success = saved, Address = record, Mode = mode, Message = message }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool success = false;
            string errorMessage = string.Empty;
            try
            {
                AddressRecord record = new AddressRecord(id);
                record.Delete();
                success = true;
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
            }
            return Json(new { Success = success, Error = errorMessage }, JsonRequestBehavior.DenyGet);
        }

    }
}