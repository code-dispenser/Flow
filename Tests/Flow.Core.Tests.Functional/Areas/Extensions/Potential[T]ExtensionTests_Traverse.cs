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
            var potentials = new List<Potential<int>> { Potential<int>.WithValue(1), Potential<int>.WithValue(2), Potential<int>.WithValue(3) };
            var result     = potentials.Traverse(value => Potential<int>.WithValue(value * 2));

            result.Should().Match<Potential<IEnumerable<int>>>(r => r.HasValue == true && r.Reduce(new List<int>()).SequenceEqual(new List<int> { 2, 4, 6 }));
        }

        [Fact]
        public void Traverse_should_return_a_potential_with_no_value_if_any_potential_has_no_value()
        {
            var potentials = new List<Potential<int>> { Potential<int>.WithValue(1), Potential<int>.WithoutValue(), Potential<int>.WithValue(3) };
            var result     = potentials.Traverse(value => Potential<int>.WithValue(value * 2));

            result.Should().Match<Potential<IEnumerable<int>>>(r => r.HasValue == false && r.Reduce(new List<int>()).SequenceEqual(new List<int>()));
        }

        [Fact]
        public void Traverse_should_throw_an_exception_if_the_collection_is_null()
        {
            List<Potential<int>> listOfPotentials = null!;

            FluentActions.Invoking(() => listOfPotentials.Traverse(value => Potential<int>.WithValue(value * 2))).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Traverse_should_return_a_potential_with_no_value_if_the_function_produces_no_value()
        {
            var potentials = new List<Potential<int>> { Potential<int>.WithValue(1), Potential<int>.WithValue(2), Potential<int>.WithValue(3) };
            var result = potentials.Traverse(_ => Potential<int>.WithoutValue());

            result.Should().Match<Potential<IEnumerable<int>>>(r => r.HasValue == false && r.Reduce(new List<int>()).SequenceEqual(new List<int>()));
        }
    }
}
