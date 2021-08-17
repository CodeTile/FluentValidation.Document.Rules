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

        public void DocumentAssembly()
        {
            DocumentAssembly(Helper.Settings.AssemblyToDocument);
        }

        public void DocumentAssembly(string assemblyPath)
        {
            Console.WriteLine($"Documenting Assembly : {Helper.Settings.AssemblyToDocument}");
            //var extractedRulesforAssembly = new ArrayList();
            // ExtractRulesForAssembly(assemblyPath);
            DocumentRulesForAssembly(ExtractRulesForAssembly(assemblyPath), OutputType.Markdown);
        }

        private static RuleAssembly ExtractRulesForAssembly(string assemblyPath)
        {
            var validatorsForAssembly = new RuleAssembly()
            {
                AssemblyFullPath = assemblyPath,
            };
            var assembly = Assembly.LoadFrom(assemblyPath);
            foreach (Type validatorType in assembly.GetTypes().Where(m => m.BaseType.Name.StartsWith("AbstractValidator")).OrderBy(m=>m.FullName) )
            {
                var rulesForValidator = new RuleValidator()
                {
                    ValidatorFullName = validatorType.FullName,
                };
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
                        rh.RuleDetails.Add(new RuleDetail()
                        {
                            ErrorMessage = component.GetUnformattedErrorMessage(),
                            ComponentValidator = component.Validator,
                        });
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

                default:
                    throw new NotImplementedException();
            }
        }

        private void GenerateForAssemblyMarkdown(RuleAssembly rulesForAssembly)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"## {rulesForAssembly.AssemblyName}  ");
            sb.AppendLine("----  ");
            foreach (RuleValidator validator in rulesForAssembly.Validators)
            {
                sb.AppendLine($"### {validator.ValidatorName}");

                sb.AppendLine("| Field | DataType | Model | LINQ Expression | Rule Count | Validator| ValidationValue | Error Message | ");
                sb.AppendLine("|---|---|---|---|---|---|---|---|  ");
                foreach (RuleHeader rh in validator.Rules)
                {
                    string line = $"| {rh.PropertyName} | {rh.PropertyType} | {rh.ModelType} | {rh.Expression} | {rh.RuleDetails.Count} |  ";
                    foreach (RuleDetail rd in rh.RuleDetails)
                    {

                        var pv = rd.ComponentValidator;
                        string errorMessage = rd.ErrorMessage;
                        string validatiorType = string.Empty;
                        string valueToCompare = string.Empty;
                        if (pv is IComparisonValidator)
                        {
                            var a = (IComparisonValidator)pv;
                            validatiorType = a.Comparison.ToString();
                            if (a.ValueToCompare != null)
                                valueToCompare = a.ValueToCompare?.ToString()?.Replace("00:00:00", string.Empty);
                            else if (a.MemberToCompare != null)
                            {
                                string sign = "";
                                sign = pv switch
                                {
                                    IGreaterThanOrEqualValidator => " >= ",
                                    ILessThanOrEqualValidator => " <= ",
                                    _ => throw new NotImplementedException(),
                                };
                                valueToCompare = $"{sign} {a.MemberToCompare.Name}";
                            }
                        }
                        else if (pv is IExactLengthValidator)
                        {
                            var c = (IExactLengthValidator)pv;
                            validatiorType = c.Name;
                            valueToCompare = c.Min.ToString();
                        }
                        else if (pv is ILengthValidator)
                        {
                            var b = (ILengthValidator)pv;
                            valueToCompare = $"{b.Min} >= Value <= {b.Max} ";
                            validatiorType = b.Name;
                        }
                        else if (pv is INotNullValidator)
                            validatiorType = pv.Name;
                        else if (pv is INotEmptyValidator)
                            validatiorType = pv.Name;
                        sb.AppendLine($"{line} {validatiorType}|{valueToCompare }| {errorMessage} |");
                        line = "||||||";
                    }
                }
            }

            var filePath = Path.Combine(Helper.Settings.ExportFolder.Markdown, rulesForAssembly.AssemblyName + "md");
            var fi = new FileInfo(filePath);
            if (fi.Exists)
                fi.Delete();
            File.WriteAllText(filePath, sb.ToString());
        }
    }
}