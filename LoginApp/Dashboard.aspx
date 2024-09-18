<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="LoginApp.Dashboard" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="0">
                <TabPages>
                    <dx:TabPage Name="RolePage" Text="Role">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <dx:ASPxGridView ID="RolesGridView" runat="server" KeyFieldName="Id" AutoGenerateColumns="true"
                                    OnRowDeleting="RolesGridView_RowDeleting"
                                    OnRowInserting="RolesGridView_RowInserting" OnRowValidating="RolesGridView_RowValidating"
                                    OnRowUpdating="RolesGridView_RowUpdating" EnableCallBacks="true" Theme="Default"
                                    OnDataBinding="RolesGridView_DataBinding">
                                    <Settings ShowGroupPanel="True" ShowFilterRow="True"></Settings>
                                    <SettingsSearchPanel Visible="True"></SettingsSearchPanel>
                                    <SettingsDataSecurity AllowEdit="true" AllowInsert="true" AllowDelete="true" />
                                    <Columns>
                                        <dx:GridViewCommandColumn ShowEditButton="True" VisibleIndex="0" ShowNewButtonInHeader="True" ShowDeleteButton="True" ShowClearFilterButton="True" SelectAllCheckboxMode="Page" ShowSelectCheckbox="True" />
                                        <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" VisibleIndex="1">
                                            <EditFormSettings Visible="False" />
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="Name" Caption="Name" VisibleIndex="2" />
                                    </Columns>
                                </dx:ASPxGridView>
                                <asp:Button ID="RolesbtnExportPDF" runat="server" Text="Export to PDF" OnClick="RolesbtnExportPDF_Click" />
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                    <dx:TabPage Name="UsersPage" Text="Users">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <dx:ASPxGridView ID="UsersGridView" runat="server"
                                    AutoGenerateColumns="false"
                                    KeyFieldName="Username"
                                    OnDataBinding="UsersGridView_DataBinding"
                                    OnRowInserting="UsersGridView_RowInserting"
                                    OnRowUpdating="UsersGridView_RowUpdating"
                                    OnRowDeleting="UsersGridView_RowDeleting" OnRowValidating="UsersGridView_RowValidating"
                                    EnableRowsCache="false"
                                    EnableCallBacks="true"
                                    Theme="Default">
                                    <Settings ShowGroupPanel="True" ShowFilterRow="True"></Settings>
                                    <SettingsSearchPanel Visible="True"></SettingsSearchPanel>
                                    <SettingsDataSecurity AllowEdit="true" AllowInsert="true" AllowDelete="true" />
                                    <Columns>
                                        <dx:GridViewCommandColumn VisibleIndex="0" ShowEditButton="True" ShowDeleteButton="True" ShowNewButton="True" ShowNewButtonInHeader="True" ShowClearFilterButton="True" SelectAllCheckboxMode="Page" ShowSelectCheckbox="True" />
                                        <dx:GridViewDataTextColumn FieldName="Username" Caption="Username" VisibleIndex="1" ReadOnly="false" LoadReadOnlyValueFromDataModel="false"/>
                                        <dx:GridViewDataTextColumn FieldName="Password" Caption="Password" VisibleIndex="2" ReadOnly="false" LoadReadOnlyValueFromDataModel="false" />
                                        <dx:GridViewDataComboBoxColumn FieldName="RoleId" Caption="Role" VisibleIndex="3" ReadOnly="false" PropertiesComboBox-DataSourceID="RoleDataSource" PropertiesComboBox-TextField="Name" PropertiesComboBox-ValueField="Id" LoadReadOnlyValueFromDataModel="false">
                                        </dx:GridViewDataComboBoxColumn>
                                    </Columns>
                                </dx:ASPxGridView>
                                <asp:Button ID="UsersbtnExportPDF" runat="server" Text="Export to PDF" OnClick="UsersbtnExportPDF_Click" />
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                </TabPages>
            </dx:ASPxPageControl>
            <asp:SqlDataSource ID="RoleDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrainingApp;Integrated Security=True"
                SelectCommand="SELECT Id, Name FROM [dbo].[Roles]" />
            <asp:SqlDataSource ID="UserDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrainingApp;Integrated Security=True"
                SelectCommand="SELECT Username, Password, RoleId FROM [dbo].[Users]" />
        </div>
    </form>
</body>
</html>
