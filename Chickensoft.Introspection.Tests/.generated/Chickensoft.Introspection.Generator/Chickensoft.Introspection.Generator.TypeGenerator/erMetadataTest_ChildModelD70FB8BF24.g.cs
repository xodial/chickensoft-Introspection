﻿#pragma warning disable
#nullable enable
namespace Chickensoft.Introspection.Tests;

using Chickensoft.Introspection;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

partial class TypeGraphMemberMetadataTest {
  partial class ChildModel : Chickensoft.Introspection.IIntrospectiveRef {
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public Chickensoft.Introspection.MixinBlackboard MixinState { get; } = new();
    
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public Chickensoft.Introspection.IMetatype Metatype => ((Chickensoft.Introspection.IIntrospectiveTypeMetadata)Chickensoft.Introspection.Types.Graph.GetMetadata(typeof(ChildModel))).Metatype;
    
    public class MetatypeMetadata : Chickensoft.Introspection.IMetatype {
      [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
      public System.Type Type => typeof(ChildModel);
      [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
      public bool HasInitProperties { get; } = true;
      
      [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
      public System.Collections.Generic.IReadOnlyList<Chickensoft.Introspection.PropertyMetadata> Properties { get; } = new System.Collections.Generic.List<Chickensoft.Introspection.PropertyMetadata>() {
        new Chickensoft.Introspection.PropertyMetadata(
          Name: "ChildName",
          IsInit: true,
          IsRequired: false,
          HasDefaultValue: false,
          Getter: static (object obj) => ((ChildModel)obj).ChildName,
          Setter: null,
          TypeNode: new Chickensoft.Introspection.TypeNode(
            OpenType: typeof(string),
            ClosedType: typeof(string),
            IsNullable: true,
            Arguments: System.Array.Empty<TypeNode>(),
            GenericTypeGetter: static receiver => receiver.Receive<string?>(),
            GenericTypeGetter2: default
          ),
          Attributes: new System.Collections.Generic.Dictionary<System.Type, System.Attribute[]>() {
          }
        )
      };
      
      [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
      public System.Collections.Generic.IReadOnlyDictionary<System.Type, System.Attribute[]> Attributes { get; } = new System.Collections.Generic.Dictionary<System.Type, System.Attribute[]>() {
        [typeof(MetaAttribute)] = new System.Attribute[] {
          new MetaAttribute()
        }, 
        [typeof(TagAttribute)] = new System.Attribute[] {
          new TagAttribute("child")
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
        args = args ?? throw new System.ArgumentNullException(nameof(args), "Constructing ChildModel requires init args.");
        return new ChildModel() {
          Age = args.ContainsKey("Age") ? (int)args["Age"] : default(int)!, 
          ChildName = args.ContainsKey("ChildName") ? (string?)args["ChildName"] : default(string?), 
          Name = args.ContainsKey("Name") ? (string?)args["Name"] : default(string?)
        };
      }
      [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
      public override bool Equals(object obj) => true;
      [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
      public override int GetHashCode() => base.GetHashCode();
    }
  }
}
#nullable restore
#pragma warning restore
