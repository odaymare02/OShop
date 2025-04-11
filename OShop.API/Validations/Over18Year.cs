using System.ComponentModel.DataAnnotations;

namespace OShop.API.Validations
{
    public class Over18Year:ValidationAttribute
    {
        private readonly int _age;
        public Over18Year(int age)
        {
            _age = age;
        }
        public override bool IsValid(object? value)
        {
            if(value is DateTime soso)
            {
                return ((DateTime.Now.Year - soso.Year) >= _age);
            }

                return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be at least {_age} years old";
        }
    }
}
