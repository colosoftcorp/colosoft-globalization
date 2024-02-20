using System;
using System.Collections.Generic;

namespace Colosoft.Globalization
{
    public interface ITranslateProvider
    {
        IEnumerable<TranslateInfo> GetTranslates();
    }
}
