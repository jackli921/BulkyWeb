﻿using Bulk.DataAccess.Data;
using Bulk.DataAccess.Repository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = StaticDetails.Role_Admin)]
public class UserController: Controller
{
       private readonly ApplicationDbContext _db;
       private readonly UserManager<IdentityUser> _userManager;

       public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
       {
              _db = db;
              _userManager = userManager;
       }
       
       // GET
       public IActionResult Index()
       {
              return View();
       }

       public IActionResult RoleManagement(string userId)
       {
              // get the roleID from userID to eventually get the role name for the user

              string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == userId)?.RoleId;
              
              // populate the roleManagementVM 
              RoleManagementVM RoleVM = new RoleManagementVM()
              {
                     ApplicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                     // use projection to retrieve and popolate new lsit
                     RoleList = _db.Roles.Select(i => new SelectListItem
                     {
                            Text = i.Name,
                            Value = i.Name
                     }),
                     CompanyList = _db.Companies.Select(i => new SelectListItem
                     {
                            Text = i.Name,
                            Value = i.Id.ToString()
                     })
              };
              RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
              return View(RoleVM);
       }
   
       [HttpPost]
       public IActionResult RoleManagement(RoleManagementVM roleMangementVM)
       {
              string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == roleMangementVM.ApplicationUser.Id)?.RoleId;
              string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
              
                     // a role was updated
                     ApplicationUser applicationUser =
                            _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleMangementVM.ApplicationUser.Id);
                     if (roleMangementVM.ApplicationUser.Role == StaticDetails.Role_Company)
                     {
                            applicationUser.CompanyId = roleMangementVM.ApplicationUser.CompanyId;
                     }
                     else
                     {
                            applicationUser.CompanyId = null;
                     }

                     _db.SaveChanges();

                     _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                     _userManager.AddToRoleAsync(applicationUser, roleMangementVM.ApplicationUser.Role).GetAwaiter()
                            .GetResult();
              
              
              return RedirectToAction("Index");
       }
       #region API Endpoints

       [HttpGet]
       public IActionResult GetAll()
       {
              List<ApplicationUser> objUserList = _db.ApplicationUsers.Include(u => u.Company).ToList();
              var userRoles = _db.UserRoles.ToList();
              var roles = _db.Roles.ToList();
              
              foreach (var user in objUserList)
              {
                     var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                     user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                     
                     if (user.Company == null)
                     {
                            user.Company = new Company()
                            {
                                   Name = ""
                            };
                     }
              }
              return Json(new { data = objUserList });
       }

       [HttpPost]
       public IActionResult LockUnlock([FromBody]string id)
       {
              var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
              if (objFromDb == null)
              {
                     return Json(new { success = false, message = "Error while Locking/Unllocking" });
              }

              if (objFromDb.LockoutEnabled != null && objFromDb.LockoutEnd > DateTime.Now)
              {
                     // user is currently locked and we need to unlock them
                     objFromDb.LockoutEnd = DateTime.Now;
              }
              else
              {
                     objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
              }

              _db.SaveChanges();
              return Json(new { success = true, message = "Edit Successful" });
       }
       #endregion
}