namespace WordleClone.API.DTOs;

public class UserResponseDto
{
    public int Id {get; set;}
    public required string Username {get; set;}
    public int Points { get; set; } 
    public int GamesWon {get; set;}
}