using ParkRD.Application.Core;
using ParkRD.Application.Dtos;

namespace ParkRD.Application.Contract
{
    public interface IReservationService
    {
        ServiceResult<IEnumerable<ReservationDto>> GetAll();

        ServiceResult<IEnumerable<ReservationDto>> GetActive();

        ServiceResult<ReservationDto> GetById(int id);

        ServiceResult<int> Create(CreateReservationDto request);

        ServiceResult<bool> Update(int id, UpdateReservationDto request);

        ServiceResult<bool> Delete(int id);
    }
}
