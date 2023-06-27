using BusinessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using BusinessLayer.DAL;
using System.Configuration; 
namespace eSGBIZ.Controllers
{
    public class MaterialController : BaseController
    {
        public ActionResult MaterialRequisitionEntry()
        {
            Material_Entry Entry = new Material_Entry();
            ViewBag.Header = "Raw Material Requisition Entry";

          

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            Entry.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            List<MaterialMaster> materialList = new DAL_Common().GetMaterialList();
            Entry.Material_List = new SelectList(materialList, "Material_Id", "Material_Name");

            List<BrandName_Master> brandNameMasterList = new DAL_Common().GeBrandNameMaster();
            Entry.BRAND_LIST = new SelectList(brandNameMasterList, "BRAND_ID", "BRAND_NAME");

            
            return View(Entry);
        }

        [HttpPost]
        [SubmitButtonSelector(Name = "Save")]
        [ActionName("MaterialRequisitionEntry")]
        public ActionResult MaterialRequisitionEntry_Save(Material_Entry Entry)
        {
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

            if (ModelState.IsValid)
            {
                try
                {
                    ResultMessage objMst;
                    string result = new DAL_MATERIAL_REQUISITION_ENTRY().INSERT_MATERIAL_REQUISITION_ENTRY(emp.Employee_Code, Entry, out objMst);

                    if (result == "")
                    {
                        Success(string.Format("<b>MATERIAL REQUISITION inserted successfully. </b> <b style='color:red'> " + objMst.CODE + "</b>"), true);
                        return RedirectToAction("MaterialRequisitionEntry", "Material");
                    }
                    else
                    {
                        Danger(string.Format("<b>Error:</b>" + result), true);
                    }
                }
                catch (Exception ex)
                {
                    Danger(string.Format("<b>Error:</b>" + ex.Message), true);
                }
            }
            else
            {
                Danger(string.Format("<b>Error:102 :</b>" + string.Join("; ", ModelState.Values.SelectMany(z => z.Errors).Select(z => z.ErrorMessage))), true);
            }


            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            Entry.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");

            List<MaterialMaster> materialList = new DAL_Common().GetMaterialList();
            Entry.Material_List = new SelectList(materialList, "Material_Id", "Material_Name");

            List<BrandName_Master> brandNameMasterList = new DAL_Common().GeBrandNameMaster();
            Entry.BRAND_LIST = new SelectList(brandNameMasterList, "BRAND_ID", "BRAND_NAME");

            return View(Entry);
        }



         [Authorize]
        public ActionResult MaterialRequisitionList()
        {
            ViewBag.Header = "Raw Material Requisition";
            Material_Req_List Entry = new Material_Req_List();

            List<EMPLOYEE_BRACH_ACCESS> _branchList = new DAL_Common().GetEmployeeBrachAccess_List(emp.Employee_Code, "0");
            Entry.BRANCH_LIST = new SelectList(_branchList, "Branch_Code", "Branch_Name");


            return View(Entry);

        }

    
        
        public ActionResult _MaterialRequisitionList()
        {
            return PartialView("_MaterialRequisitionList");
        }


        [HttpPost]
        public ActionResult _Material_Requisition_DataList(string branch_code, DateTime fDate, DateTime tDate)
        {
            // Server Side Processing
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            int totalRow = 0;

            Material_Req_List Entry = new Material_Req_List();
            List<MATERIAL_REQ_DATA_LIST> materiallist = new List<MATERIAL_REQ_DATA_LIST>();
            try
            {
                if (string.IsNullOrEmpty(branch_code))
                {
                    branch_code = "0";
                }
                Entry.BRANCH_CODE = branch_code;
                Entry.From_DT = fDate;
                Entry.To_DT = tDate;

                materiallist = new DAL_MATERIAL_REQUISITION_ENTRY().Select_Material_Data_List(Entry);

                totalRow = materiallist.Count();

            }
            catch (Exception ex)
            {
                //logger.Error(ex, "Error : _CNs_Data_List ", ex.Message);
                Danger(string.Format("<b>Exception occured.</b>"), true);
            }

            if (!string.IsNullOrEmpty(searchValue)) // Filter Operation
            {
                materiallist = materiallist.
                    Where(x => x.REQUISITION_CODE.ToLower().Contains(searchValue.ToLower()) 
                        || x.REQUISITION_DATE.ToLower().Contains(searchValue.ToLower())
                        || x.Material_Name.ToLower().Contains(searchValue.ToLower())).ToList<MATERIAL_REQ_DATA_LIST>();




            }
            int totalRowFilter = materiallist.Count();
            // sorting
         

            // Paging
            if (length == -1)
            {
                length = totalRow;
            }
            materiallist = materiallist.Skip(start).Take(length).ToList<MATERIAL_REQ_DATA_LIST>();

            var jsonResult = Json(new { data = Entry, draw = Request["draw"], recordsTotal = totalRow, recordsFiltered = totalRowFilter }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

      

    }
}