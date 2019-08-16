using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Casestudy.Models;
using Microsoft.AspNetCore.Hosting;

namespace Casestudy.Controllers
{
    public class DataController : Controller
    {
        AppDbContext _ctx;
        IHostingEnvironment _env;
        public DataController(AppDbContext context, IHostingEnvironment env)
        {
            _ctx = context;
            _env = env;
        }
        public IActionResult LoadCsv()
        {
            BranchModel model = new BranchModel(_ctx);
            bool storesLoaded = model.LoadCSVFromFile(_env.WebRootPath);
            if (storesLoaded)
                ViewBag.LoadedMsg = "Csv Loaded Successfully";
            else
                ViewBag.LoadedMsg = "Csv NOT Loaded";
            return View("Index");
        }
        public async Task<IActionResult> Json()
        {
            UtilityModel util = new UtilityModel(_ctx);
            var json = await getMenuItemJsonFromWebAsync();
            try
            {
                ViewBag.LoadedMsg = (util.loadData(json)) ? "tables loaded" : "problem loading tables";
            }
            catch (Exception ex)
            {
                ViewBag.LoadedMsg = ex.Message;
            }
            return View("Index");
        }
        public IActionResult Index()
        {
            return View();
        }
        private async Task<String> getMenuItemJsonFromWebAsync()
        {
            string url = "https://gist.githubusercontent.com/shibasensei/fd76447b47d628d34f590ab333077917/raw/cb963b4698ac729f1e94607f22e3a40e31d6810e/data.json";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}