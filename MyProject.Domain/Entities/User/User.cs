using Domain.Entities.RefreshToken.RefreshToken;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens
        => _refreshTokens.AsReadOnly();

    private User() { }

    public static User Create(string name, string email)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email
        };
    }

    public void AddRefreshToken(RefreshToken token)
    {
        var oldTokens = _refreshTokens
            .Where(t => !t.IsActive)
            .ToList();

        foreach (var old in oldTokens)
            _refreshTokens.Remove(old);

        _refreshTokens.Add(token);
    }

    public RefreshToken? GetActiveRefreshToken(string tokenHash)
    {
        return _refreshTokens
            .FirstOrDefault(t => t.TokenHash == tokenHash && t.IsActive);
    }
}