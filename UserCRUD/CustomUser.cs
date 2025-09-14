namespace UserCRUD;

public class CustomUser
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public int PasswordHash { get; set; }
}