using System;
using System.Collections.Generic;
using System.Text;

namespace ExceptionsManagement
{
    public interface IValidation
    {
        public string Validate(string operationExceptionCode);
    }
}
