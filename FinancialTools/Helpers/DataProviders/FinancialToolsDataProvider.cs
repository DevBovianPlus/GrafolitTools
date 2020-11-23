using DatabaseWebService.ModelsDW.CashFlow_Skupno;
using FinancialTools.Common;
using FinancialTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialTools.Helpers.DataProviders
{
    public class FinancialToolsDataProvider : ServerMasterPage
    {
        public bool SetCashFlow_Skupno(List<CashFlow_SkupnoModel> model)
        {
            if (model != null)
            {
                AddValueToSession(Enums.FinancialTools.CashFlow_Skupno, model);

                return true;
            }
            return false;
        }

        public List<CashFlow_SkupnoModel> GetCashFlow_SkupnoFromSession()
        {
            if (SessionHasValue(Enums.FinancialTools.CashFlow_Skupno))
                return (List<CashFlow_SkupnoModel>)GetValueFromSession(Enums.FinancialTools.CashFlow_Skupno);

            return null;
        }

        public bool SetCashFlow_SkupnoFilterByDatumPlanaAndDatum(List<CashFlow_SkupnoModel> model)
        {
            if (model != null)
            {
                AddValueToSession(Enums.FinancialTools.CashFlow_SkupnoFilterbyDatumPlanaAndDatum, model);

                return true;
            }
            return false;
        }

        public List<CashFlow_SkupnoModel> GetCashFlow_SkupnoFilterByDatumPlanaAndDatumFromSession()
        {
            if (SessionHasValue(Enums.FinancialTools.CashFlow_SkupnoFilterbyDatumPlanaAndDatum))
                return (List<CashFlow_SkupnoModel>)GetValueFromSession(Enums.FinancialTools.CashFlow_SkupnoFilterbyDatumPlanaAndDatum);

            return null;
        }
    }
}