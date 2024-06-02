using Flow.Core.Areas.Returns;
using Flow.Core.Demos.AppServer.Common.Seeds;

namespace Flow.Core.Demos.AppServer.Common.Validators
{
    public class FakeValidator
    {
        public Task<Flow<bool>> IsValid(IValidatable commandToValidate)

        => Random.Shared.Next(0, 4) == 0
            ? Task.FromResult(Flow<bool>.Failed(new Failure.ValidationFailure("The following fields failed validation:", new() { ["Some field name"] = "Wrong value." }, 0, false)))
            : Task.FromResult(Flow<bool>.Success(true));

    }
}
