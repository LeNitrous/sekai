// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sekai.Allocation;
using Sekai.Extensions;

namespace Sekai;

/// <summary>
/// Base class for all objects that resolve dependencies on the <see cref="ServiceLocator"/>.
/// </summary>
public abstract class DependencyObject : FrameworkObject
{
    protected DependencyObject()
    {
        resolve(this);
    }

    private static readonly MethodInfo serviceLocatorResolve = typeof(ServiceLocator).GetMethod(nameof(ServiceLocator.Resolve), 0, BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(Type), typeof(bool) }, null)!;
    private static readonly PropertyInfo serviceLocatorCurrent = typeof(ServiceLocator).GetProperty(nameof(ServiceLocator.Current), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly Dictionary<Type, Action<DependencyObject>> resolvers = new();

    private static void resolve(DependencyObject obj)
    {
        var type = obj.GetType();

        if (!resolvers.TryGetValue(type, out var resolver))
        {
            var expr = new List<Expression>();
            var self = Expression.Parameter(typeof(DependencyObject), "this");
            var curr = type;

            do
            {
                expr.AddRange
                (
                    curr
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                        .Where(info => info.CanWrite && info.GetCustomAttribute<ResolvedAttribute>() is not null)
                        .Select(info =>
                        {
                            var prop = Expression.Property(Expression.Convert(self, curr), info);
                            var arg0 = Expression.Constant(info.PropertyType, typeof(Type));
                            var arg1 = Expression.Constant(!info.IsNullable(), typeof(bool));
                            var call = Expression.Call(Expression.Property(null, serviceLocatorCurrent), serviceLocatorResolve, arg0, arg1);
                            return Expression.Assign(prop, Expression.Convert(call, info.PropertyType));
                        })
                );
            }
            while ((curr = curr?.BaseType) is not null);

            expr.Add(Expression.Empty());

            resolver = Expression.Lambda<Action<DependencyObject>>(Expression.Block(expr), false, self).Compile();
            resolvers.Add(type, resolver);
        }

        resolver(obj);
    }
}
