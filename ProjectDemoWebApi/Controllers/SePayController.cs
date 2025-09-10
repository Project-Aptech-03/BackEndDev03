using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.Services;
using ProjectDemoWebApi.Services.Interface;
using System.Security.Claims;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SepayController : Controller
    {
        private readonly ISePayService _sePayService;

        public SepayController(ISePayService sePayService)
        {
            _sePayService = sePayService;
        }

        public async Task<IActionResult> GetUserId()
        {
            var result = await _sePayService.GetTransactionsAsync();
            return result;
        }
    }
}
