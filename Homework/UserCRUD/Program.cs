using System.Collections.Concurrent;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using UserCRUD;
using UserCRUD.Api.Filters;
using UserCRUD.Application.Models;
using UserCRUD.Application.Services;
using UserCRUD.Application.Validators;
using UserCRUD.Domain.Abstractions;
using UserCRUD.Domain.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();

var userStorage  = new ConcurrentDictionary<Guid, CustomUser>();
builder.Services.AddSingleton(userStorage);

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginUserRequestValidator>();
builder.Services.AddScoped<ValidationFilterAttribute<CreateUserRequest>>();
builder.Services.AddScoped<ValidationFilterAttribute<UpdateUserRequest>>();
builder.Services.AddScoped<ValidationFilterAttribute<LoginUserRequest>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapPost("/users", ([FromBody] ShowUsersRequest request) =>
{
    return request.UsersMode switch
    {
        ShowUsersMode.ShowByCreationDate => Results.Ok(userStorage.Values.Where(u =>
            u.CreatedAt >= request.From && u.CreatedAt <= request.To)),
        ShowUsersMode.ShowByLastModificationDate => Results.Ok(
            userStorage.Values.Where(u => u.UpdatedAt >= request.From && u.UpdatedAt <= request.To)),
        _ => Results.BadRequest("Wrong user mode")
    };
})
.WithTags("Common Operations")
.WithName("GetUsers")
.WithDescription("Get all users by time interval");

app.Run();