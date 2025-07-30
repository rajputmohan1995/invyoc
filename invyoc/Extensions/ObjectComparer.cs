using System.Collections;

namespace invyoc.Extensions;

public class ObjectComparer
{
    public static bool AreEqual<T>(T obj1, T obj2)
    {
        if (obj1 == null || obj2 == null)
            return false;

        var type = typeof(T);
        foreach (var prop in type.GetProperties())
        {
            var val1 = prop.GetValue(obj1);
            var val2 = prop.GetValue(obj2);

            if (val1 == null && val2 == null)
                continue;
            if (val1 == null || val2 == null)
                return false;

            if (IsSimpleType(prop.PropertyType))
            {
                if (!object.Equals(val1, val2))
                    return false;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
            {
                // Compare lists or arrays
                if (!CompareEnumerables(val1 as IEnumerable, val2 as IEnumerable))
                    return false;
            }
            else
            {
                // Compare 1-level deep object
                foreach (var subProp in prop.PropertyType.GetProperties())
                {
                    var subVal1 = subProp.GetValue(val1);
                    var subVal2 = subProp.GetValue(val2);

                    if (!object.Equals(subVal1, subVal2))
                        return false;
                }
            }
        }

        return true;
    }

    private static bool CompareEnumerables(IEnumerable list1, IEnumerable list2)
    {
        if (list1 == null || list2 == null)
            return list1 == list2;

        var enum1 = list1.Cast<object>().ToList();
        var enum2 = list2.Cast<object>().ToList();

        if (enum1.Count != enum2.Count)
            return false;

        for (int i = 0; i < enum1.Count; i++)
        {
            var item1 = enum1[i];
            var item2 = enum2[i];

            if (item1 == null && item2 == null)
                continue;

            if (item1 == null || item2 == null)
                return false;

            var type = item1.GetType();
            if (IsSimpleType(type))
            {
                if (!object.Equals(item1, item2))
                    return false;
            }
            else
            {
                // Compare 1-level deep properties in list item
                foreach (var prop in type.GetProperties())
                {
                    var val1 = prop.GetValue(item1);
                    var val2 = prop.GetValue(item2);

                    if (!object.Equals(val1, val2))
                        return false;
                }
            }
        }

        return true;
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive ||
               type.IsEnum ||
               type.Equals(typeof(string)) ||
               type.Equals(typeof(decimal)) ||
               type.Equals(typeof(DateTime)) ||
               type.Equals(typeof(Guid));
    }
}