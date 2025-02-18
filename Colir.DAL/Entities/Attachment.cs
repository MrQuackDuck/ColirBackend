﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

[Index(nameof(Path), IsUnique = true)]
public class Attachment : BaseEntity
{
    [MaxLength(256)]
    public string Filename { get; set; } = default!;

    [MaxLength(1024)]
    public string Path { get; set; } = default!;

    public long SizeInBytes { get; set; }

    [ForeignKey(nameof(Message))]
    public long? MessageId { get; set; }

    public Message? Message { get; set; }

    public bool IsInRoom(string roomGuid) =>
        Path.ToLowerInvariant().Contains(roomGuid.ToLowerInvariant());
}