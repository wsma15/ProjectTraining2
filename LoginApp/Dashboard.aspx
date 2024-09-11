<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="LoginApp.Dashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

        <div>
            <dx:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="0">

                <TabPages>
                    <dx:TabPage Name="  RolePage" Text="Role">
                        <ContentCollection>
                            <dx:ContentControl runat="server">
                                <dx:ASPxGridView ID="ASPxGridView2" runat="server" KeyFieldName="RoleId" OnRowDeleting="ASPxGridView2_RowDeleting" OnRowInserting="ASPxGridView2_RowInserting" OnRowUpdating="ASPxGridView2_RowUpdating">
                                    <SettingsDataSecurity AllowEdit="true" AllowInsert="true" AllowDelete="true"></SettingsDataSecurity>
                                    <Columns>
                                        <dx:GridViewCommandColumn ShowEditButton="True" VisibleIndex="0" ShowNewButtonInHeader="True" ShowDeleteButton="True"></dx:GridViewCommandColumn>

                                        <dx:GridViewDataTextColumn FieldName="RoleId" Caption="Role ID" VisibleIndex="1">
                                            <EditFormSettings Visible="False"></EditFormSettings>

                                        </dx:GridViewDataTextColumn>

                                        <dx:GridViewDataTextColumn FieldName="RoleName" Caption="Role Name" VisibleIndex="2"></dx:GridViewDataTextColumn>
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
                                    OnRowDeleting="ASPxGridView1_RowDeleting" Theme="Default">
                                    <Settings ShowFilterRow="True"></Settings>
                                    <SettingsSearchPanel Visible="True"></SettingsSearchPanel>
                                    <SettingsDataSecurity AllowEdit="true" AllowInsert="true" AllowDelete="true"></SettingsDataSecurity>
                                    <Columns>
                                        <dx:GridViewCommandColumn VisibleIndex="0" ShowEditButton="True" ShowDeleteButton="True" ShowNewButton="True" ShowNewButtonInHeader="True" ShowClearFilterButton="True">
                                        </dx:GridViewCommandColumn>

                                        <dx:GridViewDataTextColumn FieldName="Username" Caption="Username" VisibleIndex="2"></dx:GridViewDataTextColumn>

                                        <dx:GridViewDataTextColumn FieldName="Password" Caption="Password" VisibleIndex="3"></dx:GridViewDataTextColumn>

                                        <dx:GridViewDataComboBoxColumn FieldName="RoleId" Caption="Role" VisibleIndex="4">
                                            <PropertiesComboBox DataSourceID="RoleDataSource" TextField="RoleName" ValueField="RoleId">
                                            </PropertiesComboBox>
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
                SelectCommand="SELECT RoleId, RoleName FROM [dbo].[Roles]"></asp:SqlDataSource>
        </div>
    </form>
</body>
</html>
