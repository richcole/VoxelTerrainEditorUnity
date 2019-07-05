using System;
using System.Collections.Generic;
using UnityEngine;

public class FUtils
{

    public static bool All<T>(IEnumerable<T> lst, Func<T, bool> predicate)
    {
        foreach (T t in lst)
        {
            if (!predicate.Invoke(t))
            {
                return false;
            }
        }
        return true;
    }

    public static bool Any<T>(IEnumerable<T> lst, Func<T, bool> predicate)
    {
        foreach (T t in lst)
        {
            if (predicate.Invoke(t))
            {
                return true;
            }
        }
        return false;
    }

    public static void Elvis<T>(T t, Action<T> action)
    {
        if (t != null)
        {
            action.Invoke(t);
        }
    }

    public static S Elvis<S, T>(T t, Func<T, S> action) where S : class
    {
        if (t != null)
        {
            return action.Invoke(t);
        }
        return null;
    }

    internal static S ArgMin<S>(IEnumerable<S> lst, Func<S, float> fn) where S : class
    {
        float? minValue = null;
        S minArg = null;
        foreach (S s in lst)
        {
            float sVal = fn.Invoke(s);
            if (minValue == null || sVal < minValue.Value)
            {
                minValue = sVal;
                minArg = s;
            }
        }
        return minArg;
    }

    internal static S? ArgMinStruct<S>(IEnumerable<S> lst, Func<S, float> fn) where S : struct
    {
        float? minValue = null;
        S? minArg = null;
        foreach (S s in lst)
        {
            float sVal = fn.Invoke(s);
            if (minValue == null || sVal < minValue.Value)
            {
                minValue = sVal;
                minArg = s;
            }
        }
        return minArg;
    }

    public static List<T> FlatMap<S, T>(IEnumerable<S> lst, Func<S, List<T>> fn)
    {
        List<T> ret = new List<T>();
        foreach(S s in lst)
        {
            foreach(T t in fn.Invoke(s))
            {
                ret.Add(t);
            }
        }
        return ret;
    }

    public static List<T> Map<S, T>(IEnumerable<S> lst, Func<S, T> fn)
    {
        List<T> ret = new List<T>();
        foreach (S s in lst)
        {
            ret.Add(fn.Invoke(s));
        }
        return ret;
    }

    public static S? FirstThatStruct<S>(IEnumerable<S> lst, Func<S, Boolean> fn) where S: struct
    {
        foreach (S s in lst)
        {
            if (fn.Invoke(s))
            {
                return s;
            }
        }

        return null;
    }

    public static S FirstThatClass<S>(IEnumerable<S> lst, Func<S, Boolean> fn) where S : class
    {
        foreach (S s in lst)
        {
            if (fn.Invoke(s))
            {
                return s;
            }
        }

        return null;
    }
}
