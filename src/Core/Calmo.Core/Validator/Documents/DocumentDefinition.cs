namespace Calmo.Core.Validator.Documents
{
	/// <summary>
	/// Abstract definition for any document validation (ID's, CPF, Passports, etc)
	/// </summary>
    public abstract class DocumentDefinition
    {
		/// <summary>
		/// Abstract validation method
		/// </summary>
		/// <param name="value">Document number/value</param>
		/// <returns></returns>
        public abstract bool Validate(string value);
    }
}