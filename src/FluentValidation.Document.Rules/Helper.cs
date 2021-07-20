using System;
using System.Reflection;

namespace FluentValidation.Document.Rules
{
    public static class Helper
    {
        public static class Refelction
        {
            private const BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Default;

            internal static object RefelctInstanceField(object instance, string fieldName, Type type)
            {
                var field = type.GetField(fieldName, bindFlags);
                return field?.GetValue(instance);
            }

            internal static object RefelctInstanceProperty(object instance, string propertyName)
            {
                string masterPropertyName = propertyName;
                if (propertyName.Contains("."))
                {
                    masterPropertyName = propertyName;
                    propertyName = masterPropertyName.Split('.')[0];
                }
                var fieldValue = instance.GetType().GetProperty(propertyName, bindFlags)?.GetValue(instance);
                if (masterPropertyName.Contains("."))
                {
                    masterPropertyName = masterPropertyName.Substring(propertyName.Length + 1);
                    fieldValue = RefelctInstanceProperty(fieldValue, masterPropertyName);
                }

                return fieldValue;
            }

            internal static object ReflectInstanceField(object instance, string fieldName)
            {
                var t = instance.GetType();
                return RefelctInstanceField(instance, fieldName, t);
            }
        }
    }
}