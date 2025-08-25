using Flow.Core.Common.Models;
using FluentAssertions;

namespace Flow.Core.Tests.Unit.Common.Models;

public class InvalidEntryTests
{
    [Fact]
    public void Invalid_entry_all_properties_should_be_settable_by_the_constructor()

        => new InvalidEntry("FailureMessage", "Path", "PropertyName", "DisplayName", "SystemError")
                .Should().Match<InvalidEntry>(i => i.Path == "Path" && i.PropertyName == "PropertyName" && i.DisplayName == "DisplayName" && i.FailureMessage == "FailureMessage"
                                           & i.Cause == "SystemError");


    [Fact]
    public void Invalid_entry_any_null_property_value_should_be_set_to_an_empty_string_via_the_constructor()

        => new InvalidEntry(null!, null!, null!, null!, null!)
                .Should().Match<InvalidEntry>(i => i.Path == "" && i.PropertyName == "" && i.DisplayName == "" && i.FailureMessage == ""
                                           & i.Cause == "");


    [Fact]
    public void Invalid_entry_should_make_copy_of_self_with_an_empty_with_as_there_are_no_public_setters()
    {
        var testWith = new InvalidEntry("FailureMessage", "Path", "PropertyName", "DisplayName", "SystemError");

        var copy = testWith with { };

        copy.Should().BeEquivalentTo(testWith);

    }
}
