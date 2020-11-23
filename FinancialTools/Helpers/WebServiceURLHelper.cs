using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace FinancialTools.Helpers
{
    public static class WebServiceURLHelper
    {
        private static string BaseWebServiceURI
        {
            get
            {
                return WebConfigurationManager.AppSettings["BaseServiceURI"].ToString();
            }
        }
        private static string WebServiceFinancialControlURI
        {
            get
            {
                return BaseWebServiceURI + WebConfigurationManager.AppSettings["FinancialControlPartialURI"].ToString();
            }
        }

        private static string WebServiceCashFlowURI
        {
            get
            {
                return BaseWebServiceURI + WebConfigurationManager.AppSettings["CashFlowPartialURI"].ToString();
            }
        }

        #region FinancialControl
        public static string GetFinancialControlData()
        {
            return WebServiceFinancialControlURI + "GetFinancialControlData";
        }
        #endregion

        #region CashFlow
        public static string GetCashFlow_SkupnoByDatumPlana(string datumPlana)
        {
            return WebServiceCashFlowURI + "GetCashFlow_SkupnoByDatumPlana?datumPlana=" + datumPlana;
        }
        public static string GetCashFlow_SkupnoByDatum(string datumTeden)
        {
            return WebServiceCashFlowURI + "GetCashFlow_SkupnoByDatum?datumTeden=" + datumTeden;
        }
        public static string GetCashFlow_SkupnoByVrsta(string vrsta)
        {
            return WebServiceCashFlowURI + "GetCashFlow_SkupnoByVrsta?vrsta=" + vrsta;
        }
        #endregion
    }
}