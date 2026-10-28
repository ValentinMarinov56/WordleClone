namespace WordleClone.API.DTOs;

public class ReviewResponseDto
{
    public int Id {get; set;}
    public required string Text {get; set;}
    public required string AuthorUsername {get;set;}
    public required string UserId {get;set;}
}