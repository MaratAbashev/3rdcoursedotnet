using UserCRUD;
using UserCRUD.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var userStorage  = new Dictionary<Guid, CustomUser>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/user/create", (UserRequest request) =>
{
    var newUser = new CustomUser
    {
        Id = Guid.NewGuid(),
        Email = request.Email,
        PasswordHash = request.Password.GetHashCode()
    };
    while (!userStorage.TryAdd(newUser.Id, newUser))
    {
        var existingUser = userStorage[newUser.Id];
        if (existingUser.Email == request.Email)
            return Results.BadRequest("User already exists");
        newUser.Id = Guid.NewGuid();
    }
    return Results.Ok(newUser);
});

app.MapDelete("/user/{id:Guid}/delete", (Guid id) => 
    userStorage.Remove(id, out var user) 
        ? Results.Ok((object?)user) 
        : Results.NotFound());

app.MapPut("/user/{id:guid}/update", (Guid id, UserRequest request) =>
{
    if (!userStorage.TryGetValue(id, out var user))
        return Results.NotFound("User not found");
    user.Email = request.Email;
    user.PasswordHash = request.Password.GetHashCode();
    return Results.Ok(user);
});

app.MapPost("/user/auth", (UserRequest request) =>
{
    var existingUser = userStorage.Values.FirstOrDefault(u => u.Email == request.Email);
    if (existingUser == null)
        return Results.BadRequest("User email is invalid");
    return existingUser.PasswordHash != request.Password.GetHashCode() 
        ? Results.BadRequest("Password does not match") 
        : Results.Ok(userStorage[existingUser.Id]);
});

app.MapGet("/users", () => Results.Ok(userStorage.Values.OrderBy(u => u.Email)));

app.Run();