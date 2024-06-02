using Flow.Core.Demos.AppServer.Common.Seeds;

namespace Flow.Core.Demos.AppServer.Common.Validators;

public static class ValidatorFactory
{
    public static Task<FakeValidator> CreateValidatorFor<T>(T instruction) where T : IValidatable

        => Task.FromResult(new FakeValidator());
}
