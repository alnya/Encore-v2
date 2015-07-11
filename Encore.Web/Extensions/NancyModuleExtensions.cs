namespace Encore.Web.Extensions
{
    using Nancy;
    using System.Collections;
    using Nancy.ModelBinding;
    using Nancy.Validation;
    using System;

    public static class NancyModuleExtensions
    {
        public static TModel BindAndValidateEnumerable<TModel>(this INancyModule module) where TModel : IEnumerable
        {
            var boundEnumerable = module.Bind<TModel>();
            module.ValidateEnumerable(boundEnumerable);
            return boundEnumerable;
        }

        public static ModelValidationResult ValidateEnumerable(this INancyModule module, IEnumerable enumerable)
        {
            IModelValidator validator = null;

            int row = 0;

            foreach (var model in enumerable)
            {
                row++;

                validator = validator ?? module.ValidatorLocator.GetValidatorForType(model.GetType());
                var result = validator.Validate(model, module.Context);

                foreach (var resultError in result.Errors)
                {
                    module.ModelValidationResult.Errors.Add(
                        String.Format("{0} {1}", resultError.Key, row), resultError.Value);
                }
            }

            return module.ModelValidationResult;
        }
    }
}
