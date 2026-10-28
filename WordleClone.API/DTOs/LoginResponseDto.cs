namespace WordleClone.API.DTOs;
public class LoginResponseDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Token { get; set; }
}