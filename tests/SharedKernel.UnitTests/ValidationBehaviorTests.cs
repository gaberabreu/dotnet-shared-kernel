using Ardalis.Result;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;

namespace SharedKernel.UnitTests;

public class ValidationBehaviorTests
{
    private readonly Mock<RequestHandlerDelegate<Result<int>>> _nextMock;

    public ValidationBehaviorTests()
    {
        _nextMock = new Mock<RequestHandlerDelegate<Result<int>>>();
    }

    [Fact]
    public async Task Handle_ShouldDoNothingWhenHasNoValidators()
    {
        // Arrange
        var request = new SampleCommand(Guid.NewGuid());
        var response = Result.Success(1);

        var validators = new List<IValidator<SampleCommand>>();
        var behavior = new ValidationBehavior<SampleCommand, Result<int>>(validators);
        var cancellationToken = new CancellationToken();

        _nextMock.Setup(handler => handler()).ReturnsAsync(response);

        // Act
        var result = await behavior.Handle(request, _nextMock.Object, cancellationToken);

        // Assert
        result.Should().Be(response);
        _nextMock.Verify(handler => handler(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallValidatorsWhenHasAny()
    {
        // Arrange
        var request = new SampleCommand(Guid.NewGuid());
        var response = Result.Success(1);

        var validators = new List<IValidator<SampleCommand>>()
        {
            new SampleCommandValidator()
        };
        var behavior = new ValidationBehavior<SampleCommand, Result<int>>(validators);
        var cancellationToken = new CancellationToken();

        _nextMock.Setup(handler => handler()).ReturnsAsync(response);

        // Act
        await behavior.Handle(request, _nextMock.Object, cancellationToken);

        // Assert
        _nextMock.Verify(handler => handler(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowExceptionWhenFailSomeValidation()
    {
        // Arrange
        var request = new SampleCommand(Guid.Empty);
        var response = Result.Success(1);

        var validators = new List<IValidator<SampleCommand>>()
        {
            new SampleCommandValidator()
        };
        var behavior = new ValidationBehavior<SampleCommand, Result<int>>(validators);
        var cancellationToken = new CancellationToken();

        _nextMock.Setup(handler => handler()).ReturnsAsync(response);

        // Act
        await behavior.Invoking(y => y.Handle(request, _nextMock.Object, cancellationToken))
            .Should().ThrowAsync<ValidationException>();

        // Assert
        _nextMock.Verify(handler => handler(), Times.Never);
    }

    public record SampleCommand(Guid Id) : IRequest<Result<int>>;

    public class SampleCommandValidator : AbstractValidator<SampleCommand> 
    {
        public SampleCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty();
        }
    }
}
