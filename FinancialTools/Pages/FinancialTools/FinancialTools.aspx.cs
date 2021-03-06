﻿using DatabaseWebService.ModelsDW.CashFlow_Skupno;
using DevExpress.Web;
using FinancialTools.Common;
using FinancialTools.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FinancialTools.Pages.FinancialTools
{
    public partial class FinancialTools : ServerMasterPage
    {
        List<CashFlow_SkupnoModel> model = null;
        List<CashFlow_SkupnoModel> modelFilterByDatumPlanaAndDatum = null;
        bool datumTedenValueChanged = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (model == null || GetFinancialToolDataProviderInstance().GetCashFlow_SkupnoFromSession() != null)
                model = GetFinancialToolDataProviderInstance().GetCashFlow_SkupnoFromSession();

            if (modelFilterByDatumPlanaAndDatum == null || GetFinancialToolDataProviderInstance().GetCashFlow_SkupnoFilterByDatumPlanaAndDatumFromSession() != null)
                modelFilterByDatumPlanaAndDatum = GetFinancialToolDataProviderInstance().GetCashFlow_SkupnoFilterByDatumPlanaAndDatumFromSession();
        }

        protected void CallbackPanelFinancialTools_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            string[] split = e.Parameter.Split(';');
            if (split[0] == "DatumPlana")
            {
                DateTime datumPlana = DateTime.MinValue;
                DateTime.TryParse(split[1], out datumPlana);
                if (datumPlana.CompareTo(DateTime.MinValue) > 0)
                {
                    model = CheckModelValidation(GetDatabaseConnectionInstance().GetCashFlowSkupnoByDatumPlana(datumPlana));

                    if (model != null)
                        GetFinancialToolDataProviderInstance().SetCashFlow_Skupno(model);

                    ComboBoxDatumTedna.DataBind();
                    ComboBoxTip.DataBind();
                }
            }
            else if (split[0] == "ComboBoxDatumTedna")
            {
                if (model == null) return;

                DateTime datum = DateTime.MinValue;
                DateTime.TryParse(split[1], out datum);
                if (datum.CompareTo(DateTime.MinValue) > 0)
                {
                    modelFilterByDatumPlanaAndDatum = model.Where(ft => ft.Datum.CompareTo(datum) == 0).ToList();
                    GetFinancialToolDataProviderInstance().SetCashFlow_SkupnoFilterByDatumPlanaAndDatum(modelFilterByDatumPlanaAndDatum);
                    datumTedenValueChanged = true;
                    ComboBoxTip.DataBind();
                }
            }
        }

        protected void ComboBoxTip_DataBinding(object sender, EventArgs e)
        {
            if (datumTedenValueChanged)
            {
                (sender as ASPxComboBox).DataSource = modelFilterByDatumPlanaAndDatum;
            }
            else 
            {
                if (model != null)
                    (sender as ASPxComboBox).DataSource = model;
            }
        }

        protected void ComboBoxDatumTedna_DataBinding(object sender, EventArgs e)
        {
            if (model != null)
            {
                (sender as ASPxComboBox).DataSource = model;
            }
        }

        protected void btnPotrdi_Click(object sender, EventArgs e)
        {

        }

        protected void btnPotrdiIzracunPlan_Click(object sender, EventArgs e)
        {

        }
    }
}