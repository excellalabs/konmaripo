using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Konmaripo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;

namespace Konmaripo.Web.Controllers
{
    public class OrgWideVisibilityController : Controller
    {
        public OrgWideVisibilityController(IOptions<OrgWideVisibilitySettings> visibilitySettings)
        {
            if (visibilitySettings == null){throw new ArgumentNullException(nameof(visibilitySettings));}
        }
        public IActionResult Index()
        {

            return View();
        }
    }
}
