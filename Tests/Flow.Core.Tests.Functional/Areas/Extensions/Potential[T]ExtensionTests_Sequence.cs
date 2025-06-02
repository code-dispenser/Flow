using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using FluentAssertions;
using FluentAssertions.Execution;

namespace Flow.Core.Tests.Functional.Areas.Extensions;

public class PotentialExtensionTests_Sequence
{
    [Fact]
    public void Sequence_should_convert_an_ienumerable_of_potentials_to_single_potential_with_values_if_they_all_have_values()
    {
        List<Potential<int>> listOfPotentials = [1, 2, 3];

        var potentialValues = listOfPotentials.Sequence();


        using (new FluentAssertions.Execution.AssertionScope())
        {
            potentialValues.Should().Match<Potential<IEnumerable<int>>>(r => r.HasValue == true && r.HasNoValue == false);
            var list = potentialValues.Reduce([]).ToList();
            list.Should().HaveCount(3);
            list[1].Should().Be(2);
        }
    }
    [Fact]
    public void Sequence_should_convert_an_ienumerable_of_potentials_to_single_potential_with_no_value_if_any_potential_has_no_value()
    {
        List<Potential<int>> listOfPotentials = [Potential<int>.WithValue(1), Potential<int>.WithoutValue()];

        var potentialValues = listOfPotentials.Sequence();

        using (new AssertionScope())
        {
            potentialValues.Should().Match<Potential<IEnumerable<int>>>(r => r.HasValue == false && r.HasNoValue == true);

            var list = potentialValues.Reduce([]).ToList();

            list.Should().HaveCount(0);

        }
    }

    [Fact]
    public void Sequence_should_through_an_exception_if_the_collection_is_null()
    {
        List<Potential<int>> listOfPotentials = null!;

        FluentActions.Invoking(() => listOfPotentials.Sequence()).Should().Throw<ArgumentNullException>();
    }
}
