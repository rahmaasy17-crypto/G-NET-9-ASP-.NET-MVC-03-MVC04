using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositories.interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    //talk with repo to get data from database
    public class MemberService : IMemberService
    {
        private readonly IGenaricReposatory<Member> _MemberReposatory;

        public MemberService(IGenaricReposatory<Member> MemberReposatory) //TO inject repo
        {
            _MemberReposatory = MemberReposatory;
        }


        #region index view
        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken c = default)
        {
            var members = await _MemberReposatory.GetAllAsync(c: c);

            if (!members.Any()) return [];
            var membersViewModel = members.Select(m => new MemberViewModel()
            {
                Email = m.Email,
                Gender = m.Gender.ToString(),
                Id = m.Id,
                Name = m.Name,
                Phone = m.Phone
            });
            return membersViewModel;
        }
        #endregion

        #region create member
        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken c = default)
        {
            var EmailExist =await _MemberReposatory.AnyAsync(x=>x.Email==model.Email, c);
            var PhoneExist =await _MemberReposatory.AnyAsync(x => x.Phone == model.Phone, c);
            if (PhoneExist || EmailExist) return false;
            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Gender = model.Gender,
                Phone = model.Phone,
                DateofBirth = model.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                Address=new Address() { 
                BuildingNumber=model.BuildingNumber,
                City=model.City,
                Street=model.Street,
                },
                HealthRecord=new HealthRecord()
                {
                    BLoodType=model.HealthRecordViewModel.BloodType,
                    Weight=model.HealthRecordViewModel.Weight,
                    Height=model.HealthRecordViewModel.Height,  
                    Note=model.HealthRecordViewModel.Note

                }

            };

      var result=await   _MemberReposatory.AddAsync(member);
            return result>0? true: false;
        }
        #endregion
    }
}
