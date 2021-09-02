using System.Collections.Generic;
using System.IO;

namespace FluentValidation.Document.Rules.Models
{
    public class RuleAssembly
    {
        public RuleAssembly()
        {
            Validators = new List<RuleValidator>();
        }

        public FileInfo AssemblyInfo { get; set; }

        public List<RuleValidator> Validators { get; set; }
    }
}