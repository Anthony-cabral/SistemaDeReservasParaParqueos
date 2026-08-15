using Microsoft.AspNetCore.Mvc;
using ParkRD.Application.Contract;

namespace ParkRD.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IParkingService _parkingService;
        private readonly IVehicleService _vehicleService;
        private readonly IReservationService _reservationService;
        private readonly IWalletService _walletService;

        public ReportsController(
            IUserService userService,
            IParkingService parkingService,
            IVehicleService vehicleService,
            IReservationService reservationService,
            IWalletService walletService)
        {
            _userService = userService;
            _parkingService = parkingService;
            _vehicleService = vehicleService;
            _reservationService = reservationService;
            _walletService = walletService;
        }

        [HttpGet]
        [Route("users")]
        public IActionResult GetUsersReport()
        {
            var result = _userService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("parkings")]
        public IActionResult GetParkingsReport()
        {
            var result = _parkingService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("vehicles")]
        public IActionResult GetVehiclesReport()
        {
            var result = _vehicleService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("reservations")]
        public IActionResult GetReservationsReport()
        {
            var result = _reservationService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("wallets")]
        public IActionResult GetWalletsReport()
        {
            var result = _walletService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("wallet-transactions")]
        public IActionResult GetWalletTransactionsReport()
        {
            var result = _walletService.GetTransactions();

            return Ok(result.Data);
        }
    }
}
