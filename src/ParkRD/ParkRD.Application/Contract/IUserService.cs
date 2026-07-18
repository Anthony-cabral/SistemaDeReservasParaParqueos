using ParkRD.Application.Core;
using ParkRD.Application.Dtos;

namespace ParkRD.Application.Contract
{
    public interface IUserService
    {
        ServiceResult<IEnumerable<UserDto>> GetAll();

        ServiceResult<UserDto> GetById(int id);

        ServiceResult<int> Create(CreateUserDto request);

        ServiceResult<bool> Update(int id, UpdateUserDto request);

        ServiceResult<bool> Delete(int id);
    }
}
