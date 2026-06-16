using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.interfaces;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken c = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();

            if (!trainers.Any()) return [];
            var trainersViewModel = trainers.Select(m => new TrainerViewModel()
            {
                Id = m.Id,
                Email = m.Email,
                Name = m.Name,
                Phone = m.Phone,
                Specialty = m.Spectatty.ToString()
            });
            return trainersViewModel;
        }
       
        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken c = default)
        {
            var EmailExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == model.Email, c);
            var PhoneExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == model.Phone, c);
            if (PhoneExist || EmailExist) return false;
            var trainer = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Gender = model.Gender,
                Phone = model.Phone,
                Spectatty=model.Specialty,
                DateofBirth = model.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street,
                }
            };
            _unitOfWork.GetRepository<Trainer>().Add(trainer);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }


        public async Task<TrainerViewModel?> GetTrainerDetailsByIdAsync(int TrainerId, CancellationToken c = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(TrainerId);
            if (trainer == null) return null;
            else
            return new TrainerViewModel()
            {
                Name = trainer.Name,
                Phone = trainer.Phone,
                Specialty = trainer.Spectatty.ToString(),
                Email = trainer.Email,
                DateOfBirth = trainer.DateofBirth.ToShortDateString(),
                Address = $" {trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}"
            };
             
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int TrainerId, CancellationToken c = default)
        {
         var model= await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(TrainerId);
            if (model == null) return null;
            else
                return new TrainerToUpdateViewModel()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Specialty = model.Spectatty,
                    BuildingNumber = model.Address.BuildingNumber,
                    City = model.Address.City,
                    Street = model.Address.Street
                };
            }    
   
        public async Task<bool> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken c = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(id, c);
            if (trainer == null) return false;

            var emailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Email == model.Email && m.Id != id, c);
            var phoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, c);

            if (emailExists || phoneExists) return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.City = model.City;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await _unitOfWork.SaveChangesAsync(c);
            return result > 0;
        }

        
        public async Task<bool> RemoveTrainerAsync(int id, CancellationToken c = default)
        {
            {
                var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIDAsync(id, c);
                if (trainer == null) return false;

                var hasFuturesessions = await _unitOfWork.GetRepository<Session>().AnyAsync(x => x.TrainerId == id && x.StartDate >DateTime.Now,c);
                if (hasFuturesessions) return false;

                _unitOfWork.GetRepository<Trainer>().Delete(trainer);
                var result = await _unitOfWork.SaveChangesAsync(c);
                return result > 0;

            }
        }

     
    }
}
