namespace Chickensoft.Introspection;

using System;

/// <summary>
/// Interface applied to a type to indicate that it has generated metatype
/// information available.
/// </summary>
public interface IIntrospective {
  /// <summary>
  /// Generated metatype information.
  /// </summary>
  IMetatype Metatype { get; }
}

/// <summary>
/// Interface applied to a type to introspective reference types. Introspective
/// reference types support mixins.
/// </summary>
public interface IIntrospectiveRef : IIntrospective {
  /// <summary>
  /// Arbitrary data that is shared between mixins. Mixins are free to store
  /// additional instance state in this blackboard.
  /// </summary>
  MixinBlackboard MixinState { get; }

  /// <summary>
  /// Determines if the type has a mixin applied to it.
  /// </summary>
  /// <param name="type">Type of mixin to look for.</param>
  /// <returns>True if the type has the specified mixin, false otherwise.
  /// </returns>
  bool HasMixin(Type type) => Metatype.MixinHandlers.ContainsKey(type);

  /// <summary>
  /// Invokes the handler of each mixin that is applied to the type.
  /// </summary>
  void InvokeMixins() {
    for (var i = 0; i < Metatype.Mixins.Count; i++) {
      Metatype.MixinHandlers[Metatype.Mixins[i]](this);
    }
  }

  /// <summary>
  /// Invokes the handler of a specific mixin that is applied to the type.
  /// </summary>
  /// <param name="type">Mixin type.</param>
  /// <exception cref="InvalidOperationException" />
  void InvokeMixin(Type type) {
    if (!HasMixin(type)) {
      throw new InvalidOperationException(
        $"Type {GetType()} does not have mixin {type}"
      );
    }

    Metatype.MixinHandlers[type](this);
  }
}
