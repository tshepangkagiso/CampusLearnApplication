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

global using CampusLearn.UserProfileManagement.API.Database.Module_Data_Seeder;
global using CampusLearn.UserProfileManagement.API.Controllers.Subscription.DTOs;

global using CampusLearn.UserProfileManagement.API.Controllers.Profile.DTOs;
global using CampusLearn.UserProfileManagement.API.Services;
global using Minio;
global using Minio.DataModel.Args;
global using Minio.DataModel.Result;
