using FCG.Api.Catalog.Application.Contracts.Repositories;
using FCG.Api.Catalog.Domain.Entities;
using FCG.Lib.Shared.Messaging.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FCG.Api.Catalog.Application.Consumers;

public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly IGameRepository _gameRepository;
    private readonly ILogger<PaymentProcessedEventConsumer> _logger;

    public PaymentProcessedEventConsumer(
        IGameRepository gameRepository,
        ILogger<PaymentProcessedEventConsumer> logger)
    {
        _gameRepository = gameRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var evt = context.Message;
        
        _logger.LogInformation(
            "Recebido evento PaymentProcessedEvent para pedido {OrderId} - Status: {Status}",
            evt.OrderId,
            evt.Status);

        if (evt.Status == PaymentStatus.Approved)
        {
            // Adicionar jogo à biblioteca do usuário
            var userGame = UserGame.Create(evt.UserId, evt.GameId, 0); // Preço já foi processado
            
            await _gameRepository.AddUserGameAsync(userGame);
            await _gameRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Jogo {GameId} adicionado à biblioteca do usuário {UserId}",
                evt.GameId,
                evt.UserId);
        }
        else
        {
            _logger.LogWarning(
                "Pagamento rejeitado para pedido {OrderId}: {Message}",
                evt.OrderId,
                evt.Message);
        }
    }
}
