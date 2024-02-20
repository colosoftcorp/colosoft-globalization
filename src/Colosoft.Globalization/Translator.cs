using System;
using System.Collections.Generic;
using System.Linq;

namespace Colosoft
{
    public static class Translator
    {
        public static IEnumerable<Globalization.TranslateInfo> GetTranslates<T>()
        {
            return GetTranslates(typeof(T));
        }

        public static IEnumerable<Globalization.TranslateInfo> GetTranslates(Type type)
        {
            return GetTranslates(type, null);
        }

        public static IEnumerable<Globalization.TranslateInfo> GetTranslates(Type type, object groupKey)
        {
            if (type == null)
            {
                yield break;
            }

            var translateAttribute = (TranslateAttribute)type.GetCustomAttributes(typeof(TranslateAttribute), false).FirstOrDefault();

            if (type.IsEnum)
            {
                object provider = null;

                if (translateAttribute != null &&
                    translateAttribute.ProviderType != null)
                {
                    try
                    {
                        provider = type
                            .GetCustomAttributes(typeof(TranslateAttribute), false)
                            .Select(f => Activator.CreateInstance(((TranslateAttribute)f).ProviderType))
                            .FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            $"Não foi possível carregar o provedor de tradução para o tipo '{type}'", ex);
                    }
                }

                if (provider is Globalization.ITranslateProvider provider1)
                {
                    IEnumerable<Globalization.TranslateInfo> translates = null;

                    if (provider is Globalization.IMultipleTranslateProvider provider2)
                    {
                        translates = provider2.GetTranslates(groupKey);
                    }
                    else
                    {
                        translates = provider1.GetTranslates();
                    }

                    foreach (var i in translates)
                    {
                        yield return i;
                    }

                    yield break;
                }

                foreach (var field in type.GetFields().Where(f => f.FieldType.IsEnum))
                {
                    if (field.GetCustomAttributes(typeof(IgnoreTranslateOptionAttribute), false).Any())
                    {
                        continue;
                    }

                    yield return new Globalization.TranslateInfo(
                        field.GetValue(null),
                        (field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                            .Select(f => ((System.ComponentModel.DescriptionAttribute)f).Description)
                            .FirstOrDefault() ?? field.Name).GetFormatter());
                }
            }
        }

        public static IEnumerable<Globalization.TranslateInfo> GetTranslatesFromTypeName(string typeName)
        {
            return GetTranslates(Type.GetType(typeName, true));
        }

        public static IEnumerable<Globalization.TranslateInfo> GetTranslatesFromTypeName(string typeName, object groupKey)
        {
            return GetTranslates(Type.GetType(typeName, true), groupKey);
        }

        public static IMessageFormattable Translate(this object value)
        {
            return Translate(value, null);
        }

        public static IMessageFormattable Translate(this object value, object groupKey)
        {
            if (value == null)
            {
                return MessageFormattable.Empty;
            }

            var valueType = value.GetType();

            // Verifica se o tipo e um enumerador
            if (valueType.IsEnum)
            {
                var translateAttribute = (TranslateAttribute)valueType.GetCustomAttributes(typeof(TranslateAttribute), false).FirstOrDefault();

                object provider = null;

                if (translateAttribute != null &&
                    translateAttribute.ProviderType != null)
                {
                    try
                    {
                        provider = valueType
                            .GetCustomAttributes(typeof(TranslateAttribute), false)
                            .Select(f => Activator.CreateInstance(((TranslateAttribute)f).ProviderType))
                            .FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            $"Não foi possível carregar o provedor de tradução para o tipo '{valueType}'", ex);
                    }
                }

                IEnumerable<Globalization.TranslateInfo> translates = null;

                if (value != null)
                {
                    IEnumerable<System.Reflection.FieldInfo> fields = null;

                    var isFlags = valueType.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0;

                    long[] flags = null;

                    if (isFlags)
                    {
                        flags = ((Enum)value).GetIndividualFlags().ToArray();
                    }

                    fields = valueType.GetFields().Where(f =>
                        {
                            if (f.FieldType.IsEnum)
                            {
                                var fieldValue = f.GetValue(null);

                                if (isFlags)
                                {
                                    return flags.Contains(Convert.ToInt64((Enum)fieldValue, System.Globalization.CultureInfo.InvariantCulture));
                                }

                                return fieldValue.ToString() == value.ToString();
                            }

                            return false;
                        });

                    if (provider is Globalization.ITranslateProvider provider1)
                    {
                        if (translates == null)
                        {
                            if (provider is Globalization.IMultipleTranslateProvider provider2)
                            {
                                translates = provider2.GetTranslates(groupKey);
                            }
                            else
                            {
                                translates = provider1.GetTranslates();
                            }
                        }

                        var texts = fields.Select(field =>
                            translates
                                .Where(f => field.Name.Equals((f.Key ?? string.Empty).ToString()))
                                .Select(f => f.Text)
                                .FirstOrDefault())
                            .Where(f => f != null);

                        return texts.Join(", ");
                    }
                    else
                    {
                        var texts = fields.Select(field =>
                            (field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                                .Select(f => ((System.ComponentModel.DescriptionAttribute)f).Description)
                                .FirstOrDefault() ?? field.Name).GetFormatter());

                        return texts.Join(", ");
                    }
                }

                var emptyDescription = valueType
                    .GetCustomAttributes(typeof(EmptyDescriptionAttribute), false) as EmptyDescriptionAttribute[];

                if (emptyDescription != null && emptyDescription.Length > 0)
                {
                    return emptyDescription[0].Description.GetFormatter();
                }
            }

            return MessageFormattable.Empty;
        }
    }
}
