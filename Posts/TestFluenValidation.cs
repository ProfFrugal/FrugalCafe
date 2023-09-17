using FluentValidation.Results;
using FluentValidation;
using System.Collections.Generic;
using System;

namespace FrugalCafe.Posts
{
    public static class Helper
    {
        
    }

    public class Customer
    {
        public string Surname { get; set; }
        public string Forename { get; set; }
        public int Discount { get; set; }
        public bool HasDiscount { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }

        public static void Test()
        {
            var validator = new CustomerValidator();

            PerfTest.MeasurePerf(
                () => 
                {
                    var customer = new Customer();

                    customer.Surname = "Last";
                    customer.Forename = "First";

                    ValidationResult results = validator.Validate(customer);

                    // Inspect any validation failures.
                    bool success = results.IsValid;
                    List<ValidationFailure> failures = results.Errors;
                },
                "FluentValidation",
                1000 * 1000);
        }

        public static void SafeAdd<T>(ref List<T> list, T item)
        {
            if (list == null)
            {
                list = new List<T>(1);
            }

            list.Add(item);
        }

        public IReadOnlyList<string> Validate()
        {
            List<string> results = null;

            if (Surname == null)
            {
                SafeAdd(ref results, "Surname is empty");
            }

            if (Forename == null) 
            {
                SafeAdd(ref results, "Please specify a first name");
            }

            if (HasDiscount && (Discount == 0))
            {
                SafeAdd(ref results, "Discount is invalid");
            }

            if ((Address != null) && ((Address.Length < 20) || (Address.Length > 250)))
            {
                SafeAdd(ref results, "Address length is invalid");
            }

            if (!BeAValidPostcode(Postcode))
            {
                SafeAdd(ref results, "Please specify a valid postcode");
            }

            if (results == null)
            {
                return Array.Empty<string>();
            }

            return results;
        }

        private bool BeAValidPostcode(string postcode)
        {
            return true;
        }
    }

    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Surname).NotEmpty();
            RuleFor(x => x.Forename).NotEmpty().WithMessage("Please specify a first name");
            RuleFor(x => x.Discount).NotEqual(0).When(x => x.HasDiscount);
            RuleFor(x => x.Address).Length(20, 250);
            RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
        }

        private bool BeAValidPostcode(string postcode)
        {
            return true;
        }
    }
}
