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
                                <dx:ASPxGridView ID="ASPxGridView2" runat="server" KeyFieldName="RoleId" AutoGenerateColumns="true"
                                    OnRowDeleting="ASPxGridView2_RowDeleting"
                                    OnRowInserting="ASPxGridView2_RowInserting" OnRowValidating="ASPxGridView2_RowValidating"
                                    OnRowUpdating="ASPxGridView2_RowUpdating" EnableCallBacks="true" Theme="Default">
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
                                <dx:ASPxGridView ID="ASPxGridView1" runat="server" AutoGenerateColumns="false" KeyFieldName="Username"
                                    OnRowInserting="ASPxGridView1_RowInserting"
                                    OnRowUpdating="ASPxGridView1_RowUpdating"
                                    OnRowDeleting="ASPxGridView1_RowDeleting"  OnCustomCallback="ASPxGridView1_CustomCallback" EnableRowsCache="false" OnRowValidating="ASPxGridView1_RowValidating"
                                   
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
