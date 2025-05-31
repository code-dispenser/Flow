using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Core.Tests.Functional.Areas.Extensions;

public class MightExtensionTests_Sequence
{
    [Fact]
    public void Sequence_should_convert_an_ienumerable_of_mights_to_single_might_with_values_if_they_all_have_values()
    {
        List<Might<int>> listOfMights = [1, 2, 3];

        var mightValues = listOfMights.Sequence();


        using (new FluentAssertions.Execution.AssertionScope())
        {
            mightValues.Should().Match<Might<IEnumerable<int>>>(r => r.HasValue == true && r.HasNoValue == false);
            var list = mightValues.GetValueOr([]).ToList();
            list.Should().HaveCount(3);
            list[1].Should().Be(2);
        }
    }
    [Fact]
    public void Sequence_should_convert_an_ienumerable_of_mights_to_single_might_with_no_value_if_any_might_has_no_value()
    {
        List<Might<int>> listOfMights = [Might<int>.WithValue(1), Might<int>.WithoutValue()];

        var mightValues = listOfMights.Sequence();

        using (new AssertionScope())
        {
            mightValues.Should().Match<Might<IEnumerable<int>>>(r => r.HasValue == false && r.HasNoValue == true);

            var list = mightValues.GetValueOr([]).ToList();

            list.Should().HaveCount(0);

        }
    }

    [Fact]
    public void Sequence_should_through_an_excetpion_if_the_collection_is_null()
    {
        List<Might<int>> listOfMights = null!;

        FluentActions.Invoking(() => listOfMights.Sequence()).Should().Throw<ArgumentNullException>();
    }
}
