﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SoundExplorers.Model {
  public class PropertyDictionary : Dictionary<string, PropertyInfo> {
    public PropertyDictionary(Type type) {
      var properties = type.GetProperties().ToArray();
      foreach (var property in properties) {
        Add(property.Name, property);
      }
    }
  }
}