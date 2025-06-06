﻿#pragma warning disable
#nullable enable
namespace Chickensoft.Introspection.Generator.Tests.TestCases;

partial class OuterContainer {
  partial class MidContainer {
    partial class AInnerContainer {
      partial class ZMyModel : Chickensoft.Introspection.IIntrospectiveRef {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Chickensoft.Introspection.MixinBlackboard MixinState { get; } = new();
        
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public Chickensoft.Introspection.IMetatype Metatype => ((Chickensoft.Introspection.IIntrospectiveTypeMetadata)Chickensoft.Introspection.Types.Graph.GetMetadata(typeof(ZMyModel))).Metatype;
        
        public class MetatypeMetadata : Chickensoft.Introspection.IMetatype {
          [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
          public System.Type Type => typeof(ZMyModel);
          [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
          public bool HasInitProperties { get; } = false;
          
          [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
          public System.Collections.Generic.IReadOnlyList<Chickensoft.Introspection.PropertyMetadata> Properties { get; } = new System.Collections.Generic.List<Chickensoft.Introspection.PropertyMetadata>() {
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
            return new ZMyModel();
          }
          [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
          public override bool Equals(object obj) => true;
          [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
          public override int GetHashCode() => base.GetHashCode();
        }
      }
    }
  }
}
#nullable restore
#pragma warning restore
