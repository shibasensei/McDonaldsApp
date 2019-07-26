using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Casestudy.Models;


namespace Casestudy.Controllers
{
    public class DataController : Controller
    {
        AppDbContext _ctx;
        public DataController(AppDbContext context)
        {
            _ctx = context;
        }
        public async Task<IActionResult> Index()
        {
            UtilityModel util = new UtilityModel(_ctx);
            string msg = "";
            var json = await getMenuItemJsonFromWebAsync();
            try
            {
                msg = (util.loadData(json)) ?  " Tables loaded" : "problem loading tables";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            ViewBag.LoadedMsg = msg;
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