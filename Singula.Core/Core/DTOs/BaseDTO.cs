namespace Singula.Core.Core.DTOs;

/// <summary>
/// DTO base con propiedades comunes
/// </summary>
public abstract class BaseDTO
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
