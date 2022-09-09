// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sekai.Engine.Effects.Documents;

public class EffectDocument
{
    public string Name { get; }
    public EffectDocumentPass[] Passes { get; }

    private EffectDocument(string name, EffectDocumentPass[] passes)
    {
        Name = name;
        Passes = passes;
    }

    public static EffectDocument Load(string source)
    {
        var rootMatch = regex_effect_root.Match(source);

        if (!rootMatch.Success)
            throw new InvalidOperationException(@"Root not found. May be a malformed source string.");

        var passes = new List<EffectDocumentPass>();

        string rootName = rootMatch.Groups["Name"].Value.Trim();
        string rootBody = rootMatch.Groups["Body"].Value.Trim();

        var passMatch = regex_effect_pass.Match(rootBody);

        do
        {
            if (!passMatch.Success)
                break;

            var methods = new List<EffectMethodInfo>();
            var structs = new List<EffectStructInfo>();
            var members = new List<EffectMemberInfo>();
            var directives = new List<EffectDirectiveInfo>();

            string passName = passMatch.Groups["Name"].Value.Trim();
            string passBody = passMatch.Groups["Body"].Value.Trim();

            var methodMatch = regex_effect_method.Match(passBody);

            do
            {
                if (!methodMatch.Success)
                    break;

                methods.Add(new EffectMethodInfo
                (
                    methodMatch.Groups["Name"].Value.Trim(),
                    methodMatch.Groups["Type"].Value.Trim(),
                    methodMatch.Value.Trim(),
                    string.IsNullOrEmpty(methodMatch.Groups["Args"].Value)
                        ? Array.Empty<EffectIdentifierInfo>()
                        : methodMatch.Groups["Args"].Value.Trim().Split(',').Select(p => p.Split(' ')).Select(p => new EffectIdentifierInfo(p[1], p[0])).ToArray()
                ));
            }
            while ((methodMatch = methodMatch.NextMatch()).Success);

            var structMatch = regex_effect_struct.Match(passBody);

            do
            {
                if (!structMatch.Success)
                    break;

                var structMembers = new List<EffectStructMemberInfo>();
                var structMemberMatch = regex_effect_struct_member.Match(structMatch.Groups["Body"].Value.Trim());

                do
                {
                    if (!structMemberMatch.Success)
                        break;

                    structMembers.Add(new EffectStructMemberInfo
                    (
                        structMemberMatch.Groups["Name"].Value.Trim(),
                        structMemberMatch.Groups["Type"].Value.Trim(),
                        structMemberMatch.Groups["Size"].Value.Trim()
                    ));
                }
                while ((structMemberMatch = structMemberMatch.NextMatch()).Success);

                structs.Add(new EffectStructInfo
                (
                    structMatch.Groups["Name"].Value.Trim(),
                    structMembers.ToArray()
                ));
            }
            while ((structMatch = structMatch.NextMatch()).Success);

            var memberMatch = regex_effect_member.Match(passBody);

            do
            {
                if (!memberMatch.Success)
                    break;

                members.Add(new EffectMemberInfo
                (
                    memberMatch.Groups["Name"].Value.Trim(),
                    memberMatch.Groups["Type"].Value.Trim(),
                    memberMatch.Groups["Size"].Value.Trim(),
                    memberMatch.Groups["Qualifier"].Value.Trim()
                ));
            }
            while ((memberMatch = memberMatch.NextMatch()).Success);

            var directiveMatch = regex_effect_directive.Match(passBody);

            do
            {
                if (!directiveMatch.Success)
                    break;

                directives.Add(new EffectDirectiveInfo
                (
                    directiveMatch.Groups["Key"].Value.Trim(),
                    directiveMatch.Groups["Value"].Value.Trim()
                ));
            }
            while ((directiveMatch = directiveMatch.NextMatch()).Success);

            passes.Add(new EffectDocumentPass
            (
                passName,
                methods.ToArray(),
                structs.ToArray(),
                members.ToArray(),
                directives.ToArray()
            ));
        }
        while ((passMatch = passMatch.NextMatch()).Success);

        return new EffectDocument(rootName, passes.ToArray());
    }

    private static readonly Regex regex_effect_root = new(@"^\s*effect\s+""(?<Name>(?<="").*?(?=""))""\s*{(?<Body>.*)}\s*$", RegexOptions.Singleline);
    private static readonly Regex regex_effect_pass = new(@"pass\s+""(?<Name>(?<="").*?(?=""))""\s*{(?<Body>.*)}", RegexOptions.Singleline);
    private static readonly Regex regex_effect_method = new(@"(?<Type>\w+)\s+(?<Name>\w+)\s*\((?<Args>[^\(\)]*)\)\s*\{(?>\{(?<c>)|[^{}]+|\}(?<-c>))*(?(c)(?!))\}", RegexOptions.Singleline);
    private static readonly Regex regex_effect_struct = new(@"struct\s+(?<Name>\w+)\s*{\s*(?<Body>.*)}", RegexOptions.Singleline);
    private static readonly Regex regex_effect_member = new(@"\s*(?<Qualifier>\w+)\s+(?<Type>\w+)\s+(?<Name>\w+)(?:\[(?<Size>\d*)\])?;", RegexOptions.Multiline);
    private static readonly Regex regex_effect_directive = new(@"^#(?<Key>\w+)\s*(?<Value>.+)$", RegexOptions.Multiline);
    private static readonly Regex regex_effect_struct_member = new(@"\s*(?<Type>\w+)\s+(?<Name>\w+)(?:\[(?<Size>\d*)\])?;", RegexOptions.Multiline);
}
