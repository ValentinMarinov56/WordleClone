using System.ComponentModel.DataAnnotations;

namespace WordleClone.API.DTOs;

public sealed class UpdateUserStatsRequestDto
{
    [Range(0, int.MaxValue)]
    public int Points { get; set; }

    [Range(0, int.MaxValue)]
    public int GamesWon { get; set; }
}
