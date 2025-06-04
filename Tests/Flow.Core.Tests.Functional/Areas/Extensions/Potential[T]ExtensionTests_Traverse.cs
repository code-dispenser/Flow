using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using FluentAssertions;

namespace Flow.Core.Tests.Functional.Areas.Extensions
{
    public class Potential_ExtensionTests_Traverse
    {
        [Fact]
        public void Traverse_should_return_a_potential_with_values_when_all_potentials_have_values()
        {
            List<int> source = [1, 2, 3];
            var       result = source.Traverse(value => Potential<int>.WithValue(value * 2));

            result.Should().Match<Potential<IEnumerable<int>>>(r => r.HasValue == true && r.GetValueOr(new List<int>()).SequenceEqual(new List<int> { 2, 4, 6 }));
        }

        [Fact]
        public void Traverse_should_return_a_potential_with_no_value_if_any_potential_has_no_value()
        {
            List<int> source = [1, 0, 3];
            Func<int,Potential<int>> transformer = value => value == 0 ? Potential<int>.WithoutValue() : Potential<int>.WithValue(value * 2);

            var result = source.Traverse(transformer);

            result.Should().Match<Potential<IEnumerable<int>>>(r => r.HasValue == false && r.GetValueOr(new List<int>()).SequenceEqual(new List<int>()));
        }

        [Fact]
        public void Traverse_should_throw_an_exception_if_the_collection_is_null()
        {
            List<int> source = null!;

            FluentActions.Invoking(() => source.Traverse(value => Potential<int>.WithValue(value * 2))).Should().Throw<ArgumentNullException>();
        }

    }
}
