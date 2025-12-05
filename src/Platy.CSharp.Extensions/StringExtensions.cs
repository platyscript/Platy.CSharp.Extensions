using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Platy.CSharp.Extensions;

public static class StringExtensions
{

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under MIT.
// The code below is copied from Microsoft.OpenApi

  private static readonly ConcurrentDictionary<Type, ReadOnlyDictionary<string, object>> EnumDisplayCache = new();
    
  /// <summary>
  /// Gets the enum value associated with its DisplayAttribute Name.
  /// </summary>
  /// <param name="displayName">The value of the Name property of the DisplayAttribute</param>
  /// <param name="result">The enum field value</param>
  /// <typeparam name="T">The enum type</typeparam>
  /// <returns>The enum field value associated with the DisplayName attribute</returns>
  public static bool TryGetEnumFromDisplayName<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(this string? displayName,
    out T? result) where T : Enum
  {
    var type = typeof(T);

    var displayMap = EnumDisplayCache.GetOrAdd(type, _ => GetEnumValues<T>(type));

    if (displayName is not null && displayMap.TryGetValue(displayName, out var cachedValue))
    {
      result = (T)cachedValue;
      return true;
    }

    result = default;
    return false;
  }

  /// <summary>
  /// Maps an enum's DisplayAttribute name with the Enum field value
  /// </summary>
  /// <param name="enumType">The Enum type whose fields have a DisplayAttrbute</param>
  /// <typeparam name="T">An enum type</typeparam>
  /// <returns>A mapping between the DisplayAttribute name and the corresponding enum value</returns>
  public static ReadOnlyDictionary<string, object> GetEnumValues<T>(
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type enumType) where T : Enum
  {
    var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    foreach (var field in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
    {
      if (field.GetCustomAttribute<DisplayAttribute>() is { } displayAttribute
          && field.GetValue(null) is T enumValue
          && displayAttribute.Name is not null)
      {
        result.Add(displayAttribute.Name, enumValue);
      }
    }

    return new ReadOnlyDictionary<string, object>(result);
  }}
