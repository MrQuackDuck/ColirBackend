using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class BaseEntity
{
    [Key]
    public long Id { get; set; }
}