namespace UserCRUD.Application.Models;

public record UpdateUserRequest(string NewEmail, string NewPassword);