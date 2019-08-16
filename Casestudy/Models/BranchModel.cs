using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Casestudy.Models
{
    public class BranchModel
    {
        private AppDbContext _db;
        public BranchModel(AppDbContext context)
        {
            _db = context;
        }
        public bool LoadCSVFromFile(string path)
        {
            bool addWorked = false;
            try
            {
                // clear out the old rows
                _db.Branches.RemoveRange(_db.Branches);
                _db.SaveChanges();
                var csv = new List<string[]>();
                var csvFile = path + "\\exercisesStoreRaw.csv";
                var lines = System.IO.File.ReadAllLines(csvFile);
                foreach (string line in lines)
                    csv.Add(line.Split(',')); // populate store with csv
                foreach (string[] x in csv)
                {
                    Branch aBranch = new Branch();
                    aBranch.Longitude = Convert.ToDouble(x[0]);
                    aBranch.Latitude = Convert.ToDouble(x[1]);
                    aBranch.Street = x[2];
                    aBranch.City = x[3];
                    aBranch.Region = x[4];
                    _db.Branches.Add(aBranch);
                    _db.SaveChanges();
                }
                addWorked = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return addWorked;
        }

        public List<Branch> GetThreeClosestStores(float? lat, float? lng)
        {
            List<Branch> branchDetails = null;
            try
            {
                var latParam = new SqlParameter("@lat", lat);
                var lngParam = new SqlParameter("@lng", lng);
                var query = _db.Branches.FromSql("dbo.pGetThreeClosestStores @lat, @lng", latParam, lngParam);
                branchDetails = query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return branchDetails;
        }
    }
}