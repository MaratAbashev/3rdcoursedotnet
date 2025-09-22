namespace UserCRUD.Application.Models;

public record ShowUsersRequest(DateTime From, DateTime To, ShowUsersMode UsersMode);