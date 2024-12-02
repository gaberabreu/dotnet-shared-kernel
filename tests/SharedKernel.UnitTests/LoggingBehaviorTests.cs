using Ardalis.Result;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace SharedKernel.UnitTests;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<Mediator>> _loggerMock;
    private readonly Mock<RequestHandlerDelegate<Result<int>>> _nextMock;
    private readonly LoggingBehavior<SampleCommand, Result<int>> _behavior;

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<Mediator>>();
        _nextMock = new Mock<RequestHandlerDelegate<Result<int>>>();
        _behavior = new LoggingBehavior<SampleCommand, Result<int>>(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldLogRequestAndResponse()
    {
        // Arrange
        var request = new SampleCommand(Guid.NewGuid());
        var response = Result.Success(1);
        var cancellationToken = new CancellationToken();

        _nextMock.Setup(handler => handler()).ReturnsAsync(response);

        // Act
        var result = await _behavior.Handle(request, _nextMock.Object, cancellationToken);

        // Assert
        result.Should().Be(response);
        _nextMock.Verify(handler => handler(), Times.Once);
    }

    public record SampleCommand(Guid Id) : IRequest<Result<int>>;
}
