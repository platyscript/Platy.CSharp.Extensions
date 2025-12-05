using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Platy.CSharp.Extensions;

public static class EnumExtensions
{
  
  // Cache to store display names of enum values.
  private static readonly ConcurrentDictionary<Enum, string> DisplayNameCache = new();


  /// <summary>
  ///     Gets an attribute on an enum field value.
  /// </summary>
  /// <param name="enumValue">The enum value</param>
  /// <typeparam name="T">The type of the attribute to retrieve.</typeparam>
  /// <returns>The attribute of the specified type or null</returns>
  public static T? GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
  {
    var type = enumValue.GetType();

    // Use GetField to field info for the enum value
    var memInfo = type.GetField(enumValue.ToString(), BindingFlags.Public | BindingFlags.Static);

    if (memInfo == null)
    {
      return null;
    }

    // Retrieve the custom attributes of type T
    var attributes = memInfo.GetCustomAttributes<T>(false);
    return attributes.FirstOrDefault();
  }

  /// <summary>
  ///     Gets the enum display name.
  /// </summary>
  /// <param name="enumValue">The enum value.</param>
  /// <returns>
  ///     Use <see cref="DisplayAttribute" /> if it exists.
  ///     Otherwise, use the standard string representation.
  /// </returns>
  public static string GetDisplayName(this Enum enumValue)
  {
    // Retrieve the display name from the cache if it exists
    return DisplayNameCache.GetOrAdd(enumValue, e =>
    {
      // Get the DisplayAttribute
      var attribute = e.GetAttributeOfType<DisplayAttribute>();

      // Return the DisplayAttribute name if it exists, otherwise return the enum's string representation
      return attribute?.Name is not null ? attribute.Name : e.ToString();
    });
  }}
