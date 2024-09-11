<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="LoginApp.Dashboard" %>

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
                                <dx:ASPxGridView ID="RolesGridView" runat="server" KeyFieldName="RoleId" AutoGenerateColumns="true"
                                    OnRowDeleting="RolesGridView_RowDeleting"
                                    OnRowInserting="RolesGridView_RowInserting" OnRowValidating="RolesGridView_RowValidating"
                                    OnRowUpdating="RolesGridView_RowUpdating" EnableCallBacks="true" Theme="Default">
                                    <SettingsDataSecurity AllowEdit="true" AllowInsert="true" AllowDelete="true" />
                                    <Columns>
                                        <dx:GridViewCommandColumn ShowEditButton="True" VisibleIndex="0" ShowNewButtonInHeader="True" ShowDeleteButton="True" />
                                        <dx:GridViewDataTextColumn FieldName="RoleId" Caption="Role ID" VisibleIndex="1">
                                            <EditFormSettings Visible="False" />
                                        </dx:GridViewDataTextColumn>
                                        <dx:GridViewDataTextColumn FieldName="RoleName" Caption="Role Name" VisibleIndex="2" />
                                    </Columns>
                                </dx:ASPxGridView>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>

                    <dx:TabPage Name="UsersPage" Text="Users">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <dx:ASPxGridView ID="UsersGridView" runat="server" AutoGenerateColumns="false" KeyFieldName="Username"
                                    OnRowInserting="UsersGridView_RowInserting"
                                    OnRowUpdating="UsersGridView_RowUpdating"
                                    OnRowDeleting="UsersGridView_RowDeleting"  OnCustomCallback="UsersGridView_CustomCallback"
                                    EnableRowsCache="false" OnRowValidating="UsersGridView_RowValidating"
                                   
                                    EnableCallBacks="True"
                                    Theme="Default">

                                    <Settings ShowFilterRow="True" ShowGroupPanel="True" />
                                    <SettingsSearchPanel Visible="True" />
                                    <SettingsBehavior AllowSort="True" />
                                    <Columns>
                                        <dx:GridViewCommandColumn VisibleIndex="0" ShowEditButton="True" ShowDeleteButton="True" ShowNewButton="True" ShowNewButtonInHeader="True" ShowClearFilterButton="True" SelectAllCheckboxMode="Page" ShowSelectCheckbox="True" />
                                        <dx:GridViewDataTextColumn FieldName="Username" Caption="Username" VisibleIndex="1" />
                                        <dx:GridViewDataTextColumn FieldName="Password" Caption="Password" VisibleIndex="2" />
                                        <dx:GridViewDataComboBoxColumn FieldName="RoleId" Caption="Role" VisibleIndex="3">
                                            <PropertiesComboBox DataSourceID="RoleDataSource" TextField="RoleName" ValueField="RoleId" />
                                        </dx:GridViewDataComboBoxColumn>
                                    </Columns>
                                </dx:ASPxGridView>
                            </dx:ContentControl>
                        </ContentCollection>
                    </dx:TabPage>
                </TabPages>
            </dx:ASPxPageControl>
            <asp:SqlDataSource ID="RoleDataSource" runat="server"
                ConnectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrainingApp;Integrated Security=True"
                SelectCommand="SELECT RoleId, RoleName FROM [dbo].[Roles]" />
        </div>
    </form>
</body>
</html>
