using ParkRD.Application.Core;
using ParkRD.Application.Dtos;

namespace ParkRD.Application.Contract
{
    public interface IVehicleService
    {
        ServiceResult<IEnumerable<VehicleDto>> GetAll();

        ServiceResult<VehicleDto> GetById(int id);

        ServiceResult<int> Create(CreateVehicleDto request);

        ServiceResult<bool> Update(int id, UpdateVehicleDto request);

        ServiceResult<bool> Delete(int id);
    }
}
