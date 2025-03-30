using Microsoft.AspNetCore.Mvc;

namespace GroupApi.Controllers
{
    public class ModelConttroller: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
