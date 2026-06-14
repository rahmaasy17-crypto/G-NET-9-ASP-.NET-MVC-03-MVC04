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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymManagement.BLL.Services.Classes
{
    //talk with repo to get data from database
    public class MemberService : IMemberService
    {
        private readonly IGenaricReposatory<Member> _memberReposatory;
        private readonly IGenaricReposatory<MemberShip> _memberShipReposatory;
        private readonly IGenaricReposatory<Plan>        _planReposatory     ;
        private readonly IGenaricReposatory<HealthRecord> _healthRecordReposatory;

        public MemberService(IGenaricReposatory<Member> memberReposatory,IGenaricReposatory<MemberShip> memberShipReposatory,IGenaricReposatory<Plan> planReposatory, IGenaricReposatory<HealthRecord> healthRecordReposatory) //TO inject repo
        {
            _memberReposatory = memberReposatory;
            _planReposatory = planReposatory;
            _healthRecordReposatory = healthRecordReposatory;
            _memberShipReposatory = memberShipReposatory;
        }


        #region index view
        public async Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken c = default)
        {
            var members = await _memberReposatory.GetAllAsync(c: c);

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
            var EmailExist =await _memberReposatory.AnyAsync(x=>x.Email==model.Email, c);
            var PhoneExist =await _memberReposatory.AnyAsync(x => x.Phone == model.Phone, c);
            if (PhoneExist || EmailExist) return false;
            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Gender = model.Gender,
                Phone = model.Phone,
                DateofBirth = model.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                Address=new Address() { 
                BuildingNumber=model.BuildingNumber,//propertiesعشان الفيو هو اللي فيه الداتا ك
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

      var result=await   _memberReposatory.AddAsync(member);
            return result>0;
        }


        #endregion
        #region  Get Member Details
        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken c = default)
        {
            var member = await _memberReposatory.GetByIDAsync(id, c);
            if (member == null) return null; //من الاول كدا
            var model = new MemberViewModel()
            {
                Email=member.Email,
                Name = member.Name,
                Phone = member.Phone,
                DateOfBirth = member.DateofBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}"
            };
            var activeMembership = await _memberShipReposatory.FirstOrDefultAsync(x => x.Id == id && x.EndDate > DateTime.Now);
            if (activeMembership is not null)
            {
                var activePlan = await _planReposatory.GetByIDAsync(activeMembership.PlanId, c); //relationshiopهجيب البلان اللي فال
                model.PlanName = activePlan.Name;
               model.MemberShipStartDate= activeMembership.CreatedAt.ToString();
                model.MemberShipEndDate = activeMembership.EndDate.ToString();
            }
            return model;   
                }



        #endregion
        #region Health Record Details
        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken c = default)
        {
          var record=await _healthRecordReposatory.FirstOrDefultAsync(x=>x.Id == memberId,c:c);
            if (record is null) return null;
            else
            {
                return new HealthRecordViewModel() { 
                    Weight= record.Weight,
                    Height= record.Height,
                    BloodType=record.BLoodType,
                    Note= record.Note,
                };
            }
        }


        #endregion
        #region Update
        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken c = default) 
        {
            var member = await _memberReposatory.GetByIDAsync(memberId, c);
            if (member == null) return null; //من الاول كدا
           else
                return new MemberToUpdateViewModel()
            {
                Email = member.Email,
                Name = member.Name,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
               City = member.Address.City,  
               Photo=member.Photo,
               Street=member.Address.Street,
            };
        }
        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken c = default)
        {
            var member = await _memberReposatory.GetByIDAsync(id, c);

            if (member == null) return false;

            var emailExists = await _memberReposatory.AnyAsync(m => m.Email == model.Email && m.Id != id,c);
            var phoneExists = await _memberReposatory.AnyAsync(m => m.Phone == model.Phone && m.Id != id,c);

            if (emailExists || phoneExists) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.City = model.City;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.Now;

            var result = await _memberReposatory.UpdateAsync(member, c);

            return result > 0;
        }

        #endregion
    }
}
