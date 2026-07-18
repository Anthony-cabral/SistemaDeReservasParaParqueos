using ParkRD.Application.Contract;
using ParkRD.Application.Core;
using ParkRD.Application.Dtos;
using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Application.Services
{
    public class ParkingService : IParkingService
    {
        private readonly IParkingRepository _parkingRepository;

        public ParkingService(IParkingRepository parkingRepository)
        {
            _parkingRepository = parkingRepository;
        }

        public ServiceResult<IEnumerable<ParkingDto>> GetAll()
        {
            var parkings = _parkingRepository.GetAll().Select(parking => new ParkingDto
            {
                Id = parking.Id,
                Code = parking.Code,
                Status = parking.Status,
                HourlyRate = parking.HourlyRate,
                DailyRate = parking.DailyRate,
                IsActive = parking.IsActive
            }).ToList();

            return new ServiceResult<IEnumerable<ParkingDto>>
            {
                Success = true,
                Data = parkings
            };
        }

        public ServiceResult<IEnumerable<ParkingDto>> GetAvailable()
        {
            var parkings = _parkingRepository.GetAll()
                .Where(parking => parking.Status == "Available" && parking.IsActive)
                .Select(parking => new ParkingDto
                {
                    Id = parking.Id,
                    Code = parking.Code,
                    Status = parking.Status,
                    HourlyRate = parking.HourlyRate,
                    DailyRate = parking.DailyRate,
                    IsActive = parking.IsActive
                }).ToList();

            return new ServiceResult<IEnumerable<ParkingDto>>
            {
                Success = true,
                Data = parkings
            };
        }

        public ServiceResult<ParkingDto> GetById(int id)
        {
            var parking = _parkingRepository.GetById(id);

            if (parking == null)
            {
                return new ServiceResult<ParkingDto>
                {
                    Success = false,
                    Message = "The selected parking space was not found."
                };
            }

            var response = new ParkingDto
            {
                Id = parking.Id,
                Code = parking.Code,
                Status = parking.Status,
                HourlyRate = parking.HourlyRate,
                DailyRate = parking.DailyRate,
                IsActive = parking.IsActive
            };

            return new ServiceResult<ParkingDto>
            {
                Success = true,
                Data = response
            };
        }

        public ServiceResult<int> Create(CreateParkingDto request)
        {
            var validation = ValidateCreate(request);

            if (!validation.Success)
            {
                return validation;
            }

            var codeExists = _parkingRepository.GetAll().Any(parking => parking.Code == request.Code);

            if (codeExists)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "This parking code is already registered."
                };
            }

            var parking = new Parkings
            {
                Code = request.Code,
                Status = "Available",
                HourlyRate = request.HourlyRate,
                DailyRate = request.DailyRate,
                IsActive = true
            };

            _parkingRepository.Create(parking);
            _parkingRepository.SaveChanges();

            return new ServiceResult<int>
            {
                Success = true,
                Data = parking.Id
            };
        }

        public ServiceResult<bool> Update(int id, UpdateParkingDto request)
        {
            if (id != request.Id)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected parking space does not match the information sent."
                };
            }

            var validation = ValidateUpdate(request);

            if (!validation.Success)
            {
                return validation;
            }

            var existing = _parkingRepository.GetById(id);

            if (existing == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected parking space was not found."
                };
            }

            var codeExists = _parkingRepository.GetAll().Any(parking => parking.Id != id && parking.Code == request.Code);

            if (codeExists)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "This parking code is already registered."
                };
            }

            existing.Code = request.Code;
            existing.Status = request.Status;
            existing.HourlyRate = request.HourlyRate;
            existing.DailyRate = request.DailyRate;
            existing.IsActive = request.IsActive;

            _parkingRepository.Update(existing);
            _parkingRepository.SaveChanges();

            return new ServiceResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        public ServiceResult<bool> Delete(int id)
        {
            var existing = _parkingRepository.GetById(id);

            if (existing == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected parking space was not found."
                };
            }

            _parkingRepository.Delete(existing);
            _parkingRepository.SaveChanges();

            return new ServiceResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        private ServiceResult<int> ValidateCreate(CreateParkingDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The parking code is required."
                };
            }

            if (request.HourlyRate <= 0)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The hourly rate must be greater than zero."
                };
            }

            if (request.DailyRate <= 0)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The daily rate must be greater than zero."
                };
            }

            return new ServiceResult<int>
            {
                Success = true
            };
        }

        private ServiceResult<bool> ValidateUpdate(UpdateParkingDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The parking code is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The parking status is required."
                };
            }

            if (request.HourlyRate <= 0)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The hourly rate must be greater than zero."
                };
            }

            if (request.DailyRate <= 0)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The daily rate must be greater than zero."
                };
            }

            return new ServiceResult<bool>
            {
                Success = true
            };
        }
    }
}
