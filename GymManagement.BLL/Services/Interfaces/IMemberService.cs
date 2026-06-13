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
         Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken c=default);
   Task<bool> CreateMemberAsync(CreateMemberViewModel member,CancellationToken c=default);
    }
}
