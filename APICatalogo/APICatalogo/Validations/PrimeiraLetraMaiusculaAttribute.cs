using System.ComponentModel.DataAnnotations;

namespace ApiCatalogo.Validations
{
    public class PrimeiraLetraMaiusculaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext) // ValidationContext é o contexto de validação
        {
            if (value == null || string.IsNullOrEmpty(value.ToString())) // se o valor for nulo ou vazio, retorna sucesso
            {
                return ValidationResult.Success;
            }
            var primeiraLetra = value.ToString()[0].ToString(); // pega a primeira letra do valor
            if (primeiraLetra != primeiraLetra.ToUpper()) // se a primeira letra não for maiúscula, retorna erro
            {
                return new ValidationResult("A primeira letra do nome do produto deve ser maiúscula");
            }

            return ValidationResult.Success;
        }
    }
}
