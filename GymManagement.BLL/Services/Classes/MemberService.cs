using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositories.classes;
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
        private readonly IUnitOfWork _unitOfWork;

        public MemberService(IUnitOfWork unitOfWork ) //TO inject repo
        {
            _unitOfWork = unitOfWork;
        }


        #region index view
        public async Task<IEnumerable<TrainerViewModel>> GetAllMemberAsync(CancellationToken c = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(c: c);

            if (!members.Any()) return [];
            var membersViewModel = members.Select(m => new TrainerViewModel()
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
            var EmailExist =await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Email == model.Email, c);
            var PhoneExist =await _unitOfWork.GetRepository<Member>().AnyAsync(x => x.Phone == model.Phone, c);
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
            _unitOfWork.GetRepository<Member>().Add(member);
            var result =await _unitOfWork.SaveChangesAsync();
            return result>0;
        }


        #endregion

        #region  Get Member Details
        public async Task<TrainerViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken c = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync(id, c);
            if (member == null) return null; //من الاول كدا
            var model = new TrainerViewModel()
            {
                Email=member.Email,
                Name = member.Name,
                Phone = member.Phone,
                DateOfBirth = member.DateofBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = $"{member.Address.BuildingNumber} - {member.Address.Street} - {member.Address.City}"
            };
            var activeMembership = await _unitOfWork.GetRepository<MemberShip>().FirstOrDefultAsync(x => x.Id == id && x.EndDate > DateTime.Now);
            if (activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIDAsync(activeMembership.PlanId, c); //relationshiopهجيب البلان اللي فال
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
          var record=await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefultAsync(x=>x.Id == memberId,c:c);
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
        public async Task<TrainerToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken c = default) 
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync(memberId, c);
            if (member == null) return null; //من الاول كدا
           else
                return new TrainerToUpdateViewModel()
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
        public async Task<bool> UpdateMemberDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken c = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIDAsync(id, c);

            if (member == null) return false;

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id,c);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id,c);

            if (emailExists || phoneExists) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.City = model.City;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.Now;

             _unitOfWork.GetRepository<Member>().Update(member);
            var result =await _unitOfWork.SaveChangesAsync(c);
            return result > 0;
        }

        #endregion

        #region Delete
        public async Task<bool> RemoveMemberAsync(int id, CancellationToken c = default) 
        {
        var member=await _unitOfWork.GetRepository<Member>().GetByIDAsync(id,c);
            if (member == null) return false;

            var hasFutureBooking = await _unitOfWork.GetRepository<Booking>().AnyAsync(x => x.MemberId == id && x.Session.StartDate > DateTime.Now,c);
      if (hasFutureBooking) return false;

   _unitOfWork.GetRepository<Member>().Delete(member);
            var result =await _unitOfWork.SaveChangesAsync(c);
            return result > 0;
        
        }
        #endregion

    }
}
