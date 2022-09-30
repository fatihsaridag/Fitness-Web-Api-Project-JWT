using FitnessAuthSever.Core.Dtos;
using FitnessAuthSever.Core.Entity;
using FitnessAuthSever.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessAuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineCoachController : CustomBaseController
    {
        private readonly IGenericService<OnlineCoach, OnlineCoachDto> _onlineCoachService;

        public OnlineCoachController(IGenericService<OnlineCoach, OnlineCoachDto> onlineCoachService)
        {
            _onlineCoachService = onlineCoachService;
        }

        [HttpGet]
        public async Task<IActionResult> OnlineCoachGet()
        {
            return ActionResultInstance( await _onlineCoachService.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> OnlineCoachSave(OnlineCoachDto onlineCoachDto)
        {
            return ActionResultInstance(await _onlineCoachService.AddAsync(onlineCoachDto));
        }

        [HttpPut]
        public async Task<IActionResult> OnlineCoachUpdate(OnlineCoachDto onlineCoachDto)
        {
            return ActionResultInstance(await _onlineCoachService.Update(onlineCoachDto,onlineCoachDto.Id));

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> OnlineCoachDelete(int id)
        {
            return ActionResultInstance(await _onlineCoachService.Remove(id));
        }



    }
}
