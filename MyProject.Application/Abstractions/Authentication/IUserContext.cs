namespace Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid? CurrentUserId { get; }
    string Email { get; }
    string CurrentUserName { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
    public bool SoftDeleteEnabled { get; set; }
}
