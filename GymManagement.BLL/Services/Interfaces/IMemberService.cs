using GymManagement.BLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllMemberAsync(CancellationToken c = default);
        Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken c = default);
        Task<TrainerViewModel?> GetMemberDetailsByIdAsync(int memberId, CancellationToken c = default);
       Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken c = default);
        Task<TrainerToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken c = default);
        Task<bool> UpdateMemberDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken c = default);
        Task<bool> RemoveMemberAsync(int id,CancellationToken c = default);


    }
}