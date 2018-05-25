using Domain.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Dictionary
{
    public class DictionaryLanguage : BaseEntity, IEntityGeneral
    {
        public string Language { get; set; }
        public bool IsSelected { get; set; }

        public string  DictionaryId { get; set; }
        public TwitterDictionary Dictionary { get; set; }
    }
}
