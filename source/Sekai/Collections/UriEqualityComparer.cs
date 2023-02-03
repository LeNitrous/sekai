// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System;

namespace Sekai.Collections;

public class UriEqualityComparer : EqualityComparer<Uri>
{
    public override bool Equals(Uri? x, Uri? y)
    {
        return x == y;
    }

    public override int GetHashCode([DisallowNull] Uri obj)
    {
        return obj.GetHashCode();
    }
}
