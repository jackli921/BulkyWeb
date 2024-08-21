﻿using Bulk.DataAccess.Data;
using Bulk.DataAccess.Repository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = StaticDetails.Role_Admin)]
public class UserController: Controller
{
       private readonly ApplicationDbContext _db;

       public UserController(ApplicationDbContext db)
       {
              _db = db;
       }
       
       // GET
       public IActionResult Index()
       {
              return View();
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

       [HttpDelete]
       public IActionResult Delete(int id)
       {
              return Json(new { success = true, message = "Delete Successful" });
       }
       #endregion
}