namespace Chickensoft.Introspection.Generator.Tests.Models;

using Chickensoft.Introspection.Generator.Models;
using Shouldly;
using Xunit;

public class DeclaredAttributeTest {
  [Fact]
  public void Equality() {
    var attr = new DeclaredAttribute(
      "", [], []
    );

    attr.GetHashCode().ShouldBeOfType<int>();

    attr.ShouldBe(
      new DeclaredAttribute(
        "", [], []
      )
    );

    new DeclaredAttribute(
      "", [], ["b"]
    ).ShouldNotBe(
      new DeclaredAttribute(
        "a", [], []
      )
    );

    attr.ShouldNotBe(null);
  }
}
