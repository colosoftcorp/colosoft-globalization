using System;

namespace Colosoft.Globalization
{
    public class TranslateInfo
    {
        public int Value
        {
            get
            {
                if (this.Key != null)
                {
                    var keyType = this.Key.GetType();

                    if (keyType.IsEnum)
                    {
                        keyType = Enum.GetUnderlyingType(keyType);
                    }

                    if (keyType == typeof(int))
                    {
                        return (int)this.Key;
                    }
                    else if (keyType == typeof(short))
                    {
                        return (short)this.Key;
                    }
                    else if (keyType == typeof(long))
                    {
                        return (int)(long)this.Key;
                    }
                    else if (keyType == typeof(uint))
                    {
                        return (int)(uint)this.Key;
                    }
                    else if (keyType == typeof(ulong))
                    {
                        return (int)(ulong)this.Key;
                    }
                    else if (keyType == typeof(ushort))
                    {
                        return (ushort)this.Key;
                    }
                    else if (keyType == typeof(byte))
                    {
                        return (byte)this.Key;
                    }
                    else
                    {
                        return 0;
                    }
                }

                return 0;
            }
        }

        public object Key { get; set; }

        public IMessageFormattable Text { get; set; }

        public string Translation => this.Text.Format(System.Globalization.CultureInfo.CurrentCulture);

        public TranslateInfo(object key, IMessageFormattable text)
        {
            this.Key = key;
            this.Text = text;
        }
    }
}
