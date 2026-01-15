namespace FCG.Application.Contracts.Users;

/// <summary>
/// DTO para representar dados do usuário vindos da API de Usuários
/// </summary>
public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string Role,
    DateTime CreatedAt
);
