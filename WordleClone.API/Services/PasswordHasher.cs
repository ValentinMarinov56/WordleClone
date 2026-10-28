using System.Security.Cryptography;

namespace WordleClone.API.Services;

public interface IPasswordHasher
{
    string HashPassword (string password);
    bool VerifyPassword (string password, string hash);
}

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Itearation = 100000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt,Itearation,Algorithm,HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool VerifyPassword(string password, string hash)
    {
        var parts = hash.Split('-');
        if(parts.Length != 2)
        {
            return false;
        }

        var hashBytes = Convert.FromHexString(parts[0]);
        var saltBytes = Convert.FromHexString(parts[1]);
        var passwordHash = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Itearation, Algorithm, HashSize);

        return hashBytes.SequenceEqual(passwordHash);
    }
}