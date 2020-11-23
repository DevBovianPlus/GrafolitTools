<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="FinancialTools.aspx.cs" Inherits="FinancialTools.Pages.FinancialTools.FinancialTools" %>

<%@ Register Assembly="DevExpress.Web.v19.2, Version=19.2.8.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function DatumPlana_DateChanged(s, e) {
            clientCallbackPanelFinancialTools.PerformCallback("DatumPlana;" + s.GetText());
        }

        function ComboBoxDatumTedna_ValueChanged(s, e)
        {
            clientCallbackPanelFinancialTools.PerformCallback("ComboBoxDatumTedna;" + s.GetValue());
        }

        function CauseValidation(s, e) {
            var procees = false;
            var comboBoxItems = [clientComboBoxDatumTedna, clientComboBoxTip];
            var dateEditItems = [clientDateEditDatumPlana];

            procees = InputFieldsValidation(null, null, dateEditItems, null, comboBoxItems);

            if (procees)
                e.processOnServer = true;
            else
                e.processOnServer = false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <dx:ASPxCallbackPanel ID="CallbackPanelFinancialTools" runat="server" Width="100%"
        ClientInstanceName="clientCallbackPanelFinancialTools" OnCallback="CallbackPanelFinancialTools_Callback">
        <PanelCollection>
            <dx:PanelContent>
                <div>
                    <div class="section group ">
                        <div class="col span_3_of_6" style="border: 1px solid #ffffff; padding: 5px;">
                            <div class="col span_1_of_3 no-margin-left-important">
                                <div>
                                    <dx:ASPxDateEdit ID="DateEditDatumPlana" runat="server" Caption="Datum plana" CaptionSettings-HorizontalAlign="Left"
                                        CaptionSettings-Position="Top" CssClass="text-box-input input-field-font-size" Font-Bold="true" Theme="MetropolisBlue"
                                        Width="100%" ClientInstanceName="clientDateEditDatumPlana">
                                        <FocusedStyle CssClass="focus-text-box-input" />
                                        <ClientSideEvents DateChanged="DatumPlana_DateChanged" Init="SetFocus" />
                                    </dx:ASPxDateEdit>
                                </div>
                            </div>
                            <div class="col span_1_of_3">
                                <div>
                                    <dx:ASPxComboBox ID="ComboBoxDatumTedna" runat="server" ValueType="System.String"
                                        ClientInstanceName="clientComboBoxDatumTedna"
                                        CssClass="text-box-input input-field-font-size" Font-Bold="true" Theme="MetropolisBlue"
                                        Width="100%" Caption="Datum tedna" CaptionSettings-HorizontalAlign="Left" CaptionSettings-Position="Top"
                                        OnDataBinding="ComboBoxDatumTedna_DataBinding" ValueField="Datum" TextField="Datum">
                                        <FocusedStyle CssClass="focus-text-box-input" />
                                        <ClientSideEvents ValueChanged="ComboBoxDatumTedna_ValueChanged" />
                                    </dx:ASPxComboBox>
                                </div>
                            </div>
                            <div class="col span_1_of_3">
                                <div>
                                    <dx:ASPxComboBox ID="ComboBoxTip" runat="server" ValueType="System.String"
                                        CssClass="text-box-input input-field-font-size" Font-Bold="true" Theme="MetropolisBlue"
                                        Width="100%" Caption="Tip" CaptionSettings-HorizontalAlign="Left" CaptionSettings-Position="Top"
                                        OnDataBinding="ComboBoxTip_DataBinding" ValueField="Vrsta" TextField="Vrsta"
                                        ClientInstanceName="clientComboBoxTip">
                                        <FocusedStyle CssClass="focus-text-box-input" />
                                    </dx:ASPxComboBox>
                                </div>
                            </div>
                        </div>
                        <div class="col span_3_of_6" style="padding: 7px;">
                            <div class="col span_1_of_3">
                                <div>
                                    <dx:ASPxTextBox ID="ASPxTextBox1" runat="server" Width="100%"
                                        Caption="Plan" CaptionSettings-HorizontalAlign="Left" CaptionSettings-Position="Top"
                                        CssClass="text-box-input input-field-font-size" Font-Bold="true" Theme="MetropolisBlue">
                                        <FocusedStyle CssClass="focus-text-box-input" />
                                    </dx:ASPxTextBox>
                                </div>
                            </div>

                            <div class="col span_1_of_3">
                                <div>
                                    <dx:ASPxTextBox ID="ASPxTextBox2" runat="server" Width="100%"
                                        Caption="Sprememba" CaptionSettings-HorizontalAlign="Left" CaptionSettings-Position="Top"
                                        CssClass="text-box-input input-field-font-size" Font-Bold="true" Theme="MetropolisBlue">
                                        <FocusedStyle CssClass="focus-text-box-input" />
                                    </dx:ASPxTextBox>
                                </div>
                            </div>
                            <div class="col span_1_of_3" style="align-items: flex-end !important; justify-content: center;">
                            <div>
                                <dx:ASPxButton ID="btnPotrdi" runat="server" Text="Potrdi" Theme="Moderno" OnClick="btnPotrdi_Click">
                                    <ClientSideEvents Click="CauseValidation" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                        </div>
                        
                        <div class="col span_3_of_6"></div>
                        <div class="col span_3_of_6">
                            <div class="col span_1_of_3">
                                <dx:ASPxTextBox ID="ASPxTextBox3" runat="server" Width="100%"
                                    Caption="Izračunan plan" CaptionSettings-HorizontalAlign="Left" CaptionSettings-Position="Top"
                                    CssClass="text-box-input input-field-font-size" Font-Bold="true" Theme="MetropolisBlue">
                                    <FocusedStyle CssClass="focus-text-box-input" />
                                </dx:ASPxTextBox>
                            </div>
                        </div>
                        <div class="col span_6_of_6 align_right_column_content">
                            <div>
                                <dx:ASPxButton ID="btnPotrdiIzracunPlan" runat="server" Text="Potrdi izračun plana" Theme="Moderno"
                                    OnClick="btnPotrdiIzracunPlan_Click">
                                    <ClientSideEvents Click="CauseValidation" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                    </div>
                </div>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
</asp:Content>
