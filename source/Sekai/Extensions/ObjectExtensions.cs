// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.ComponentModel;

namespace Sekai.Extensions;

public static class ObjectExtensions
{
    public static string GetDescription(this object obj)
    {
        if (obj is string str)
            return str;

        var type = obj as Type ?? obj.GetType();
        var field = type.GetField(obj.ToString() ?? string.Empty);

        if (field is not null)
        {
            var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            if (attr is not null)
            {
                return ((DescriptionAttribute)attr).Description;
            }
        }

        return obj.ToString() ?? string.Empty;
    }
}
