using Microsoft.AspNetCore.Mvc;

namespace DummyProject.Broken
{
    public class InvalidDependecy : Controller
    {
        public class InvalidDependency
        {
            public void Test()
            {
                MissingClass obj = new MissingClass();
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
