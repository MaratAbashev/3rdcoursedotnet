namespace UserCRUD.Domain.Utils;

public enum ErrorType
{
    Generic,
    Unauthorized,
    NotFound,
    Validation,
    Conflict,
    BadRequest,
    Forbidden,
    Exception
}