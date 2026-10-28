using System.ComponentModel.DataAnnotations;

namespace WordleClone.API.Models;

public class Wordl {

    public int Id {get; set;}
    [RegularExpression(@"^[a-z]{5}$")] // Exactly 5 lowercase letters
    [MaxLength(5)]
    public required string Word{get; set;}

}