using Microsoft.AspNetCore.Mvc;
using Api.DTOs;
using Api.Models;
using Api.Services;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ReportsService _reportsService;

    
}