using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        #region Index
        //Index() >list all members 
        //GET baseurl/Members/Index 

       public async Task<IActionResult> Index(CancellationToken c ) 
        { 
            var members= await _memberService.GetAllMemberAsync(c);
            return  View(members);   
        }

        #endregion

        #region MemberDetails
        //MemberDetails(int id)  >for 1 member
        //GET baseurl/Members/MemberDetails/{id} 
        #endregion

        #region HealthRecordDetails
        //HealthRecordDetails(int id)   >for HealthRecord Detail for 1 member
        //GET baseurl/Members/HealthRecordDetails/{id} 
        #endregion

        #region Createing Member > 2 steps
        //Create() >show empty form 
        //GET baseurl/Members/Create

        [HttpGet]
        public IActionResult Create() => View();
        //CreateMember() >subbmit form 
        //post baseurl/Members/Create{Members} 
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model,CancellationToken c)
        {
            //before talking with service [imp even if i di clint side validation because frontand can do inspect and change clint side validation and send request with invalid data]
            if (!ModelState.IsValid) return View(nameof(Create),model); //ارجع لنفس الفورم ومعاك البيانات والأخطاء]
           var result=await   _memberService.CreateMemberAsync(model,c);
            if (result)
                TempData["successMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Create Member";
            return RedirectToAction(nameof(Index));//have 2 div depend on result[create or not]
        }
     
        #endregion

        #region Editing Member > 2 steps

        //MemberEdit(int id >disply edit form (have data)
        //GET baseurl/Members/MemberEdit/{id}

        //MemberEdit(int id >subbmit form 
        //post baseurl/Members/MemberEdit{Members} 
        #endregion


        #region DeletingMember > 2 steps
        //Delete(int id) >show form 
        //GET baseurl/Members/Delete/{id} 

        //DeleteConfirmed(int id) >subbmit form 
        //post baseurl/Members/DeleteConfirmed/{id} 
        #endregion

    }
}
