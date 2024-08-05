﻿using Bulk.DataAccess.Data;
using Bulky.Models;
using BulkyWeb.Models;
using System.Linq.Expressions;

namespace Bulk.DataAccess.Repository;

public class CompanyRepository: Repository<Company>, ICompanyRepository
{
    private ApplicationDbContext _db;
    public CompanyRepository(ApplicationDbContext db) : base(db) => _db = db;

    public void Update(Company obj) => _db.Companies.Update(obj);
}