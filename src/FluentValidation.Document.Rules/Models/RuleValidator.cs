using System.Collections.Generic;
using System.Linq;

namespace FluentValidation.Document.Rules.Models
{
    public class RuleValidator
    {
        public RuleValidator()
        {
            Rules = new List<RuleHeader>();
        }

        public List<RuleHeader> Rules { get; set; }
        public string ValidatorFullName { get; internal set; }

        public string ValidatorName
        {
            get
            {
                return ValidatorFullName.Split("\\").LastOrDefault();
                // .Replace(ValidatorFullName.Split('.').LastOrDefault(), "");
            }
        }
    }
}