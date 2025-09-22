namespace UserCRUD.Domain.Models;

public class CustomUser
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public int PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}