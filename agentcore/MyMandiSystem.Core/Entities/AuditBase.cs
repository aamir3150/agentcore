using System;
using System.ComponentModel.DataAnnotations;

namespace MyMandiSystem.Core.Entities;

public abstract class AuditBase
{
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}
