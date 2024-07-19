namespace Colir.BLL.Models;

public class ReactionModel
{
    public long Id { get; set; }
    public string Symbol { get; set; } = default!;
    public int AuthorHexId { get; set; }
}