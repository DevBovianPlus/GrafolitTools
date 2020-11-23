using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialTools.Common
{
    public class Enums
    {
        public enum UserRole
        {
            SuperAdmin,
            Admin,
            Salesman,
            User
        }

        public enum UserAction : int
        {
            Add = 1,
            Edit = 2,
            Delete = 3
        }

        public enum QueryStringName
        {
            action,
            recordId,
            eventClientId,
            eventCategorieId,
            eventEmployeeId,
            categorieId
        }
        public enum CommonSession
        {
            ShowWarning,
            ShowWarningMessage,
            UserActionPopUp,
            activeTab,
            PrintModel
        }

        public enum FinancialTools
        {
            CashFlow_Skupno,
            CashFlow_SkupnoFilterbyDatumPlanaAndDatum
        }
        
    }
}