namespace FluentValidation.Document.Rules.Models
{
    public class RuleAssembly
    {
        public RuleAssembly()
        {
            Validators = [];
        }

        public FileInfo? AssemblyInfo { get; set; }

        public List<RuleValidator> Validators { get; set; }
    }
}