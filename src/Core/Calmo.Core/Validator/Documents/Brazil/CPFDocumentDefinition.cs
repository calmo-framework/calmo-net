using System;
using System.Linq;

namespace Calmo.Core.Validator.Documents.Brazil
{
    public class CPFDocumentDefinition : DocumentDefinition
    {
        public override bool Validate(string value)
        {
            if (String.IsNullOrEmpty(value))
                return false;

            value = FormatValidation.Brazil.CPF.Unformat(value);

            if (value == "00000000000")
                return false;

            // Cria Objetos 
            var totalDigito1 = 0;
            var totalDigito2 = 0;

            // Limpa o CPF 
            var clearCpf = value.Trim(); // Elimina Espaços em Branco 
            clearCpf = clearCpf.Replace("-", ""); // Remove Separador de Dígito Verificador 
            clearCpf = clearCpf.Replace(".", ""); // Remove os Separadores das Casas 
            clearCpf = clearCpf.PadLeft(11, '0');

            // Verifica o Tamanho do Texto de Input )
            if (clearCpf.Length != 11)
                return false;

            // Verifica se no Array Existe Apenas Números 
            if (clearCpf.ToCharArray().Any(c => !char.IsNumber(c)))
                return false;

            // Converte o CPF em Array Numérico para Validar 
            var cpfArray = new int[11];

            for (var i = 0; i < clearCpf.Length; i++)
                cpfArray[i] = int.Parse(clearCpf[i].ToString());


            // Computa os Totais para os 2 Dígitos Verificadores 
            for (int position = 0; position < cpfArray.Length - 2; position++)
            {
                totalDigito1 += cpfArray[position] * (10 - position);
                totalDigito2 += cpfArray[position] * (11 - position);
            }

            // Aplica Regras do Dígito 1 
            var mod1 = totalDigito1 % 11;

            if (mod1 < 2)
                mod1 = 0;
            else
                mod1 = 11 - mod1;

            // Verifica o Digito 1 
            if (cpfArray[9] != mod1)
                return false;

            // Aplica o Peso para o Digito Verificador 2 
            totalDigito2 += mod1 * 2;

            // Aplica Regras do Dígito Verificador 2 
            var mod2 = totalDigito2 % 11;

            if (mod2 < 2) { mod2 = 0; }
            else { mod2 = 11 - mod2; }

            // Verifica o Digito 2 
            if (cpfArray[10] != mod2)
                return false;

            // CPF Válido! 
            return true;
        }
    }
}