namespace Chickensoft.Introspection.Generator.Models;

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using Chickensoft.Introspection.Generator.Utils;
using Microsoft.CodeAnalysis;

/// <summary>
/// Represents a declared type.
/// </summary>
/// <param name="Reference">Type reference, including the name, construction,
/// type parameters, and whether or not the type is partial.</param>
/// <param name="SyntaxLocation">Syntax node location (used for diagnostics).
/// </param>
/// <param name="Location">Location of the type in the source code, including
/// namespaces and containing types.</param>
/// <param name="BaseType">Base type of the type.</param>
/// <param name="Usings">Using directives that are in scope for the type.
/// </param>
/// <param name="Kind">Kind of the type.</param>
/// <param name="IsStatic">True if the type is static. Static types can't
/// provide generic type retrieval.</param>
/// <param name="IsPublicOrInternal">True if the public or internal
/// visibility modifier was seen on the type.</param>
/// <param name="Properties">Properties declared on the type.</param>
/// <param name="Attributes">Attributes declared on the type.</param>
/// <param name="Mixins">Mixins that are applied to the type.</param>
public sealed record DeclaredType(
  TypeReference Reference,
  Location SyntaxLocation,
  TypeLocation Location,
  string? BaseType,
  ImmutableHashSet<UsingDirective> Usings,
  DeclaredTypeKind Kind,
  bool IsStatic,
  bool IsPublicOrInternal,
  ImmutableArray<DeclaredProperty> Properties,
  ImmutableArray<DeclaredAttribute> Attributes,
  ImmutableArray<string> Mixins
) {
  private static readonly MD5 _md5 = MD5.Create();
  private const int NAME_PORTION = 25;
  private const int HASH_PORTION = 10;

  /// <summary>Output filename (only works for non-generic types).</summary>
  public string Filename {
    get {
      var name = FullNameOpen.Replace('.', '_');

      if (name.Length <= NAME_PORTION + HASH_PORTION) {
        return name;
      }

      // Name is too long, grab last section of the string
      var truncated = name.Substring(name.Length - NAME_PORTION);
      var hash = _md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(name));
      var hashString = BitConverter.ToString(hash).Replace("-", "");
      var hashLast = hashString.Substring(
        hashString.Length - HASH_PORTION
      );
      return $"{truncated}{hashLast}";
    }
  }

  /// <summary>
  /// Fully qualified, open generic name, as determined based on syntax nodes
  /// only.
  /// </summary>
  public string FullNameOpen => Location.Prefix + Reference.SimpleNameOpen;

  /// <summary>
  /// Fully qualified, closed generic name, as determined based on syntax nodes
  /// only.
  /// </summary>
  public string FullNameClosed => Location.Prefix + Reference.SimpleNameClosed;

  /// <summary>
  /// True if the metatype information can be generated for this type.
  /// </summary>
  public bool CanGenerateMetatypeInfo =>
    HasIntrospectiveAttribute &&
    Location.IsFullyPartialOrNotNested &&
    !IsGeneric;

  /// <summary>True if the type has the meta attribute.</summary>
  public bool HasIntrospectiveAttribute => IntrospectiveAttribute is not null;

  /// <summary>True if the type has a mixin attribute.</summary>
  public bool HasMixinAttribute => MixinAttribute is not null;

  /// <summary>True if the type has a version attribute.</summary>
  public bool HasVersionAttribute => VersionAttribute is not null;

  /// <summary>
  /// Whether or not the declared type was given a specific identifier.
  /// </summary>
  public bool HasIdAttribute => IdAttribute is not null;

  /// <summary>
  /// True if the type is generic. A type is generic if it has type parameters
  /// or is nested inside any containing types that have type parameters.
  /// </summary>
  public bool IsGeneric =>
    Reference.TypeParameters.Length > 0 ||
    Location.IsInGenericType;

  /// <summary>
  /// Identifier of the type. Types tagged with the [Meta] attribute can also
  /// be tagged with the optional [Id] attribute, which allows a custom string
  /// identifier to be given as the type's id.
  /// </summary>
  public string? Id => IdAttribute?.ConstructorArgs.FirstOrDefault();

  /// <summary>
  /// Version of the type. Types tagged with the [Meta] attribute can also be
  /// tagged with the optional [Id] attribute, which allows a custom version
  /// number to be given as the type's version.
  /// </summary>
  public int Version {
    get {
      if (Kind == DeclaredTypeKind.AbstractType) {
        // Abstract types don't have versions.
        return -1;
      }

      return int.TryParse(
        VersionAttribute?.ConstructorArgs.FirstOrDefault() ?? "1",
        out var version
      ) ? version : 1;
    }
  }

  /// <summary>
  /// Validates that the DeclaredType of this type and its containing types
  /// satisfy the given predicate. Returns a list of types that do not satisfy
  /// the predicate.
  /// </summary>
  /// <param name="allTypes">Table of type full names with open generics to
  /// the declared type they represent.</param>
  /// <param name="predicate">Predicate each type must satisfy.</param>
  /// <returns>Enumerable of types that do not satisfy the predicate.</returns>
  public IEnumerable<DeclaredType> ValidateTypeAndContainingTypes(
    IDictionary<string, DeclaredType> allTypes,
    Func<DeclaredType, bool> predicate
  ) {
    // Have to reconstruct the full names of the containing types from our
    // type reference and location information.
    var fullName = Location.Namespace;
    var containingTypeFullNames = new Dictionary<TypeReference, string>();

    foreach (var containingType in Location.ContainingTypes) {
      fullName +=
        (fullName.Length == 0 ? "" : ".") +
        containingType.SimpleNameOpen;

      containingTypeFullNames[containingType] = fullName;
    }

    var typesToValidate =
      new[] { this }.Concat(Location.ContainingTypes.Select(
        (typeRef) => allTypes[containingTypeFullNames[typeRef]]
      )
    );

    return typesToValidate.Where((type) => !predicate(type));
  }

  private DeclaredAttribute? IntrospectiveAttribute =>
    Attributes
      .FirstOrDefault(
        (attr) => attr.Name == Constants.INTROSPECTIVE_ATTRIBUTE_NAME
      );

  private DeclaredAttribute? MixinAttribute => Attributes
      .FirstOrDefault((attr) => attr.Name == Constants.MIXIN_ATTRIBUTE_NAME);

  private DeclaredAttribute? IdAttribute => Attributes
      .FirstOrDefault((attr) => attr.Name == Constants.ID_ATTRIBUTE_NAME);

  private DeclaredAttribute? VersionAttribute => Attributes
      .FirstOrDefault((attr) => attr.Name == Constants.VERSION_ATTRIBUTE_NAME);

  internal enum DeclaredTypeState {
    Unsupported,
    Type,
    ConcreteType,
    AbstractIntrospectiveType,
    ConcreteIntrospectiveType,
    AbstractIdentifiableType,
    ConcreteIdentifiableType
  }

  internal DeclaredTypeState GetState(bool knownToBeAccessibleFromGlobalScope) {
    if (Kind is DeclaredTypeKind.Interface or DeclaredTypeKind.StaticClass) {
      // Can't generate metadata about interfaces or static classes.
      return DeclaredTypeState.Unsupported;
    }

    if (!knownToBeAccessibleFromGlobalScope) {
      // Can't generate metadata about types that aren't visible from the
      // global scope.
      return DeclaredTypeState.Unsupported;
    }

    if (IsGeneric) {
      // Can't construct generic types because we wouldn't know the type
      // parameters to use.
      return DeclaredTypeState.Type;
    }

    if (HasIntrospectiveAttribute) {
      if (HasIdAttribute) {
        return Kind is DeclaredTypeKind.ConcreteType
          ? DeclaredTypeState.ConcreteIdentifiableType
          : DeclaredTypeState.AbstractIdentifiableType;
      }
      return Kind is DeclaredTypeKind.ConcreteType
        ? DeclaredTypeState.ConcreteIntrospectiveType
        : DeclaredTypeState.AbstractIntrospectiveType;
    }
    // Non-generic, non-introspective type that's visible from the global scope.
    return Kind is DeclaredTypeKind.ConcreteType
      ? DeclaredTypeState.ConcreteType
      : DeclaredTypeState.Type;
  }

  /// <summary>
  /// Merge this partial type definition with another partial type definition
  /// for the same type.
  /// </summary>
  /// <param name="declaredType">Declared type representing the same type.
  /// </param>
  /// <returns>Updated representation of the declared type.</returns>
  public DeclaredType MergePartialDefinition(
    DeclaredType declaredType
  ) => new(
    Reference: Reference.MergePartialDefinition(declaredType.Reference),
    SyntaxLocation: PickSyntaxLocation(declaredType.SyntaxLocation),
    Location: Location,
    BaseType: BaseType,
    Usings: Usings.Union(declaredType.Usings),
    Kind: PickDeclaredTypeKind(Kind, declaredType.Kind),
    IsStatic: PickIsStatic(declaredType.IsStatic),
    IsPublicOrInternal: PickIsPublicOrInternal(declaredType.IsPublicOrInternal),
    Properties
      .ToImmutableHashSet()
      .Union(declaredType.Properties)
      .ToImmutableArray(),
    Attributes.Concat(declaredType.Attributes).ToImmutableArray(),
    Mixins.Concat(declaredType.Mixins).ToImmutableArray()
  );

  internal Location PickSyntaxLocation(Location other) =>
    HasIntrospectiveAttribute ? SyntaxLocation : other;

  internal static DeclaredTypeKind PickDeclaredTypeKind(
    DeclaredTypeKind kind,
    DeclaredTypeKind other
  ) => kind switch {
    // both are the same — no change
    _ when other == kind => kind,

    // abstract + concrete = abstract
    DeclaredTypeKind.AbstractType when other is DeclaredTypeKind.ConcreteType
      => DeclaredTypeKind.AbstractType,
    DeclaredTypeKind.ConcreteType when other is DeclaredTypeKind.AbstractType
      => DeclaredTypeKind.AbstractType,
    // ---

    // static + concrete = static
    DeclaredTypeKind.StaticClass when other is DeclaredTypeKind.ConcreteType
      => DeclaredTypeKind.StaticClass,
    DeclaredTypeKind.ConcreteType when other is DeclaredTypeKind.StaticClass
      => DeclaredTypeKind.StaticClass,
    // ---

    // no other combinations have valid results
    _ => DeclaredTypeKind.Error
  };

  internal bool PickIsStatic(bool other) => IsStatic || other;

  internal bool PickIsPublicOrInternal(bool other) =>
    IsPublicOrInternal || other;

  public bool WriteMetadata(
    IndentedTextWriter writer,
    bool knownToBeAccessibleFromGlobalScope
  ) {
    const string prefix = "Chickensoft.Introspection";
    var name = $"\"{Reference.SimpleNameClosed}\"";
    var genericTypeGetter = $"static (r) => r.Receive<{FullNameClosed}>()";
    var factory =
      $"static () => System.Activator.CreateInstance<{FullNameClosed}>()";
    var metatype = $"new {FullNameClosed}.{Constants.METATYPE_IMPL}()";
    var id = Id ?? "";
    var version = $"{Version}";

    switch (GetState(knownToBeAccessibleFromGlobalScope)) {
      case DeclaredTypeState.Type:
        writer.Write($"new {prefix}.TypeMetadata({name})");
        return true;
      case DeclaredTypeState.ConcreteType:
        writer.Write(
          $"new {prefix}.ConcreteTypeMetadata({name}, {genericTypeGetter}, " +
          $"{factory})"
        );
        return true;
      case DeclaredTypeState.AbstractIntrospectiveType:
        writer.Write(
          $"new {prefix}.AbstractIntrospectiveTypeMetadata(" +
          $"{name}, {genericTypeGetter}, {metatype})"
        );
        return true;
      case DeclaredTypeState.ConcreteIntrospectiveType:
        writer.Write(
          $"new {prefix}.IntrospectiveTypeMetadata(" +
          $"{name}, {genericTypeGetter}, {factory}, {metatype}, {version})"
        );
        return true;
      case DeclaredTypeState.AbstractIdentifiableType:
        writer.Write(
          $"new {prefix}.AbstractIdentifiableTypeMetadata(" +
          $"{name}, {genericTypeGetter}, {metatype}, {id})"
        );
        return true;
      case DeclaredTypeState.ConcreteIdentifiableType:
        writer.Write(
          $"new {prefix}.IdentifiableTypeMetadata(" +
          $"{name}, {genericTypeGetter}, {factory}, {metatype}, {id}, " +
          $"{version})"
        );
        return true;
      default:
      case DeclaredTypeState.Unsupported:
        break;
    }
    return false;
  }

  public void WriteMetatype(
    IndentedTextWriter writer,
    IEnumerable<DeclaredType> baseTypes
  ) {
    var isValueType = Reference.Construction is
      Construction.RecordStruct or Construction.Struct;

    if (!string.IsNullOrEmpty(Location.Namespace)) {
      writer.WriteLine($"namespace {Location.Namespace};\n");
    }

    var usings = Usings
      .Where(u => !u.IsGlobal) // Globals are universally available
      .OrderBy(u => u.Name)
      .ThenBy(u => u.IsGlobal)
      .ThenBy(u => u.IsStatic)
      .ThenBy(u => u.IsAlias)
      .Select(@using => @using.CodeString).ToList();

    foreach (var usingDirective in usings) {
      writer.WriteLine(usingDirective);
    }

    if (usings.Count > 0) { writer.WriteLine(); }

    // Nest our metatype inside all the containing types
    foreach (var containingType in Location.ContainingTypes) {
      writer.WriteLine($"{containingType.CodeString} {{");
      writer.Indent++;
    }

    // Apply mixin interfaces to the type.
    var mixins = Mixins.Length > 0 ? ", " + string.Join(", ", Mixins) : "";

    // Types which have been given an explicit user-defined id
    // are marked as IIdentifiable to allow the serializer to reject
    // introspective types without explicitly given id's.
    var identifiable = HasIdAttribute ? $", {Constants.IDENTIFIABLE}" : "";

    var allProperties = Properties
      .Concat(baseTypes.SelectMany(t => t.Properties))
      .OrderBy(p => p.Name); // order for determinism

    var initProperties = allProperties
      .Where(prop => prop.IsInit || prop.IsRequired)
      .ToArray();

    var introspectiveInterface = isValueType
      ? Constants.INTROSPECTIVE
      : Constants.INTROSPECTIVE_REF;

    // Nest inside us.
    writer.WriteLine(
      $"{Reference.CodeString} : " +
      $"{introspectiveInterface}{identifiable}{mixins} {{"
    );
    writer.Indent++;

    if (!isValueType) {
      // Add a mixin state bucket to the type itself.
      writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
      writer.WriteLine(
        $"public {Constants.MIXIN_BLACKBOARD} MixinState {{ get; }} = new();"
      );
      writer.WriteLine();
    }

    // Add a metatype accessor to the type for convenience
    writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
    writer.WriteLine(
      $"public {Constants.METATYPE} Metatype " +
      "=> ((Chickensoft.Introspection.IIntrospectiveTypeMetadata)" +
      "Chickensoft.Introspection.Types.Graph.GetMetadata" +
      $"(typeof({Reference.SimpleName}))).Metatype;"
    );
    writer.WriteLine();

    writer.WriteLine(
      $"public class {Constants.METATYPE_IMPL} : {Constants.METATYPE} {{"
    );

    writer.Indent++; // metatype contents

    // Type property
    // ----------------------------------------------------------------- //
    writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
    writer.WriteLine(
      $"public System.Type Type => typeof({Reference.SimpleNameClosed});"
    );
    // ----------------------------------------------------------------- //

    // HasInitProperties property
    // ----------------------------------------------------------------- //
    var hasInitProperties = initProperties.Any();

    writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
    writer.WriteLine(
      "public bool HasInitProperties { get; } = " +
      $"{(hasInitProperties ? "true" : "false")};"
    );
    // ----------------------------------------------------------------- //

    writer.WriteLine();

    // Properties property
    // ----------------------------------------------------------------- //
    writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
    writer.WriteLine(
      "public System.Collections.Generic.IReadOnlyList<" +
      $"{Constants.PROPERTY_METADATA}> Properties {{ get; }} = " +
      "new System.Collections.Generic.List<" +
      $"{Constants.PROPERTY_METADATA}>() {{"
    );

    writer.WriteCommaSeparatedList(
      Properties.OrderBy(p => p.Name),
      (property) => property.Write(writer, Reference.SimpleNameClosed),
      multiline: true
    );

    writer.WriteLine("};"); // close properties list
    // ----------------------------------------------------------------- //

    writer.WriteLine();

    // Attributes property
    // ----------------------------------------------------------------- //
    writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
    writer.WriteLine(
      "public System.Collections.Generic.IReadOnlyDictionary" +
      "<System.Type, System.Attribute[]> Attributes { get; } = " +
      "new System.Collections.Generic.Dictionary" +
      "<System.Type, System.Attribute[]>() {"
    );
    DeclaredAttribute.WriteAttributeMap(writer, Attributes);
    writer.WriteLine("};"); // close attributes dictionary
    // ----------------------------------------------------------------- //

    writer.WriteLine();

    // Mixins property
    // ----------------------------------------------------------------- //
    writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
    writer.WriteLine(
      "public System.Collections.Generic.IReadOnlyList<System.Type> " +
      "Mixins { get; } = new System.Collections.Generic.List<System.Type>() {"
    );

    var orderedMixins = Mixins.OrderBy(m => m);

    writer.WriteCommaSeparatedList(
      orderedMixins,
      (mixin) => writer.Write($"typeof({mixin})"),
      multiline: true
    );

    writer.WriteLine("};"); // close mixins list
    // ----------------------------------------------------------------- //

    writer.WriteLine();

    // MixinHandlers property
    // ----------------------------------------------------------------- //
    writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
    writer.WriteLine(
      "public System.Collections.Generic.IReadOnlyDictionary" +
      "<System.Type, System.Action<object>> MixinHandlers { get; } = " +
      "new System.Collections.Generic.Dictionary" +
      "<System.Type, System.Action<object>>() {"
    );

    writer.WriteCommaSeparatedList(
      orderedMixins,
      (mixin) => writer.Write(
        $"[typeof({mixin})] = static (obj) => (({mixin})obj).Handler()"
      ),
      multiline: true
    );

    writer.WriteLine("};"); // close mixin handlers dictionary
    writer.WriteLine();
    // ----------------------------------------------------------------- //

    writer.WriteLine();

    // Generate constructor for init properties, if needed
    // ----------------------------------------------------------------- //
    writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
    writer.WriteLine(
      "public object Construct(" +
      "System.Collections.Generic.IReadOnlyDictionary<string, object?>? " +
      "args = null) {"
    );
    writer.Indent++;

    if (Kind is not DeclaredTypeKind.ConcreteType) {
      // Skip constructor implementation for non-concrete types.
      writer.WriteLine(
        "throw new System.NotImplementedException(" +
        $"\"{Reference.SimpleNameClosed} is not instantiable.\"" +
        ");"
      );

      goto CLOSE_CONSTRUCT_METHOD; /* yay! a goto! */
    }

    if (initProperties.Length == 0) {
      if (Kind is DeclaredTypeKind.ConcreteType) {
        writer.WriteLine($"return new {Reference.SimpleNameClosed}();");
      }

      goto CLOSE_CONSTRUCT_METHOD; /* wow! another goto! */
    }

    writer.WriteLine(
      $"args = args ?? throw new System.ArgumentNullException(" +
      $"nameof(args), \"Constructing {Reference.SimpleNameClosed} requires " +
      "init args.\");"
    );
    writer.WriteLine($"return new {Reference.SimpleNameClosed}() {{");

    var propStrings = allProperties
      .Where(prop => prop.IsInit || prop.IsRequired)
      .Select(
        (prop) => {
          var bang = prop.TypeNode.IsNullable ? "" : "!";
          return
            $"{prop.Name} = args.ContainsKey(\"{prop.Name}\") " +
            $"? ({prop.TypeNode.ClosedType})args[\"{prop.Name}\"] : " +
            $"{(
            prop.DefaultValueExpression is { } value
              ? value
              : $"default({prop.TypeNode.ClosedType}){bang}"
            )}";
        }
      );

    writer.WriteCommaSeparatedList(
      propStrings,
      writer.Write,
      multiline: true
    );

    writer.WriteLine("};"); // close init args

    CLOSE_CONSTRUCT_METHOD:
    writer.Indent--; // close construct method
    writer.WriteLine("}");
    // ----------------------------------------------------------------- //

    // Generate constructor for init properties, if needed
    // ----------------------------------------------------------------- //
    writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
    writer.WriteLine("public override bool Equals(object obj) => true;");
    writer.WriteLine($"[{Constants.EXCLUDE_COVERAGE}]");
    writer.WriteLine(
      "public override int GetHashCode() => base.GetHashCode();"
    );
    // ----------------------------------------------------------------- //

    writer.Indent--; // close metatype contents

    // Close nested types
    for (var i = writer.Indent; i >= 0; i--) {
      writer.WriteLine("}");
      writer.Indent--;
    }
  }


  public bool Equals(DeclaredType? other) =>
    other is not null &&
    Reference.Equals(other.Reference) &&
    SyntaxLocation.Equals(other.SyntaxLocation) &&
    Location.Equals(other.Location) &&
    Usings.SequenceEqual(other.Usings) &&
    Kind == other.Kind &&
    IsStatic == other.IsStatic &&
    HasIntrospectiveAttribute == other.HasIntrospectiveAttribute &&
    HasMixinAttribute == other.HasMixinAttribute &&
    IsPublicOrInternal == other.IsPublicOrInternal &&
    Properties.SequenceEqual(other.Properties) &&
    Attributes.SequenceEqual(other.Attributes) &&
    Mixins.SequenceEqual(other.Mixins);

  public override int GetHashCode() => HashCode.Combine(
    Reference,
    SyntaxLocation,
    Location,
    Usings,
    Kind,
    IsStatic,
    HasIntrospectiveAttribute,
    HasMixinAttribute,
    IsPublicOrInternal,
    Properties,
    Attributes,
    Mixins
  );
}
