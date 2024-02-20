using System.Collections.Generic;

namespace Colosoft.Globalization
{
    public interface IMultipleTranslateProvider : ITranslateProvider
    {
        IEnumerable<TranslateInfo> GetTranslates(object groupKey);
    }
}
