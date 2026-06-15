using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GymManagement.Controllers
{
    public class PlansController : Controller
    {

        private readonly IPlanService _planService;
        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }
    
        public async Task<IActionResult> Index(CancellationToken c)
        {
            var plans = await _planService.GetAllPlansAsync(c: c); //no tracking
            return View(plans);
        }
    
        //public async Task<IActionResult> Details(int id,CancellationToken c)
        //{
        //    var plan = await _planService.GetByIDAsync(id,c);
        //    if (plan is null) return RedirectToAction(nameof(Index));
        //  else
        //    return View(plan);
        //}
    }
}
