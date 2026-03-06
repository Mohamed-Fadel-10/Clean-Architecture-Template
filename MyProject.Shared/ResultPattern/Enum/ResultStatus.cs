namespace Domain.Enums.ResultPattern
{
    public enum ResultStatus
    {
        Success = 200,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        Error = 500,
    }
}
