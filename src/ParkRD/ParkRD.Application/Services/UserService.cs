using ParkRD.Application.Contract;
using ParkRD.Application.Core;
using ParkRD.Application.Dtos;
using ParkRD.Domain.Entities;
using ParkRD.Infrastructure.Interfaces;

namespace ParkRD.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public ServiceResult<IEnumerable<UserDto>> GetAll()
        {
            var users = _userRepository.GetAll().Select(user => new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NationalId = user.NationalId,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            }).ToList();

            return new ServiceResult<IEnumerable<UserDto>>
            {
                Success = true,
                Data = users
            };
        }

        public ServiceResult<UserDto> GetById(int id)
        {
            var user = _userRepository.GetById(id);

            if (user == null)
            {
                return new ServiceResult<UserDto>
                {
                    Success = false,
                    Message = "The selected user was not found."
                };
            }

            var response = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NationalId = user.NationalId,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return new ServiceResult<UserDto>
            {
                Success = true,
                Data = response
            };
        }

        public ServiceResult<int> Create(CreateUserDto request)
        {
            var validation = ValidateCreate(request);

            if (!validation.Success)
            {
                return validation;
            }

            var emailExists = _userRepository.GetAll().Any(user => user.Email == request.Email);

            if (emailExists)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "Email is already registered."
                };
            }

            var nationalIdExists = _userRepository.GetAll().Any(user => user.NationalId == request.NationalId);

            if (nationalIdExists)
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "NationalId is already registered."
                };
            }

            var user = new Users
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                NationalId = request.NationalId,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _userRepository.Create(user);
            _userRepository.SaveChanges();

            return new ServiceResult<int>
            {
                Success = true,
                Data = user.Id
            };
        }

        public ServiceResult<bool> Update(int id, UpdateUserDto request)
        {
            if (id != request.Id)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "ID in URL does not match ID in body."
                };
            }

            var validation = ValidateUpdate(request);

            if (!validation.Success)
            {
                return validation;
            }

            var existing = _userRepository.GetById(id);

            if (existing == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected user was not found."
                };
            }

            var emailExists = _userRepository.GetAll().Any(user => user.Id != id && user.Email == request.Email);

            if (emailExists)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "Email is already registered."
                };
            }

            var nationalIdExists = _userRepository.GetAll().Any(user => user.Id != id && user.NationalId == request.NationalId);

            if (nationalIdExists)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "NationalId is already registered."
                };
            }

            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.NationalId = request.NationalId;
            existing.Email = request.Email;
            existing.PhoneNumber = request.PhoneNumber;
            existing.IsActive = request.IsActive;

            _userRepository.Update(existing);
            _userRepository.SaveChanges();

            return new ServiceResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        public ServiceResult<bool> Delete(int id)
        {
            var existing = _userRepository.GetById(id);

            if (existing == null)
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "The selected user was not found."
                };
            }

            _userRepository.Delete(existing);
            _userRepository.SaveChanges();

            return new ServiceResult<bool>
            {
                Success = true,
                Data = true
            };
        }

        private ServiceResult<int> ValidateCreate(CreateUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "FirstName is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "LastName is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.NationalId))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "NationalId is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "Email is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return new ServiceResult<int>
                {
                    Success = false,
                    Message = "PhoneNumber is required."
                };
            }

            return new ServiceResult<int>
            {
                Success = true
            };
        }

        private ServiceResult<bool> ValidateUpdate(UpdateUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "FirstName is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "LastName is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.NationalId))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "NationalId is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "Email is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "PhoneNumber is required."
                };
            }

            return new ServiceResult<bool>
            {
                Success = true
            };
        }
    }
}
