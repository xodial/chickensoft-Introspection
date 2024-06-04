﻿#pragma warning disable
#nullable enable
namespace OtherNamespace.Altogether;

using Chickensoft.Introspection;

partial class A {
  partial class B {
    partial class C {
      partial class D {
        partial class SomeBaseClass : Chickensoft.Introspection.IIntrospective {
          public Chickensoft.Introspection.MixinBlackboard MixinState { get; } = new();
          
          public Chickensoft.Introspection.IMetatype Metatype => ((Chickensoft.Introspection.IIntrospectiveTypeMetadata)Chickensoft.Introspection.Types.Graph.GetMetadata(typeof(SomeBaseClass))).Metatype;
          
          public class MetatypeMetadata : Chickensoft.Introspection.IMetatype {
            public System.Type Type => typeof(SomeBaseClass);
            public bool HasInitProperties { get; } = true;
            
            public System.Collections.Generic.IReadOnlyList<Chickensoft.Introspection.PropertyMetadata> Properties { get; } = new System.Collections.Generic.List<Chickensoft.Introspection.PropertyMetadata>() {
              new Chickensoft.Introspection.PropertyMetadata(
                Name: "Identifier",
                IsInit: true,
                IsRequired: true,
                Getter: (object obj) => ((SomeBaseClass)obj).Identifier,
                Setter: null,
                GenericType: new GenericType(
                  OpenType: typeof(string),
                  ClosedType: typeof(string),
                  Arguments: System.Array.Empty<GenericType>(),
                  GenericTypeGetter: receiver => receiver.Receive<string>(),
                  GenericTypeGetter2: default
                ),
                Attributes: new System.Collections.Generic.Dictionary<System.Type, System.Attribute[]>() {
                }
              )
            };
            
            public System.Collections.Generic.IReadOnlyDictionary<System.Type, System.Attribute[]> Attributes { get; } = new System.Collections.Generic.Dictionary<System.Type, System.Attribute[]>() {
              [typeof(MetaAttribute)] = new System.Attribute[] {
                new MetaAttribute()
              }
            };
            
            public System.Collections.Generic.IReadOnlyList<System.Type> Mixins { get; } = new System.Collections.Generic.List<System.Type>() {
            };
            
            public System.Collections.Generic.IReadOnlyDictionary<System.Type, System.Action<object>> MixinHandlers { get; } = new System.Collections.Generic.Dictionary<System.Type, System.Action<object>>() {
            };
            
            
            public object Construct(System.Collections.Generic.IReadOnlyDictionary<string, object?>? args = null) {
              args = args ?? throw new System.ArgumentNullException(nameof(args), "Constructing SomeBaseClass requires init args.");
              return new SomeBaseClass() {
                Identifier = args.ContainsKey("Identifier") ? (string)args["Identifier"] : default!
              };
            }
            public override bool Equals(object obj) => true;
            public override int GetHashCode() => base.GetHashCode();
          }
        }
      }
    }
  }
}
#nullable restore
#pragma warning restore
