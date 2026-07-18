using ParkRD.Application.Contract;
using ParkRD.Application.Core;
using ParkRD.Application.Dtos;
using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IUserRepository _userRepository;

        public VehicleService(IVehicleRepository vehicleRepository, IUserRepository userRepository)
        {
            _vehicleRepository = vehicleRepository;
            _userRepository = userRepository;
        }

        public ServiceResult<IEnumerable<VehicleDto>> GetAll()
        {
            var vehicles = _vehicleRepository.GetAll().Select(vehicle =>
            {
                var user = _userRepository.GetById(vehicle.UserId);

                return new VehicleDto
                {
                    Id = vehicle.Id,
                    Plate = vehicle.Plate,
                    Brand = vehicle.Brand,
                    Model = vehicle.Model,
                    Color = vehicle.Color,
                    UserId = vehicle.UserId,
                    UserName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                    IsActive = vehicle.IsActive
                };
            }).ToList();

            return new ServiceResult<IEnumerable<VehicleDto>>
            {
                Success = true,
                Data = vehicles
            };
        }

        public ServiceResult<VehicleDto> GetById(int id)
        {
            var vehicle = _vehicleRepository.GetById(id);

            if (vehicle == null)
            {
                return new ServiceResult<VehicleDto>
                {
                    Success = false,
                    Message = "The selected vehicle was not found."
                };
            }

            var user = _userRepository.GetById(vehicle.UserId);

            var response = new VehicleDto
            {
                Id = vehicle.Id,
                Plate = vehicle.Plate,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Color = vehicle.Color,
                UserId = vehicle.UserId,
                UserName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                IsActive = vehicle.IsActive
            };

            return new ServiceResult<VehicleDto>
            {
                Success = true,
                Data = response
            };
        }

        public ServiceResult<int> Create(CreateVehicleDto request)
        {
            var validation = ValidateCreate(request);

            if (!validation.Success)
            {
                return validation;
            }

            var user = _userRepository.GetById(request.UserId);

            if (user == null)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The selected user was not found."
                };
            }

            var plateExists = _vehicleRepository.GetAll().Any(vehicle => vehicle.Plate == request.Plate);

            if (plateExists)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "This plate is already registered."
                };
            }

            var vehicle = new Vehicles
            {
                Plate = request.Plate,
                Brand = request.Brand,
                Model = request.Model,
                Color = request.Color,
                UserId = request.UserId,
                IsActive = true
            };

            _vehicleRepository.Create(vehicle);
            _vehicleRepository.SaveChanges();

            return new ServiceResult<int>
            {
                Success = true,
                Data = vehicle.Id
            };
        }

        public ServiceResult<bool> Update(int id, UpdateVehicleDto request)
        {
            if (id != request.Id)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected vehicle does not match the information sent."
                };
            }

            var validation = ValidateUpdate(request);

            if (!validation.Success)
            {
                return validation;
            }

            var existing = _vehicleRepository.GetById(id);

            if (existing == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected vehicle was not found."
                };
            }

            var user = _userRepository.GetById(request.UserId);

            if (user == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected user was not found."
                };
            }

            var plateExists = _vehicleRepository.GetAll().Any(vehicle => vehicle.Id != id && vehicle.Plate == request.Plate);

            if (plateExists)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "This plate is already registered."
                };
            }

            existing.Plate = request.Plate;
            existing.Brand = request.Brand;
            existing.Model = request.Model;
            existing.Color = request.Color;
            existing.UserId = request.UserId;
            existing.IsActive = request.IsActive;

            _vehicleRepository.Update(existing);
            _vehicleRepository.SaveChanges();

            return new ServiceResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        public ServiceResult<bool> Delete(int id)
        {
            var existing = _vehicleRepository.GetById(id);

            if (existing == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected vehicle was not found."
                };
            }

            _vehicleRepository.Delete(existing);
            _vehicleRepository.SaveChanges();

            return new ServiceResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        private ServiceResult<int> ValidateCreate(CreateVehicleDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Plate))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The plate is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Brand))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The brand is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Model))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The model is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Color))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The color is required."
                };
            }

            if (request.UserId <= 0)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "You need to select a user."
                };
            }

            return new ServiceResult<int>
            {
                Success = true
            };
        }

        private ServiceResult<bool> ValidateUpdate(UpdateVehicleDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Plate))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The plate is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Brand))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The brand is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Model))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The model is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Color))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The color is required."
                };
            }

            if (request.UserId <= 0)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "You need to select a user."
                };
            }

            return new ServiceResult<bool>
            {
                Success = true
            };
        }
    }
}
