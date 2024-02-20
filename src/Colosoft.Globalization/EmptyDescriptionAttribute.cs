using System;
using System.Collections.Generic;
using System.Linq;

namespace Colosoft
{
    [AttributeUsage(AttributeTargets.All)]
    public class EmptyDescriptionAttribute : Attribute
    {
        public string Description { get; set; }

        public EmptyDescriptionAttribute()
        {
        }

        public EmptyDescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}
