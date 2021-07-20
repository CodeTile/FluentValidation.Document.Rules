using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Validators;

namespace FluentValidation.Document.Rules.Models
{
    public class RuleAssembly
    {
        public RuleAssembly()
        {
            Validators = new List<RuleValidator>();
        }

        public string AssemblyFullPath { get; set; }

        public string AssemblyName
        {
            get
            {
                return AssemblyFullPath.Split("\\").LastOrDefault()
                       .Replace(AssemblyFullPath.Split('.').LastOrDefault(), "");
            }
        }

        public List<RuleValidator> Validators { get; set; }
    }

    public class RuleDetail
    {
        public IPropertyValidator ComponentValidator { get; internal set; }
        public string ErrorMessage { get; set; }
    }

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