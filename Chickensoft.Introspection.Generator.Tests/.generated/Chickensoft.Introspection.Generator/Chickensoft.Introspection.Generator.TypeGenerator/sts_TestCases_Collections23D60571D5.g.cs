﻿#pragma warning disable
#nullable enable
namespace Chickensoft.Introspection.Generator.Tests.TestCases;

using Chickensoft.Introspection.Generator.Tests.TestUtils;
using System.Collections.Generic;

partial class Collections : Chickensoft.Introspection.IIntrospectiveRef {
  [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
  public Chickensoft.Introspection.MixinBlackboard MixinState { get; } = new();
  
  [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
  public Chickensoft.Introspection.IMetatype Metatype => ((Chickensoft.Introspection.IIntrospectiveTypeMetadata)Chickensoft.Introspection.Types.Graph.GetMetadata(typeof(Collections))).Metatype;
  
  public class MetatypeMetadata : Chickensoft.Introspection.IMetatype {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public System.Type Type => typeof(Collections);
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public bool HasInitProperties { get; } = false;
    
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public System.Collections.Generic.IReadOnlyList<Chickensoft.Introspection.PropertyMetadata> Properties { get; } = new System.Collections.Generic.List<Chickensoft.Introspection.PropertyMetadata>() {
      new Chickensoft.Introspection.PropertyMetadata(
        Name: "NestedCollections",
        IsInit: false,
        IsRequired: false,
        HasDefaultValue: true,
        Getter: static (object obj) => ((Collections)obj).NestedCollections,
        Setter: null,
        TypeNode: new Chickensoft.Introspection.TypeNode(
          OpenType: typeof(Dictionary<,>),
          ClosedType: typeof(Dictionary<List<string>, List<List<int>>>),
          IsNullable: false,
          Arguments: new TypeNode[] {
              new Chickensoft.Introspection.TypeNode(
                OpenType: typeof(List<>),
                ClosedType: typeof(List<string>),
                IsNullable: false,
                Arguments: new TypeNode[] {
                    new Chickensoft.Introspection.TypeNode(
                      OpenType: typeof(string),
                      ClosedType: typeof(string),
                      IsNullable: false,
                      Arguments: System.Array.Empty<TypeNode>(),
                      GenericTypeGetter: static receiver => receiver.Receive<string>(),
                      GenericTypeGetter2: default
                    )
                },
                GenericTypeGetter: static receiver => receiver.Receive<List<string>>(),
                GenericTypeGetter2: default
              ), 
              new Chickensoft.Introspection.TypeNode(
                OpenType: typeof(List<>),
                ClosedType: typeof(List<List<int>>),
                IsNullable: false,
                Arguments: new TypeNode[] {
                    new Chickensoft.Introspection.TypeNode(
                      OpenType: typeof(List<>),
                      ClosedType: typeof(List<int>),
                      IsNullable: false,
                      Arguments: new TypeNode[] {
                          new Chickensoft.Introspection.TypeNode(
                            OpenType: typeof(int),
                            ClosedType: typeof(int),
                            IsNullable: false,
                            Arguments: System.Array.Empty<TypeNode>(),
                            GenericTypeGetter: static receiver => receiver.Receive<int>(),
                            GenericTypeGetter2: default
                          )
                      },
                      GenericTypeGetter: static receiver => receiver.Receive<List<int>>(),
                      GenericTypeGetter2: default
                    )
                },
                GenericTypeGetter: static receiver => receiver.Receive<List<List<int>>>(),
                GenericTypeGetter2: default
              )
          },
          GenericTypeGetter: static receiver => receiver.Receive<Dictionary<List<string>, List<List<int>>>>(),
          GenericTypeGetter2: static receiver => receiver.Receive<List<string>, List<List<int>>>()
        ),
        Attributes: new System.Collections.Generic.Dictionary<System.Type, System.Attribute[]>() {
          [typeof(TagAttribute)] = new System.Attribute[] {
            new TagAttribute("nested_collections")
          }
        }
      )
    };
    
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public System.Collections.Generic.IReadOnlyDictionary<System.Type, System.Attribute[]> Attributes { get; } = new System.Collections.Generic.Dictionary<System.Type, System.Attribute[]>() {
      [typeof(MetaAttribute)] = new System.Attribute[] {
        new MetaAttribute()
      }
    };
    
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public System.Collections.Generic.IReadOnlyList<System.Type> Mixins { get; } = new System.Collections.Generic.List<System.Type>() {
    };
    
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public System.Collections.Generic.IReadOnlyDictionary<System.Type, System.Action<object>> MixinHandlers { get; } = new System.Collections.Generic.Dictionary<System.Type, System.Action<object>>() {
    };
    
    
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public object Construct(System.Collections.Generic.IReadOnlyDictionary<string, object?>? args = null) {
      return new Collections();
    }
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public override bool Equals(object obj) => true;
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public override int GetHashCode() => base.GetHashCode();
  }
}
#nullable restore
#pragma warning restore
