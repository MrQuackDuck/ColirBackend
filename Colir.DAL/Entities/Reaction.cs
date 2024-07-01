﻿using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class Reaction : BaseEntity
{
    public string Symbol { get; set; } = default!;

    [ForeignKey(nameof(Author))]
    public long AuthorId { get; set; }

    [ForeignKey(nameof(Message))]
    public long MessageId { get; set; }

    public User Author { get; set; } = default!;
    public Message Message { get; set; } = default!;
}