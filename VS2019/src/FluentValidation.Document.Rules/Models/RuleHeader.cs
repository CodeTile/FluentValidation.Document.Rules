using System.Collections.Generic;

namespace FluentValidation.Document.Rules.Models
{
    public class RuleHeader
    {
        public RuleHeader()
        {
            RuleDetails = new List<RuleDetail>();
        }

        public string Expression { get; internal set; }
        public string ModelType { get; internal set; }
        public string PropertyName { get; internal set; }
        public string PropertyType { get; internal set; }
        public List<RuleDetail> RuleDetails { get; set; }
    }
}