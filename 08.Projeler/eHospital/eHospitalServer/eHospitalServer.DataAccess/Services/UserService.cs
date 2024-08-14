﻿using AutoMapper;
using eHospitalServer.Business.Services;
using eHospitalServer.DataAccess.Extensions;
using eHospitalServer.Entities.DTOs;
using eHospitalServer.Entities.Enums;
using eHospitalServer.Entities.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TS.Result;

namespace eHospitalServer.DataAccess.Services;
internal sealed class UserService(
    UserManager<User> userManager,
    IMapper mapper) : IUserService
{
    public async Task<Result<string>> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken)
    {        
        if (request.Email is not null)
        {
            bool isEmailExists = await userManager.Users.AnyAsync(p => p.Email == request.Email);
            if (isEmailExists)
            {
                return Result<string>.Failure(StatusCodes.Status409Conflict, "Email already has taken");
            }
        }

        if (request.IdentityNumber != "11111111111")
        {
            bool isIdentityNumberExists = await userManager.Users.AnyAsync(p => p.IdentityNumber == request.IdentityNumber);
            if (isIdentityNumberExists)
            {
                return Result<string>.Failure(StatusCodes.Status409Conflict, "Identity number already exists");
            }
        }

        User user = mapper.Map<User>(request);

        bool isUserNameExists = await userManager.Users.AnyAsync(p => p.UserName == user.UserName);
        if (isUserNameExists)
        {
            return Result<string>.Failure(StatusCodes.Status409Conflict, "User name already has taken");
        }

        Random random = new();

        bool isEmailConfirmCodeExists = true;
        while (isEmailConfirmCodeExists)
        {
            user.EmailConfirmCode = random.Next(100000, 999999);
            if (!userManager.Users.Any(p => p.EmailConfirmCode == user.EmailConfirmCode))
            {
                isEmailConfirmCodeExists = false;
            }
        }

        user.EmailConfirmCodeSendDate = DateTime.UtcNow;

        if (request.Specialty is not null)
        {
            user.DoctorDetail = new DoctorDetail()
            {
                Specialty = (Specialty)request.Specialty,
                WorkingDays = request.WorkingDays ?? new()
            };
        }

        IdentityResult result;
        if (request.Password is not null)
        {
            result = await userManager.CreateAsync(user, request.Password);
        }
        else
        {
            result = await userManager.CreateAsync(user);
        }


        if (!result.Succeeded)
        {
            return Result<string>.Failure(500, result.Errors.Select(s => s.Description).ToList());
        }

        return Result<string>.Succeed("User create is successful");
    }

    public async Task<Result<Guid>> CreatePatientAsync(CreatePatientDto request, CancellationToken cancellationToken)
    {
        if (request.Email is not null)
        {
            bool isEmailExists = await userManager.Users.AnyAsync(p => p.Email == request.Email);
            if (isEmailExists)
            {
                return Result<Guid>.Failure(StatusCodes.Status409Conflict, "Email already has taken");
            }
        }

        if (request.IdentityNumber != "11111111111")
        {
            bool isIdentityNumberExists = await userManager.Users.AnyAsync(p => p.IdentityNumber == request.IdentityNumber);
            if (isIdentityNumberExists)
            {
                return Result<Guid>.Failure(StatusCodes.Status409Conflict, "Identity number already exists");
            }
        }

        User user = mapper.Map<User>(request);
        user.UserName = user.UserName!.Replace("ü", "u");
        user.UserType = UserType.Patient;

        int number = 0;
        while (await userManager.Users.AnyAsync(p => p.UserName == user.UserName))
        {
            number++;
            user.UserName += number;
        }

        Random random = new();

        bool isEmailConfirmCodeExists = true;
        while (isEmailConfirmCodeExists)
        {
            user.EmailConfirmCode = random.Next(100000, 999999);
            if (!userManager.Users.Any(p => p.EmailConfirmCode == user.EmailConfirmCode))
            {
                isEmailConfirmCodeExists = false;
            }
        }

        user.EmailConfirmCodeSendDate = DateTime.UtcNow;


        IdentityResult result = await userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            return Result<Guid>.Failure(500, result.Errors.Select(s => s.Description).ToList());
        }

        return Result<Guid>.Succeed(user.Id);
    }   
   
}
