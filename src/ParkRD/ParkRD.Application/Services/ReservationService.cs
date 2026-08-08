using ParkRD.Application.Contract;
using ParkRD.Application.Core;
using ParkRD.Application.Dtos;
using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IParkingRepository _parkingRepository;

        public ReservationService(IReservationRepository reservationRepository, IUserRepository userRepository, IVehicleRepository vehicleRepository, IParkingRepository parkingRepository)
        {
            _reservationRepository = reservationRepository;
            _userRepository = userRepository;
            _vehicleRepository = vehicleRepository;
            _parkingRepository = parkingRepository;
        }

        public ServiceResult<IEnumerable<ReservationDto>> GetAll()
        {
            var reservations = _reservationRepository.GetAll().Select(reservation => MapReservation(reservation)).ToList();

            return new ServiceResult<IEnumerable<ReservationDto>>
            {
                Success = true,
                Data = reservations
            };
        }

        public ServiceResult<IEnumerable<ReservationDto>> GetActive()
        {
            var reservations = _reservationRepository.GetAll()
                .Where(reservation => reservation.Status == "Reserved")
                .Select(reservation => MapReservation(reservation)).ToList();

            return new ServiceResult<IEnumerable<ReservationDto>>
            {
                Success = true,
                Data = reservations
            };
        }

        public ServiceResult<IEnumerable<ReservationDto>> GetByUser(int userId)
        {
            var user = _userRepository.GetById(userId);

            if (user == null)
            {
                return new ServiceResult<IEnumerable<ReservationDto>>
                {
                    Success = false,
                    Message = "The selected user was not found."
                };
            }

            var reservations = _reservationRepository.GetAll()
                .Where(reservation => reservation.UserId == userId)
                .Select(reservation => MapReservation(reservation))
                .ToList();

            return new ServiceResult<IEnumerable<ReservationDto>>
            {
                Success = true,
                Data = reservations
            };
        }

        public ServiceResult<ReservationDto> GetById(int id)
        {
            var reservation = _reservationRepository.GetById(id);

            if (reservation == null)
            {
                return new ServiceResult<ReservationDto>
                {
                    Success = false,
                    Message = "The selected reservation was not found."
                };
            }

            var response = MapReservation(reservation);

            return new ServiceResult<ReservationDto>
            {
                Success = true,
                Data = response
            };
        }

        public ServiceResult<int> Create(CreateReservationDto request)
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

            var vehicle = _vehicleRepository.GetById(request.VehicleId);

            if (vehicle == null)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The selected vehicle was not found."
                };
            }

            if (vehicle.UserId != request.UserId)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "This vehicle does not belong to the selected user."
                };
            }

            var parking = _parkingRepository.GetById(request.ParkingId);

            if (parking == null)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The selected parking space was not found."
                };
            }

            var reservationExists = _reservationRepository.GetAll().Any(reservation =>
                reservation.ParkingId == request.ParkingId &&
                reservation.ReservationDate.Date == request.ReservationDate.Date &&
                reservation.Status != "Cancelled" &&
                request.StartTime < reservation.EndTime &&
                request.EndTime > reservation.StartTime
            );

            if (reservationExists)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "This parking space is already reserved for that time."
                };
            }

            var totalAmount = GetTotalAmount(request.ReservationType, request.StartTime, request.EndTime, parking.HourlyRate, parking.DailyRate);

            var reservation = new Reservations
            {
                UserId = request.UserId,
                VehicleId = request.VehicleId,
                ParkingId = request.ParkingId,
                ReservationDate = request.ReservationDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ReservationType = request.ReservationType,
                Status = "Reserved",
                TotalAmount = totalAmount,
                CreatedAt = DateTime.Now
            };

            parking.Status = "Reserved";

            _reservationRepository.Create(reservation);
            _parkingRepository.Update(parking);
            _reservationRepository.SaveChanges();

            return new ServiceResult<int>
            {
                Success = true,
                Data = reservation.Id
            };
        }

        public ServiceResult<bool> Update(int id, UpdateReservationDto request)
        {
            if (id != request.Id)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected reservation does not match the information sent."
                };
            }

            var validation = ValidateUpdate(request);

            if (!validation.Success)
            {
                return validation;
            }

            var existing = _reservationRepository.GetById(id);

            if (existing == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected reservation was not found."
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

            var vehicle = _vehicleRepository.GetById(request.VehicleId);

            if (vehicle == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected vehicle was not found."
                };
            }

            if (vehicle.UserId != request.UserId)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "This vehicle does not belong to the selected user."
                };
            }

            var parking = _parkingRepository.GetById(request.ParkingId);

            if (parking == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected parking space was not found."
                };
            }

            var reservationExists = _reservationRepository.GetAll().Any(reservation =>
                reservation.Id != id &&
                reservation.ParkingId == request.ParkingId &&
                reservation.ReservationDate.Date == request.ReservationDate.Date &&
                reservation.Status != "Cancelled" &&
                request.StartTime < reservation.EndTime &&
                request.EndTime > reservation.StartTime
            );

            if (reservationExists)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "This parking space is already reserved for that time."
                };
            }

            var totalAmount = GetTotalAmount(request.ReservationType, request.StartTime, request.EndTime, parking.HourlyRate, parking.DailyRate);

            existing.UserId = request.UserId;
            existing.VehicleId = request.VehicleId;
            existing.ParkingId = request.ParkingId;
            existing.ReservationDate = request.ReservationDate;
            existing.StartTime = request.StartTime;
            existing.EndTime = request.EndTime;
            existing.ReservationType = request.ReservationType;
            existing.Status = request.Status;
            existing.TotalAmount = totalAmount;

            _reservationRepository.Update(existing);
            _reservationRepository.SaveChanges();

            return new ServiceResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        public ServiceResult<bool> Delete(int id)
        {
            return Cancel(id);
        }

        public ServiceResult<bool> Cancel(int id)
        {
            var existing = _reservationRepository.GetById(id);

            if (existing == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected reservation was not found."
                };
            }

            if (existing.Status == "Cancelled")
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "This reservation is already cancelled."
                };
            }

            existing.Status = "Cancelled";

            var parking = _parkingRepository.GetById(existing.ParkingId);

            if (parking != null)
            {
                var hasActiveReservations = _reservationRepository.GetAll().Any(reservation =>
                    reservation.Id != existing.Id &&
                    reservation.ParkingId == existing.ParkingId &&
                    reservation.Status == "Reserved"
                );

                if (!hasActiveReservations)
                {
                    parking.Status = "Available";
                    _parkingRepository.Update(parking);
                }
            }

            _reservationRepository.Update(existing);
            _reservationRepository.SaveChanges();

            return new ServiceResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        private ReservationDto MapReservation(Reservations reservation)
        {
            var user = _userRepository.GetById(reservation.UserId);
            var vehicle = _vehicleRepository.GetById(reservation.VehicleId);
            var parking = _parkingRepository.GetById(reservation.ParkingId);

            return new ReservationDto
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                UserName = user == null ? string.Empty : $"{user.FirstName} {user.LastName}",
                VehicleId = reservation.VehicleId,
                VehiclePlate = vehicle == null ? string.Empty : vehicle.Plate,
                ParkingId = reservation.ParkingId,
                ParkingCode = parking == null ? string.Empty : parking.Code,
                ReservationDate = reservation.ReservationDate,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                ReservationType = reservation.ReservationType,
                Status = reservation.Status,
                TotalAmount = reservation.TotalAmount,
                CreatedAt = reservation.CreatedAt
            };
        }

        private decimal GetTotalAmount(string reservationType, TimeSpan startTime, TimeSpan endTime, decimal hourlyRate, decimal dailyRate)
        {
            if (reservationType == "Day")
            {
                return dailyRate;
            }

            var hours = (decimal)(endTime - startTime).TotalHours;

            return hours * hourlyRate;
        }

        private ServiceResult<int> ValidateCreate(CreateReservationDto request)
        {
            if (request.UserId <= 0)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "You need to select a user."
                };
            }

            if (request.VehicleId <= 0)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "You need to select a vehicle."
                };
            }

            if (request.ParkingId <= 0)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "You need to select a parking space."
                };
            }

            if (request.ReservationDate == default)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "ReservationDate is required."
                };
            }

            if (request.EndTime <= request.StartTime)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "The end time cannot be before the start time."
                };
            }

            if (string.IsNullOrWhiteSpace(request.ReservationType))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "ReservationType is required."
                };
            }

            return new ServiceResult<int>
            {
                Success = true
            };
        }

        private ServiceResult<bool> ValidateUpdate(UpdateReservationDto request)
        {
            if (request.UserId <= 0)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "You need to select a user."
                };
            }

            if (request.VehicleId <= 0)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "You need to select a vehicle."
                };
            }

            if (request.ParkingId <= 0)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "You need to select a parking space."
                };
            }

            if (request.ReservationDate == default)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "ReservationDate is required."
                };
            }

            if (request.EndTime <= request.StartTime)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The end time cannot be before the start time."
                };
            }

            if (string.IsNullOrWhiteSpace(request.ReservationType))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "ReservationType is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "Status is required."
                };
            }

            return new ServiceResult<bool>
            {
                Success = true
            };
        }
    }
}