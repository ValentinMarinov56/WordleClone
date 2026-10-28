namespace WordleClone.API.DTOs;
public class LoginRequestDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Password {get; set;}
}