using ParkRD.Application.Core;
using ParkRD.Application.Dtos;

namespace ParkRD.Application.Contract
{
    public interface IParkingService
    {
        ServiceResult<IEnumerable<ParkingDto>> GetAll();

        ServiceResult<IEnumerable<ParkingDto>> GetAvailable();

        ServiceResult<ParkingDto> GetById(int id);

        ServiceResult<int> Create(CreateParkingDto request);

        ServiceResult<bool> Update(int id, UpdateParkingDto request);

        ServiceResult<bool> Delete(int id);
    }
}
