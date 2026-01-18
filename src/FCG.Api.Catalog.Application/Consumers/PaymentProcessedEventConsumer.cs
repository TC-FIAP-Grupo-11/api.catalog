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

        // Buscar UserGame pelo OrderId
        var userGame = await _gameRepository.GetUserGameByOrderIdAsync(evt.OrderId);
        
        if (userGame is null)
        {
            _logger.LogWarning(
                "UserGame não encontrado para o pedido {OrderId}",
                evt.OrderId);
            return;
        }

        if (evt.Status == PaymentStatus.Approved)
        {
            userGame.Complete();
            await _gameRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Pedido {OrderId} confirmado - Jogo {GameId} adicionado à biblioteca do usuário {UserId}",
                evt.OrderId,
                evt.GameId,
                evt.UserId);
        }
        else
        {
            userGame.Fail();
            await _gameRepository.SaveChangesAsync();

            _logger.LogWarning(
                "Pagamento rejeitado para pedido {OrderId}: {Message}",
                evt.OrderId,
                evt.Message);
        }
    }
}
