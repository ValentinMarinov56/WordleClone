namespace WordleClone.API.Models;
public class Review
{
    public int Id {get; set;}
    public required string Text {get; set;}
    public int? UserId {get; set;} // make the UserId and User nullable so that if the user is deleted we can put 'unknown' as the author
    public User? User { get; set; } = null!; // Initializes the property to null while suppressing a nullable-reference warning.
}