using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using DatabaseWebService.Domain;
using System.IO;
using Newtonsoft.Json.Linq;
using DatabaseWebService.Models;
using Newtonsoft.Json;
using System.Data;
using System.Web.Script.Serialization;
using System.Net.Http;
using DatabaseWebService.Models.Client;
using DatabaseWebService.Models.Event;
using DatabaseWebService.Models.Employee;
using DatabaseWebService.Models.EmailMessage;
using DatabaseWebService.Models.FinancialControl;
using FinancialTools.Helpers;
using DatabaseWebService.ModelsDW.CashFlow_Skupno;

namespace FinancialTools.Infrastructure
{
    public class DatabaseConnection
    {
        #region FinancialControl
        public WebResponseContentModel<FinancialControlModel> GetFinancialControlData()
        {
            WebResponseContentModel<FinancialControlModel> clients = new WebResponseContentModel<FinancialControlModel>();
            try
            {
                clients = GetResponseFromWebRequest<WebResponseContentModel<FinancialControlModel>>(WebServiceURLHelper.GetFinancialControlData(), "get");
            }
            catch (Exception ex)
            {
                clients.ValidationErrorAppSide = ConcatenateExceptionMessage(ex);
            }

            return clients;
        }
        #endregion

        #region CashFlow
        public WebResponseContentModel<List<CashFlow_SkupnoModel>> GetCashFlowSkupnoByDatumPlana(DateTime datumPlana)
        {
            WebResponseContentModel<List<CashFlow_SkupnoModel>> cashFlow = new WebResponseContentModel<List<CashFlow_SkupnoModel>>();
            try
            {
                cashFlow = GetResponseFromWebRequest<WebResponseContentModel<List<CashFlow_SkupnoModel>>>(WebServiceURLHelper.GetCashFlow_SkupnoByDatumPlana(datumPlana.ToString("dd-MM-yyyy")), "get");
            }
            catch (Exception ex)
            {
                cashFlow.ValidationErrorAppSide = ConcatenateExceptionMessage(ex);
            }

            return cashFlow;
        }
        public WebResponseContentModel<List<CashFlow_SkupnoModel>> GetCashFlowSkupnoByDatum(DateTime datumTeden)
        {
            WebResponseContentModel<List<CashFlow_SkupnoModel>> cashFlow = new WebResponseContentModel<List<CashFlow_SkupnoModel>>();
            try
            {
                cashFlow = GetResponseFromWebRequest<WebResponseContentModel<List<CashFlow_SkupnoModel>>>(WebServiceURLHelper.GetCashFlow_SkupnoByDatum(datumTeden.ToString("dd-MM-yyyy")), "get");
            }
            catch (Exception ex)
            {
                cashFlow.ValidationErrorAppSide = ConcatenateExceptionMessage(ex);
            }

            return cashFlow;
        }
        public WebResponseContentModel<List<CashFlow_SkupnoModel>> GetCashFlowSkupnoByVrsta(string vrsta)
        {
            WebResponseContentModel<List<CashFlow_SkupnoModel>> cashFlow = new WebResponseContentModel<List<CashFlow_SkupnoModel>>();
            try
            {
                cashFlow = GetResponseFromWebRequest<WebResponseContentModel<List<CashFlow_SkupnoModel>>>(WebServiceURLHelper.GetCashFlow_SkupnoByVrsta(vrsta), "get");
            }
            catch (Exception ex)
            {
                cashFlow.ValidationErrorAppSide = ConcatenateExceptionMessage(ex);
            }

            return cashFlow;
        }
        #endregion

        public T GetResponseFromWebRequest<T>(string uri, string requestMethod)
        {
            object obj = default(T);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = requestMethod.ToUpper();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string streamString = reader.ReadToEnd();

            obj = JsonConvert.DeserializeObject<T>(streamString);

            return (T)obj;
        }

        public T PostWebRequestData<T>(string uri, string requestMethod, T objectToSerialize)
        {
            object obj = default(T);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = requestMethod.ToUpper();
            request.ContentType = "application/json; charset=utf-8";

            using (var sw = new StreamWriter(request.GetRequestStream()))
            {
                string clientData = JsonConvert.SerializeObject(objectToSerialize);
                sw.Write(clientData);
                sw.Flush();
                sw.Close();
            }


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string streamString = reader.ReadToEnd();

            obj = JsonConvert.DeserializeObject<T>(streamString);

            return (T)obj;
        }

        private string ConcatenateExceptionMessage(Exception ex)
        {
            return ex.Message + " \r\n" + ex.Source + (ex.InnerException != null ? ex.InnerException.Message + " \r\n" + ex.Source : "");
        }
    }
}