using System.ComponentModel.DataAnnotations;

namespace WordleClone.API.Models;

public class User {

    public int Id {get; set;}
    [MinLength(3)]
    [MaxLength(50)]
    public required string Username{get; set;}
    public required string PasswordHash{get; set;}
    public int Points{get;set;} = 0;
    public int GamesWon{get;set;} = 0;
    public ICollection<Review> Reviews {get; set;}
        = new List<Review>();
}