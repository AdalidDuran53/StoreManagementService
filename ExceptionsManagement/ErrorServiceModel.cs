using System.Collections.Generic;

namespace ExceptionsManagement
{
    public class ErroritemServiceModel
    {
        public ErroritemServiceModel()
        {
        }
        public ErroritemServiceModel(string code, string message, string details = "")
        {
            Code = code;
            Message = message;
            Details = details;
        }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
    public class ErrorServiceModel
    {
        public ErrorServiceModel()
        {
            InitializeErrors();
        }

        private Dictionary<string, ErroritemServiceModel> errors = new Dictionary<string, ErroritemServiceModel>();

        public ErroritemServiceModel GetError(string code)
        {
            if (errors.ContainsKey(code))
            {
                return errors[code];
            }
            return new ErroritemServiceModel("UnknownError", "An unknown error occurred.");
        }


        private void InitializeErrors()
        {
            // define all error items here
            #region General
            errors.Add("OMS-GENERAL-ERROR", new ErroritemServiceModel(
                code: "OMS-GENERAL-ERROR",
                message: "unexpected error.",
                details: "An unexpected error has occurred in the service. Please try again later or contact the administrator if the problem persists."));
            #endregion

            #region User
            errors.Add("OMS-USERNAME-ERROR", new ErroritemServiceModel(
                code: "OMS-USERNAME-ERROR",
                message: "Invalid user.",
                details: "The username {0} is not valid."));

            errors.Add("OMS-PASSWORD-ERROR", new ErroritemServiceModel(
                code: "OMS-PASSWORD-ERROR",
                message: "Invalid Password.",
                details: "Password is not valid."));

            errors.Add("OMS-CLIENTNAME-ERROR", new ErroritemServiceModel(
                code: "OMS-CLIENTNAME-ERROR",
                message: "Invalid Client name.",
                details: "Client name is not valid."));

            errors.Add("OMS-CLIENTLASTNAME-ERROR", new ErroritemServiceModel(
                code: "OMS-CLIENTLASTNAME-ERROR",
                message: "Invalid Client last name.",
                details: "Client last name is not valid."));

            errors.Add("OMS-CLIENTADDRESS-ERROR", new ErroritemServiceModel(
                code: "OMS-CLIENTADDRESS-ERROR",
                message: "Invalid Client address.",
                details: "Client address is not valid."));

            errors.Add("OMS-VERIFY-CODE-ERROR", new ErroritemServiceModel(
                code: "OMS-CLIENTADDRESS-ERROR",
                message: "Invalid code.",
                details: "code is not valid."));
            #endregion

            #region Session
            errors.Add("OMS-LOGIN-ERROR", new ErroritemServiceModel(
                code: "OMS-LOGIN-ERROR",
                message: "Invalid credentials.",
                details: "Invalid credentials."));

            errors.Add("OMS-SESSION-ERROR", new ErroritemServiceModel(
                code: "OMS-SESSION-ERROR",
                message: "unexpected error.",
                details: "An unexpected error has occurred in the service. Please try again later or contact the administrator if the problem persists."));
            #endregion

            #region Store
            errors.Add("OMS-STOREBRANCH-ERROR" 
                , new ErroritemServiceModel(
                code: "OMS-STOREBRANCH-ERROR",
                message: "Invalid Store branch.",
                details: "Store branch is not valid."));

            errors.Add("OMS-STOREADDRESS-ERROR", new ErroritemServiceModel(
                code: "OMS-STOREADDRESS-ERROR",
                message: "Invalid Store address.",
                details: "Store address is not valid."));

            errors.Add("OMS-STORE-NOTFOUND-ERROR", new ErroritemServiceModel(
                code: "OMS-STORE-NOTFOUND-ERROR",
                message: "Invalid Store.",
                details: "Store is not valid."));
            #endregion

            #region Item
            errors.Add("OMS-ITEMCODE-ERROR", new ErroritemServiceModel(
                code: "OMS-ITEMCODE-ERROR",
                message: "Invalid Item code.",
                details: "Item code is not valid."));

            errors.Add("OMS-ITEMDESCRIPTION-ERROR", new ErroritemServiceModel(
                code: "OMS-ITEMDESCRIPTION-ERROR",
                message: "Invalid Item description.",
                details: "Item description is not valid."));

            errors.Add("OMS-ITEM-NOTFOUND-ERROR", new ErroritemServiceModel(
                code: "OMS-ITEM-NOTFOUND-ERROR",
                message: "Invalid Item.",
                details: "Item is not valid."));
            #endregion

            #region ItemClient
            errors.Add("OMS-ITEMAMOUNT-ERROR", new ErroritemServiceModel(
                code: "OMS-ITEMAMOUNT-ERROR",
                message: "Invalid amount.",
                details: "amount is not valid."));
            #endregion
        }
    }
}
