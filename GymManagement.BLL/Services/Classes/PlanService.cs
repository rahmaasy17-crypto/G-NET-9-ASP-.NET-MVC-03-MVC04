using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.PlanVewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService:IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork) //TO inject repo
        {
            _unitOfWork = unitOfWork;
        }

        #region Index
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken c = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(c: c);

            if (!plans.Any()) return [];
            var plansViewModel = plans.Select(p => new PlanViewModel()
            {
                Id = p.Id,
               Name=p.Name,
               Price=p.Price,
               Duration=p.DurationDays,
               Description=p.Description,
               IsActive=p.IsActive
            });
            return plansViewModel;
        }

        #endregion

        #region Details
        public async Task<PlanViewModel?> GetPlanByIdAsync(int planId, CancellationToken c = default)
        {

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId, c);
            if (plan == null) return null;
            else 
              return   new PlanViewModel() 
            {
                Name = plan.Name,
                Price = plan.Price,
                Description = plan.Description, 
                IsActive = plan.IsActive,
                Duration = plan.DurationDays,
                
            };
             
        }
        #endregion

        #region update
        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken c = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId, c);
            if (plan == null || !plan.IsActive) return null; //من الاول كدا
            else
                return new UpdatePlanViewModel()
                {
                    PlanName = plan.Name,
                    Price = plan.Price,
                    Description = plan.Description,
                    Duration = plan.DurationDays,
                };
        }

        public async Task<bool> UpdatePlanAsync(int planId, UpdatePlanViewModel model, CancellationToken c = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(planId, c);
            if (plan == null || !plan.IsActive) return null;
        
            var newplan = new UpdatePlanViewModel()
            {
                Price = plan.Price,
                Description = plan.Description,
                Duration = plan.DurationDays,
            };

            _unitOfWork.GetRepository<Member>().Update(newplan);
            var result = await _unitOfWork.SaveChangesAsync(c);
            return result > 0;
        } 
        #endregion
        public async Task<bool> ToggleActivationAsync(int planId, CancellationToken c = default)
        {    
               throw new NotImplementedException();
        }    
             
      
    }
}
