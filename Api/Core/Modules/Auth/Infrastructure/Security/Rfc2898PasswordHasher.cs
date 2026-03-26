using Api.Core.Modules.Auth.Application.Interfaces;
using System;
using System.Security.Cryptography;

namespace Api.Core.Modules.Auth.Infrastructure.Security;

public sealed class Rfc2898PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return string.Join('.', Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public bool Verify(string hash, string password)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return false;

        var segments = hash.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length != 3)
            return false;

        if (!int.TryParse(segments[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(segments[1]);
        var expected = Convert.FromBase64String(segments[2]);

        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
