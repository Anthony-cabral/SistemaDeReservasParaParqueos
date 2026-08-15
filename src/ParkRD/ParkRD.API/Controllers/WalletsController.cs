using Microsoft.AspNetCore.Mvc;
using ParkRD.Application.Contract;
using ParkRD.Application.Dtos;

namespace ParkRD.API.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<WalletDto>> GetAll()
        {
            var result = _walletService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("user/{userId}")]
        public ActionResult<WalletDto> GetByUser(int userId)
        {
            var result = _walletService.GetByUser(userId);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("transactions")]
        public ActionResult<IEnumerable<WalletTransactionDto>> GetTransactions()
        {
            var result = _walletService.GetTransactions();

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("user/{userId}/transactions")]
        public ActionResult<IEnumerable<WalletTransactionDto>> GetTransactionsByUser(int userId)
        {
            var result = _walletService.GetTransactionsByUser(userId);

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        [Route("add-amount")]
        public IActionResult AddAmount(AddWalletAmountDto request)
        {
            var result = _walletService.AddAmount(request);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return NoContent();
        }
    }
}
