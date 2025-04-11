namespace Chickensoft.Introspection.Generator.Tests.TestCases;

[Meta]
public readonly partial record struct ValueType {
  public required int Number { get; init; }
  public string? Description { get; init; }
}

[Meta, Id("value_type_with_id")]
public readonly partial record struct ValueTypeWithId {
  public required int Number { get; init; }
  public string? Description { get; init; }
}
