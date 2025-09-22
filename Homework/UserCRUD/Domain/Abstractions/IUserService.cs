using UserCRUD.Domain.Models;
using UserCRUD.Domain.Utils;

namespace UserCRUD.Domain.Abstractions;

public interface IUserService
{
    Task<Result<CustomUser>> CreateUserAsync(string email, string password);
    Task<Result<CustomUser>> UpdateUserAsync(Guid userId, string newEmail, string newPassword);
    Task<Result> DeleteUserAsync(Guid userId);
    Task<Result<CustomUser>> GetUserAsync(Guid userId);
    Task<Result> LoginUserAsync(string email, string password);
}