using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentValidation.Document.Rules.enums;
using FluentValidation.Document.Rules.Models;
using FluentValidation.Validators;

namespace FluentValidation.Document.Rules
{
    public class DocumentRules : IDisposable
    {
        public void Dispose()
        {
        }

        public void DocumentAssembly(OutputType outputType = OutputType.Markdown)
        {
            DocumentAssembly(Helper.Settings.AssemblyToDocument, outputType);
        }

        public void DocumentAssembly(string assemblyPath, OutputType outputType = OutputType.Markdown)
        {
            Console.WriteLine($"Documenting Assembly : {Helper.Settings.AssemblyToDocument}");
            DocumentRulesForAssembly(ExtractRulesForAssembly(assemblyPath), outputType);
        }

        private static RuleAssembly ExtractRulesForAssembly(string assemblyPath)
        {
            var validatorsForAssembly = new RuleAssembly()
            {
                AssemblyInfo = new FileInfo(assemblyPath),
            };
            var assembly = Assembly.LoadFrom(assemblyPath);
            Console.WriteLine($"    Interrogating '{validatorsForAssembly.AssemblyInfo.Name}'");
            var possibleValidators = assembly.GetTypes().Where(m => m.BaseType != null)?.OrderBy(m => m.FullName);

            foreach (Type validatorType in possibleValidators.Where(m => m.BaseType.Name.Contains("AbstractValidator")))
            {
                var rulesForValidator = new RuleValidator()
                {
                    ValidatorFullName = validatorType.FullName,
                };
                Console.WriteLine($"      |--- {rulesForValidator.ValidatorName}");
                var validatorClass = Activator.CreateInstance(validatorType, true);
                var rules = (IEnumerable)Helper.Refelction.RefelctInstanceProperty(validatorClass, "Rules");
                var _innerCollection = (IEnumerable)Helper.Refelction.ReflectInstanceField(rules, "_innerCollection");

                foreach (IValidationRule rule in _innerCollection)
                {
                    var member = rule.Member;
                    var rh = new RuleHeader()
                    {
                        ModelType = rule.Member.DeclaringType.FullName,
                        PropertyName = rule.Member.Name,
                        PropertyType = ResolveTypeName(rule.TypeToValidate),
                        Expression = rule.Expression.ToString(),
                        RuleDetails = new List<RuleDetail>(),
                    };

                    foreach (FluentValidation.Internal.IRuleComponent component in rule.Components)
                    {
                        var rd = new RuleDetail()
                        {
                            ErrorMessage = component.GetUnformattedErrorMessage(),
                            ComponentValidator = component.Validator,
                        };
                        var pv = rd.ComponentValidator;

                        if (pv is IComparisonValidator)
                        {
                            var a = (IComparisonValidator)pv;
                            rd.ValidatiorType = a.Comparison.ToString();
                            if (a.ValueToCompare != null)
                                rd.ValueToCompare = a.ValueToCompare?.ToString()?.Replace("00:00:00", string.Empty);
                            else if (a.MemberToCompare != null)
                            {
                                string sign = "";
                                if (pv is IGreaterThanOrEqualValidator)
                                    sign = " >= ";
                                else if (pv is ILessThanOrEqualValidator)
                                    sign = " <= ";
                                else if (pv.Name.Equals("GreaterThanValidator"))
                                    sign = " > ";
                                else if (pv.Name.Equals("LessThanValidator"))
                                    sign = " < ";
                                else
                                    throw new NotImplementedException();

                                rd.ValueToCompare = $"{sign} {a.MemberToCompare.Name}";
                            }
                        }
                        else if (pv is IExactLengthValidator)
                        {
                            var c = (IExactLengthValidator)pv;
                            rd.ValidatiorType = c.Name;
                            rd.ValueToCompare = c.Min.ToString();
                        }
                        else if (pv is ILengthValidator)
                        {
                            var b = (ILengthValidator)pv;
                            rd.ValueToCompare = $"{b.Min} >= Value <= {b.Max} ";
                            rd.ValidatiorType = b.Name;
                        }
                        else if (pv is INotNullValidator)
                            rd.ValidatiorType = pv.Name;
                        else if (pv is INotEmptyValidator)
                            rd.ValidatiorType = pv.Name;
                        rh.RuleDetails.Add(rd);
                    }

                    rulesForValidator.Rules.Add(rh);
                }

                validatorsForAssembly.Validators.Add(rulesForValidator);
            }

            return validatorsForAssembly;
        }

        private static string ResolveTypeName(Type dataType)
        {
            if (!dataType.FullName.Contains("`"))
                return dataType.FullName;
            //////////

            return ResolveTypeName(dataType.ToString());
        }

        private static string ResolveTypeName(string dataType)
        {
            var retval = dataType;
            retval = retval.Replace("System.Collections.Generic.", "");
            retval = retval.Replace("System.Collections.", "");
            retval = retval.Replace("System.", "");
            retval = retval.Replace("[", "< ");
            retval = retval.Replace("]", ">");
            retval = retval.Replace(",", ", ");
            ;
            for (int i = 0; i < 15; i++)
            {
                retval = retval.Replace($"`{i}<", "<");
            }
            return retval;
        }

        private void DocumentRulesForAssembly(RuleAssembly rulesForAssembly, OutputType outputType)
        {
            switch (outputType)
            {
                case OutputType.Markdown:
                    GenerateForAssemblyMarkdown(rulesForAssembly);
                    break;

                case OutputType.HTML:
                    GenerateForAssemblyHTML(rulesForAssembly);
                    break;

                case OutputType.All:
                    GenerateForAssemblyHTML(rulesForAssembly);
                    GenerateForAssemblyMarkdown(rulesForAssembly);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void GenerateForAssemblyHTML(RuleAssembly rulesForAssembly)
        {
            Console.WriteLine("Documenting as HTML");
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<style>");
            sb.AppendLine("	.table {");
            sb.AppendLine("	  font-family: Arial, Helvetica, sans-serif;");
            sb.AppendLine("	  border-collapse: collapse;");
            sb.AppendLine("	  width: 100%;");
            sb.AppendLine("	}");
            sb.AppendLine("");
            sb.AppendLine("	.table td, #customers th {");
            sb.AppendLine("	  border: 1px solid #ddd;");
            sb.AppendLine("	  padding: 8px;");
            sb.AppendLine("	}");
            sb.AppendLine("");
            sb.AppendLine("	#customers tr:nth-child(even){background-color: #f2f2f2;}");
            sb.AppendLine("");
            sb.AppendLine("	.table tr:hover {background-color: #ddd;}");
            sb.AppendLine("");
            sb.AppendLine("	.table th {");
            sb.AppendLine("	  padding-top: 12px;");
            sb.AppendLine("	  padding-bottom: 12px;");
            sb.AppendLine("	  text-align: left;");
            sb.AppendLine("	  background-color: #04AA6D;");
            sb.AppendLine("	  color: white;");
            sb.AppendLine("	}");
            sb.AppendLine("</style>");
            sb.AppendLine("    <head>");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"styles.css\">");
            sb.AppendLine("        <meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />");
            sb.AppendLine($"       <title>{rulesForAssembly.AssemblyInfo.NameNoExtension()}</title>");
            sb.AppendLine("    </head>");
            sb.AppendLine("<body>");
            sb.AppendLine($"       <h1>{rulesForAssembly.AssemblyInfo.NameNoExtension()}</h1>");
            sb.AppendLine("<ol>");
            foreach (RuleValidator v in rulesForAssembly.Validators)
            {
                sb.AppendLine($"<a href=\"#{v.ValidatorName}\"><li>{v.ValidatorName}</li><a>");
            }
            sb.AppendLine("</ol>");

            foreach (RuleValidator validator in rulesForAssembly.Validators)
            {
                Console.WriteLine($"    {validator.ValidatorName}");
                sb.AppendLine($"  <h2>{validator.ValidatorName}</h2>");
                sb.AppendLine($"<table  class=\"table\"  id=\"{validator.ValidatorName}\">");
                sb.AppendLine($"<tr><th>Field </th><th> DataType  </th><th>  Model  </th><th>  LINQ Expression  </th><th>  Rule Count  </th><th>  Validator </th><th>  ValidationValue  </th><th>  Error Message</th></tr>");

                foreach (RuleHeader rh in validator.Rules)
                {
                    Console.WriteLine($"      |---{rh.PropertyName}");
                    string line = $"<tr><td> {rh.PropertyName} </td><td> {rh.PropertyType} </td><td>  {rh.ModelType} </td><td>  {rh.Expression} </td><td>  {rh.RuleDetails.Count} </td><td>   ";
                    foreach (RuleDetail rd in rh.RuleDetails)
                    {
                        sb.AppendLine($"{line} {rd.ValidatiorType}</td><td> {rd.ValueToCompare }</td><td>  {rd.ErrorMessage} </td></tr>");
                        line = "<tr><td colspan=\"5\">&nbsp</td><td>  ";
                    }
                }
                sb.AppendLine("</table>");
            }
            sb.AppendLine("");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            var filePath = Path.Combine(Helper.Settings.ExportFolder.Markdown, rulesForAssembly.AssemblyInfo.NameNoExtension() + ".html");
            var fi = new FileInfo(filePath);
            if (fi.Exists)
                fi.Delete();
            File.WriteAllText(filePath, sb.ToString());
        }

        private void GenerateForAssemblyMarkdown(RuleAssembly rulesForAssembly)
        {
            Console.WriteLine("Documenting as Markdown");
            var sb = new StringBuilder();
            sb.AppendLine($"## {rulesForAssembly.AssemblyInfo.NameNoExtension()}  ");
            sb.AppendLine("----  ");
            foreach (RuleValidator validator in rulesForAssembly.Validators)
            {
                Console.WriteLine($"    {validator.ValidatorName}");
                sb.AppendLine($"### {validator.ValidatorName}");

                sb.AppendLine("| Field | DataType | Model | LINQ Expression | Rule Count | Validator| ValidationValue | Error Message | ");
                sb.AppendLine("|---|---|---|---|---|---|---|---|  ");
                foreach (RuleHeader rh in validator.Rules)
                {
                    Console.WriteLine($"      |---{rh.PropertyName}");
                    string line = $"| {rh.PropertyName} | {rh.PropertyType} | {rh.ModelType} | {rh.Expression} | {rh.RuleDetails.Count} |  ";
                    foreach (RuleDetail rd in rh.RuleDetails)
                    {
                        sb.AppendLine($"{line} {rd.ValidatiorType}|{rd.ValueToCompare }| {rd.ErrorMessage} |");
                        line = "||||||";
                    }
                }
            }

            var filePath = Path.Combine(Helper.Settings.ExportFolder.Markdown, rulesForAssembly.AssemblyInfo.NameNoExtension() + ".md");
            var fi = new FileInfo(filePath);
            if (fi.Exists)
                fi.Delete();
            File.WriteAllText(filePath, sb.ToString());
        }
    }
}