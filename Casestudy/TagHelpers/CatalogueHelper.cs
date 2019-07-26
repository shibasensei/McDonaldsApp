using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Http;
using System.Text;
using Casestudy.Models;
using Newtonsoft.Json;
using Casestudy.Utils;

namespace Casestudy.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("product", Attributes = BrandIdAttribute)]
    public class CatalogueHelper : TagHelper
    {

        private const string BrandIdAttribute = "brand";
        [HtmlAttributeName(BrandIdAttribute)]
        public string BrandId { get; set; }        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public CatalogueHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (_session.Get<ProductModel[]>("menu") != null && Convert.ToInt32(BrandId) > 0)
            {
                var innerHtml = new StringBuilder();
                ProductViewModel[] menu = _session.Get<ProductViewModel[]>("menu");
                innerHtml.Append("<h5>Items</h5>");
                innerHtml.Append("<div class=\"row w-100 m-1\" style=\"overflow-y:scroll;height:60vh;\">");
                foreach (ProductViewModel item in menu)
                {
                    if (item.BrandId == Convert.ToInt32(BrandId))
                    {
                        // remove apostrophe from JSON
                        item.Description = item.Description.Contains("'") ? item.Description.Replace("'", "") : item.Description;
                        var itemJson = JsonConvert.SerializeObject(item);
                        var btn = "catbtn" + item.Id;
                        innerHtml.Append("<div class=\"col-sm-4 col-xs-12 text-center\" style =\"border:solid;\">");
                        innerHtml.Append("<img src =\"/images/phone.png\" /><br />");
                        if (item.Description.Length > 25)
                        {
                            innerHtml.Append("<span class=\"m-0\" style=\"font-size:large;font-weight:bold;\">" +
                           item.Description.Substring(0, 25) + "...</span>");
                        }
                        else
                        {
                            innerHtml.Append("<span class=\"m-0\" style=\"font-size:large;font-weight:bold;\">" +
                           item.Description + "...</span>");
                        }
                        innerHtml.Append("<p><span style=\"font-size:medium\">More info. in details</span >");
                        innerHtml.Append("<p><a id=\"" + btn + "\" href=\"#details_popup\" data-toggle=\"modal\"");
                        innerHtml.Append(" class=\"btn btn-outline-dark pt-2 pb-2\" data-id=" + item.Id);
                        innerHtml.Append(" data-details='" + itemJson + "'>Details</a></div>");
                    }
                }
                output.Content.SetHtmlContent(innerHtml.ToString());
            }
        }
    }
}
