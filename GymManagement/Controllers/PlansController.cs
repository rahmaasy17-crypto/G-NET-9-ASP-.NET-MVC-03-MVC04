using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GymManagement.Controllers
{
    public class PlansController : Controller
    {
     
        private readonly IGenaricReposatory<Plan> PlanRepository;
        public PlansController(IGenaricReposatory<Plan> planRepository)
        {
            PlanRepository = planRepository;
        }
        // Index Action >> GET BaseUrL/Plans/Index ==Listing ALL Plans 
        //get all plans data and send it to view
        public async Task<IActionResult> Index(CancellationToken c)
        {
            var plans = await PlanRepository.GetAllAsync(c: c); //no tracking
            return View(plans);
        }
        // Details action>> Get BaseUrL/PLans/Details/ id 
        public async Task<IActionResult> Details(int id,CancellationToken c)
        {
            var plan = await PlanRepository.GetByIDAsync(id,c);
            if (plan is null) return RedirectToAction(nameof(Index));
          else
            return View(plan);
        }
    }
}
