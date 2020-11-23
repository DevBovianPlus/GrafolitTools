using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FinancialTools
{
    public partial class Main : System.Web.UI.MasterPage
    {
        private bool disableNavBar;
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["MainMenuFinancialTools"] = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\Nav_bar\\MainMenu.xml";
            SetXmlDataSourceSetttings();
        }

        private void SetXmlDataSourceSetttings(string userRole = "Admin")
        {
            XmlDataSource1.DataFile = Session["MainMenuFinancialTools"].ToString();
            XmlDataSource1.XPath = "GlavniMenu/" + userRole + "/Oddelek";

            if (!DisableNavBar)
                ASPxNavBarMainMenu.Enabled = true;
            else
                ASPxNavBarMainMenu.Enabled = false;
        }

        public bool DisableNavBar
        {
            get { return disableNavBar; }
            set { disableNavBar = value; }
        }
    }
}