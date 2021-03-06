﻿using DatabaseWebService.Models;
using DevExpress.Web;
using FinancialTools.Common;
using FinancialTools.Helpers.DataProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace FinancialTools.Infrastructure
{
    public class ServerMasterPage : System.Web.UI.Page
    {
        #region Session handeling

        protected void AddValueToSession(object sesionName, object value)
        {
            Session[sesionName.ToString()] = value;
        }

        protected object GetValueFromSession(object sessionName)
        {
            return Session[sessionName.ToString()];
        }

        protected string GetStringValueFromSession(object sessionName)
        {
            if (Session[sessionName.ToString()] == null)
                return "";

            return Session[sessionName.ToString()].ToString();
        }

        protected int GetIntValueFromSession(object sessionName)
        {
            if (Session[sessionName.ToString()] == null)
                return -1;

            return CommonMethods.ParseInt(GetStringValueFromSession(sessionName));
        }

        protected decimal GetDecimalValueFromSession(object sessionName)
        {
            if (Session[sessionName.ToString()] == null)
                return -1;

            return CommonMethods.ParseDecimal(GetStringValueFromSession(sessionName));
        }

        protected double GetDoubleValueFromSession(object sessionName)
        {
            if (Session[sessionName.ToString()] == null)
                return -1.0;

            return CommonMethods.ParseDouble(GetStringValueFromSession(sessionName));
        }

        protected bool GetBoolValueFromSession(object sessionName)
        {
            if (Session[sessionName.ToString()] == null)
                return false;

            return CommonMethods.ParseBool(Session[sessionName.ToString()].ToString());
        }

        protected bool SessionHasValue(object sessionName)
        {
            if (Session[sessionName.ToString()] != null)
                return true;

            return false;
        }

        protected void RemoveAllSesions()
        {
            Session.RemoveAll();
        }

        protected void RemoveSession(object sessionName)
        {
            Session.Remove(sessionName.ToString());
        }

        protected void ClearAllSessions<T>(List<T> sessionList)
        {
            foreach (var item in sessionList)
            {
                RemoveSession(item.ToString());
            }
        }

        protected void ClearAllSessions<T>(List<T> sessionList, string redirectPageUrl, bool isCallback = false)
        {
            foreach (var item in sessionList)
            {
                RemoveSession(item.ToString());
            }

            if (isCallback)
                ASPxWebControl.RedirectOnCallback(redirectPageUrl);
            else
                Response.Redirect(redirectPageUrl);
        }
        #endregion

        #region Instance extractor
        protected DatabaseConnection GetDatabaseConnectionInstance()
        {
            DatabaseConnection dbConnection = null;

            if (dbConnection == null)
                return new DatabaseConnection();

            return dbConnection;
        }
        protected FinancialToolsDataProvider GetFinancialToolDataProviderInstance()
        {
            FinancialToolsDataProvider financialToolsProvider = null;

            if (financialToolsProvider == null)
                return new FinancialToolsDataProvider();

            return financialToolsProvider;
        }
        #endregion

        #region Client POP UP handeling
        protected void ShowClientPopUp(string message, int popUpWindow = 0)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CommonJS", String.Format("ShowErrorPopUp('{0}', '{1}');", message, popUpWindow), true);
        }

        /// <summary>
        /// We are using this popup if the session showWarning contains value
        /// </summary>
        /// <param name="message">Message that will show on popup.</param>
        protected void ShowWarningPopUp(int popUpWindow = 0)
        {
            if (SessionHasValue(Enums.CommonSession.ShowWarning))
            {
                if (GetBoolValueFromSession(Enums.CommonSession.ShowWarning))
                    ShowClientPopUp(GetStringValueFromSession(Enums.CommonSession.ShowWarningMessage));

                RemoveSession(Enums.CommonSession.ShowWarning);
                RemoveSession(Enums.CommonSession.ShowWarningMessage);
            }
        }
        #endregion

        protected DataTable SerializeToDataTable<T>(List<T> list, string keyFieldName = "", string visibleColumn = "")
        {
            DataTable dt = new DataTable();
            string json = JsonConvert.SerializeObject(list);
            dt = JsonConvert.DeserializeObject<DataTable>(json);

            if (keyFieldName != "" && visibleColumn != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.NewRow();
                row[keyFieldName] = -1;
                row[visibleColumn] = "Izberi...";
                dt.Rows.InsertAt(row, 0);
            }

            return dt;
        }

        protected T CheckModelValidation<T>(WebResponseContentModel<T> instance)
        {
            object obj = default(T);

            if (!instance.IsRequestSuccesful)
            {
                string requestFailedError = "";

                if (!String.IsNullOrEmpty(instance.ValidationError))
                {
                    instance.ValidationError = instance.ValidationError.Replace("'", "");
                    //instance.ValidationError = instance.ValidationError.Insert(0, "'");
                    //instance.ValidationError += "'";
                    instance.ValidationError = instance.ValidationError.Replace("\\", "\\\\");
                    instance.ValidationError = instance.ValidationError.Replace("\r\n", "");
                    requestFailedError = instance.ValidationError;
                }
                else if (!String.IsNullOrEmpty(instance.ValidationErrorAppSide))
                    requestFailedError = instance.ValidationErrorAppSide.Replace("\r\n", "");

                ShowClientPopUp(requestFailedError);

                return (T)obj;
            }
            else
            {
                obj = instance.Content;
            }

            return (T)obj;
        }


        protected void UserActionConfirmBtnUpdate(ASPxButton button, int userAction, bool popUpBtn = false)
        {
            if (userAction == (int)Enums.UserAction.Delete)
            {
                button.ImageUrl = popUpBtn ? "~/Images/trashPopUp.png" : "~/Images/trash2.png";
                button.Text = "Izbrisi";
            }
            else if (userAction == (int)Enums.UserAction.Add)
            {
                button.ImageUrl = popUpBtn ? "~/Images/addPopUp.png" : "~/Images/add2.png";
                button.Text = "Shrani";
            }
            else
            {
                button.ImageUrl = popUpBtn ? "~/Images/editPopup.png" : "~/Images/edit2.png";
                button.Text = "Shrani";
            }
        }

        protected void EnabledDeleteAndEditBtnPopUp(ASPxButton buttonEdit, ASPxButton buttonDelete, bool disable = true)
        {
            if (disable)
            {
                buttonEdit.ImageUrl = "~/Images/btnPopUpEditDisabled.png";
                buttonEdit.Text = "Spremeni";
                buttonEdit.Enabled = false;

                buttonDelete.ImageUrl = "~/Images/btnPopUpDeleteDisabled.png";
                buttonDelete.Text = "Izbrisi";
                buttonDelete.Enabled = false;
            }
            else
            {
                buttonEdit.ImageUrl = "~/Images/editForPopup.png";
                buttonEdit.Text = "Spremeni";
                buttonEdit.Enabled = true;

                buttonDelete.ImageUrl = "~/Images/trashForPopUp.png";
                buttonDelete.Text = "Izbrisi";
                buttonDelete.Enabled = true;
            }
        }
        protected void EnabledAddBtnPopUp(ASPxButton buttonAdd, bool disable = true)
        {
            if (disable)
            {
                buttonAdd.ImageUrl = "~/Images/addPopupDisabled.png";
                buttonAdd.Text = "Spremeni";
                buttonAdd.Enabled = false;
            }
            else
            {
                buttonAdd.ImageUrl = "~/Images/addPopUp.png";
                buttonAdd.Text = "Spremeni";
                buttonAdd.Enabled = true;
            }
        }
    }
}