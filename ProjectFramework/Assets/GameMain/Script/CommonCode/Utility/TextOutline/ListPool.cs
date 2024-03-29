﻿using System;
using System.Collections.Generic;
using UnityEngine;

static class ListPool<T>
{
    // Object pool to avoid allocations.
    private static readonly TextOutlineObjectPool<List<T>> s_ListPool = new TextOutlineObjectPool<List<T>>(null, l => l.Clear());

    public static List<T> Get()
    {
        return s_ListPool.Get();
    }

    public static void Release(List<T> toRelease)
    {
        s_ListPool.Release(toRelease);
    }
}
