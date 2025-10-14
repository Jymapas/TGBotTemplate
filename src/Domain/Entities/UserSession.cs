using System;

namespace Domain.Entities;

/// <summary>Состояние пользователя в finite-state машине бота.</summary>
public class UserSession
{
    public int Id { get; set; }

    public long UserId { get; set; }

    public string State { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }

    public string? PayloadJson { get; set; }
}
