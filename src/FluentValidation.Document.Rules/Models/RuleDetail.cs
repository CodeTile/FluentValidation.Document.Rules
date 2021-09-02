using FluentValidation.Validators;

namespace FluentValidation.Document.Rules.Models
{
    public class RuleDetail
    {
        public IPropertyValidator ComponentValidator { get; internal set; }
        public string ErrorMessage { get; set; }
        public string ValidatiorType { get; set; }
        public string ValueToCompare { get; set; }
    }
}