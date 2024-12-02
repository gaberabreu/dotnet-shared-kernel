using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace SharedKernel.UnitTests;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<Mediator>> _loggerMock;
    private readonly Mock<RequestHandlerDelegate<SampleResponse>> _nextMock;
    private readonly LoggingBehavior<SampleRequest, SampleResponse> _behavior;

    public LoggingBehaviorTests()
    {
        _loggerMock = new Mock<ILogger<Mediator>>();
        _nextMock = new Mock<RequestHandlerDelegate<SampleResponse>>();
        _behavior = new LoggingBehavior<SampleRequest, SampleResponse>(_loggerMock.Object);
    }

    [Fact]
    public async Task Process_ShouldLogRequestAndResponse()
    {
        // Arrange
        var request = new SampleRequest();
        var response = new SampleResponse(Guid.NewGuid());
        var cancellationToken = new CancellationToken();

        _nextMock.Setup(handler => handler()).ReturnsAsync(response);

        // Act
        var result = await _behavior.Handle(request, _nextMock.Object, cancellationToken);

        // Assert
        result.Should().Be(response);
        _nextMock.Verify(handler => handler(), Times.Once);
    }

    public record SampleResponse(Guid Id);
    public record SampleRequest : IRequest<SampleResponse>;
}
