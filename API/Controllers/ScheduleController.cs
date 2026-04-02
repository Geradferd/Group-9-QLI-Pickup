using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Models;
using Api.Services;

namespace Api.Controllers;

// This is the API controller that handles registration and login requests
// [Route("api/[controller]")] means all endpoints start with /api/auth
// So register is POST /api/auth/register and login is POST /api/auth/login
[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ScheduleService _scheduleService;

    
}