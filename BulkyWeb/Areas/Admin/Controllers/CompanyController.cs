using Bulk.DataAccess.Repository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = StaticDetails.Role_Admin)]
public class CompanyController: Controller
{
       private readonly IUnitOfWork _unitOfWork;

       public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
       {
              _unitOfWork = unitOfWork;
       }
       
       // GET
       public IActionResult Index()
       {
              List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
              return this.View(objCompanyList);
       }
       
       // CREATE
       public IActionResult Upsert(int? id)
       {
              if (id == null || id == 0) // Create
              {
                     return this.View(new Company());
              }
              else // Update
              {
                     Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
                     return this.View(companyObj);
              }
       }
       
       [HttpPost]
       public IActionResult Upsert(Company companyObj)
       {
              if (this.ModelState.IsValid)
              {
                     if (companyObj.Id == 0)
                     {
                            _unitOfWork.Company.Add(companyObj);
                     }
                     else
                     {
                            _unitOfWork.Company.Update(companyObj);
                     }
                     
                     _unitOfWork.Save();
                     this.TempData["Success"] = "Company created successfully";
                     return this.RedirectToAction("Index");
              }
              else
              {
                     return this.View(companyObj);
              };
       }

       #region API Endpoints

       [HttpGet]
       public IActionResult GetAll()
       {
              List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
              return Json(new { data = objCompanyList });
       }

       [HttpDelete]
       public IActionResult Delete(int id)
       {
              var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
              if (CompanyToBeDeleted == null)
              {
                     return Json(new { success = false, message = "Error while deleting" });
              }
              
              _unitOfWork.Company.Remove(CompanyToBeDeleted);
              _unitOfWork.Save();
              return Json(new { success = true, message = "Delete Successful" });
       }
       #endregion
}