namespace FCG.Api.Catalog.Application.Contracts.Users;

/// <summary>
/// DTO para representar dados do usuário vindos da API de Usuários
/// </summary>
public record UserDto(
    Guid Id,
    string Name,
    string Email,
    RoleDto Role,
    string? AccountId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record RoleDto(
    int Id,
    string Name
);
