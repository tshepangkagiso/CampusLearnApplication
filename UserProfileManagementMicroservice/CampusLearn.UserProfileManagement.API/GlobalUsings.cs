global using CampusLearn.Library.UserManagementModels;
global using Microsoft.EntityFrameworkCore;
global using CampusLearn.UserProfileManagement.API.Database;
global using Serilog;

global using Microsoft.AspNetCore.Mvc;

global using CampusLearn.UserProfileManagement.API.Controllers.Authentication.Services.Jwt;
global using CampusLearn.UserProfileManagement.API.Controllers.Authentication.Services.Password;
global using CampusLearn.UserProfileManagement.API.Controllers.Authentication.DTOs;

global using System.Security.Claims;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using System.Text;

global using Microsoft.AspNetCore.Authorization;