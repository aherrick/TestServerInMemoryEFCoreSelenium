using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TestServerPOC.Data;

namespace TestServerInMemoryDbPOC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task Edit()
        {
            // at this point we know we already have a user added via Unit Test, just update the name
            // and save.
            var user = _context.User.FirstOrDefault();

            user.Name = "Herrick";

            _context.User.Update(user);

            await _context.SaveChangesAsync();
        }
    }
}