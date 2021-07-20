using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;

namespace FluentValidation.Document.Rules.Models
{
    public class RuleDetail
    {
        public IPropertyValidator ComponentValidator { get; internal set; }
        public string ErrorMessage { get; set; }
    }

    public class RuleHeader
    {
        public string Expression { get; internal set; }
        public string ModelType { get; internal set; }
        public string PropertyName { get; internal set; }
        public string PropertyType { get; internal set; }
        public ArrayList RuleDetails { get; set; }
        public string ValidatorFullName { get; internal set; }
    }
}