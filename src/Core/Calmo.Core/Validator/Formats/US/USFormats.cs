using Calmo.Core.Validator.Formats;
using Calmo.Core.Validator.Formats.US;

namespace Calmo.Core.Validator
{
    public class USFormats
    {
        internal USFormats()
        {

        }
        
        public FormatDefinition Phone = new PhoneFormatDefinition();
    }
}
