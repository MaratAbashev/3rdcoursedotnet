using System.Collections.Concurrent;
using UserCRUD.Domain.Abstractions;
using UserCRUD.Domain.Models;
using UserCRUD.Domain.Utils;

namespace UserCRUD.Application.Services;

public class UserService(ConcurrentDictionary<Guid, CustomUser> userStorage) : IUserService
{
    public Task<Result<CustomUser>> CreateUserAsync(string email, string password)
    {
        try
        {
            var newUser = new CustomUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = password.GetHashCode(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            while (!userStorage.TryAdd(newUser.Id, newUser))
            {
                var existingUser = userStorage[newUser.Id];
                if (existingUser.Email == email)
                    return Task.FromResult(Result<CustomUser>.Failure("Email already exists", ErrorType.BadRequest));
                newUser.Id = Guid.NewGuid();
            }

            return Task.FromResult(Result<CustomUser>.Success(newUser));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CustomUser>.Failure(ex.Message));
        }
    }

    public Task<Result<CustomUser>> UpdateUserAsync(Guid userId, string newEmail, string newPassword)
    {
        try
        {
            if (!userStorage.TryGetValue(userId, out var user))
                return Task.FromResult(Result<CustomUser>.Failure("User not found", ErrorType.NotFound));
            user.Email = newEmail;
            user.PasswordHash = newPassword.GetHashCode();
            user.UpdatedAt = DateTime.UtcNow;
            return Task.FromResult(Result<CustomUser>.Success(user));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CustomUser>.Failure(ex.Message));
        }
    }

    public Task<Result> DeleteUserAsync(Guid userId)
    {
        try
        {
            return userStorage.Remove(userId, out _) 
                ? Task.FromResult(Result.Success()) 
                : Task.FromResult(Result.Failure("User not found", ErrorType.NotFound));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Failure(ex.Message));
        }
    }

    public Task<Result<CustomUser>> GetUserAsync(Guid userId)
    {
        try
        {
            return userStorage.TryGetValue(userId, out var user) ?
                Task.FromResult(Result<CustomUser>.Success(user)) :
                Task.FromResult(Result<CustomUser>.Failure("User not found", ErrorType.NotFound));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<CustomUser>.Failure(ex.Message));
        }
    }

    public Task<Result> LoginUserAsync(string email, string password)
    {
        try
        {
            var user = userStorage.Values.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return Task.FromResult(Result.Failure("Email or password is invalid", ErrorType.BadRequest));
            return password.GetHashCode() == user.PasswordHash 
                ? Task.FromResult(Result.Success()) 
                : Task.FromResult(Result.Failure("Email or Password is invalid", ErrorType.BadRequest));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Failure(ex.Message));
        }
    }

    public Task<Result<DateTime>> ShowUserMinRegistrationDateAsync()
    {
        try
        {
            return Task.FromResult(Result<DateTime>
                .Success(userStorage
                    .Values
                    .Min(u => u.CreatedAt)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<DateTime>.Failure(ex.Message));
        }
    }
    
    public Task<Result<DateTime>> ShowUserMaxRegistrationDateAsync()
    {
        try
        {
            return Task.FromResult(Result<DateTime>
                .Success(userStorage
                    .Values
                    .Max(u => u.CreatedAt)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<DateTime>.Failure(ex.Message));
        }
    }

    public Task<Result<IEnumerable<CustomUser>>> ShowUsersSortedByDateAsync()
    {
        try
        {
            return Task.FromResult(Result<IEnumerable<CustomUser>>
                .Success(userStorage
                    .Values
                    .OrderBy(u => u.CreatedAt)
                    .ToList()));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<CustomUser>>.Failure(ex.Message));
        }
    }

    public Task<Result<IEnumerable<CustomUser>>> ShowUsersByGenderAsync(bool isMale)
    {
        try
        {
            return isMale
                ? Task.FromResult(Result<IEnumerable<CustomUser>>.Success(
                    userStorage
                        .Values
                        .Where(u => u.IsMale)))
                : Task.FromResult(Result<IEnumerable<CustomUser>>.Success(
                    userStorage
                        .Values
                        .Where(u => !u.IsMale)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<CustomUser>>.Failure(ex.Message));
        }
    }

    public Task<Result<IEnumerable<CustomUser>>> ShowUserByIdAsync(int pageNumber, int pageSize)
    {
        try
        {
            return Task.FromResult(
                Result<IEnumerable<CustomUser>>.Success(
                    userStorage
                        .Values
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<IEnumerable<CustomUser>>.Failure(ex.Message));
        }
    }
}