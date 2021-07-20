using System;
using System.Collections;
using System.Linq;
using System.Reflection;
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
            var extractedRulesforAssembly = new ArrayList();
            ExtractRulesForAssembly(assemblyPath);
        }

        private static ArrayList ExtractRulesForAssembly(string assemblyPath)
        {
            var validatorsForAssembly = new ArrayList();
            var assembly = Assembly.LoadFrom(assemblyPath);
            foreach (Type validatorType in assembly.GetTypes().Where(m => m.BaseType.Name.StartsWith("AbstractValidator")))
            {
                var rulesForValidator = new ArrayList();
                var validatorClass = Activator.CreateInstance(validatorType, true);
                var rules = (IEnumerable)Helper.Refelction.RefelctInstanceProperty(validatorClass, "Rules");
                var _innerCollection = (IEnumerable)Helper.Refelction.ReflectInstanceField(rules, "_innerCollection");

                foreach (IValidationRule rule in _innerCollection)
                {
                    var member = rule.Member;
                    var rh = new RuleHeader()
                    {
                        ValidatorFullName = validatorType.FullName,
                        ModelType = rule.Member.DeclaringType.FullName,
                        PropertyName = rule.Member.Name,
                        PropertyType = rule.TypeToValidate.FullName,
                        Expression = rule.Expression.ToString(),
                        RuleDetails = new ArrayList(),
                    };
                    foreach (FluentValidation.Internal.IRuleComponent component in rule.Components)
                    {
                        rh.RuleDetails.Add(new RuleDetail()
                        {
                            ErrorMessage = component.GetUnformattedErrorMessage(),
                            ComponentValidator = component.Validator,
                        });
                    }
                    rulesForValidator.Add(rh);
                }
                validatorsForAssembly.Add(rulesForValidator);
            }

            return validatorsForAssembly;
        }
    }
}