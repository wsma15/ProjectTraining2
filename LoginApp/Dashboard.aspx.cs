using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DevExpress.Web;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace LoginApp
{
    public partial class Dashboard : System.Web.UI.Page
    {
        readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrainingApp;Integrated Security=True";
        protected void UsersbtnExportPDF_Click(object sender, EventArgs e)
        {
            DataTable dataTable = GetUsersDataFromGridView();

            ReportDocument rptDoc = new ReportDocument();

            string reportPath = Server.MapPath("~/UsersReport.rpt");
            
            rptDoc.Load(reportPath);

            rptDoc.SetDataSource(dataTable);

            string directoryPath = Server.MapPath("~/Reports");
            
            string filePath = Path.Combine(directoryPath, "UsersReport.pdf");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            ExportOptions exportOptions = new ExportOptions();
            
            DiskFileDestinationOptions diskOptions = new DiskFileDestinationOptions();
            
            PdfRtfWordFormatOptions pdfOptions = new PdfRtfWordFormatOptions();

            diskOptions.DiskFileName = filePath;
            
            exportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
            
            exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
            
            exportOptions.DestinationOptions = diskOptions;
            
            exportOptions.FormatOptions = pdfOptions;
            
            try
            {
                rptDoc.Export(exportOptions);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while exporting the report.", ex);
            }

            FileInfo file = new FileInfo(filePath);
            
            if (file.Exists)
            {
                Response.Clear();
            
                Response.Buffer = true;
                
                Response.AddHeader("content-disposition", "attachment;filename=" + file.Name);
                
                Response.AddHeader("content-length", file.Length.ToString());
                
                Response.ContentType = "application/pdf";
                
                Response.WriteFile(file.FullName);
                
                Response.End();
            }
        }
        protected void RolesbtnExportPDF_Click(object sender, EventArgs e)
        { 
            DataTable dataTable = GetRolesDataFromGridView();

            ReportDocument rptDoc = new ReportDocument();

            string reportPath = Server.MapPath("~/RolesReport.rpt");

            rptDoc.Load(reportPath);

            rptDoc.SetDataSource(dataTable);

            string directoryPath = Server.MapPath("~/Reports");

            string filePath = Path.Combine(directoryPath, "RolesReport.pdf");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            ExportOptions exportOptions = new ExportOptions();

            DiskFileDestinationOptions diskOptions = new DiskFileDestinationOptions();

            PdfRtfWordFormatOptions pdfOptions = new PdfRtfWordFormatOptions();

            diskOptions.DiskFileName = filePath;

            exportOptions.ExportDestinationType = ExportDestinationType.DiskFile;

            exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;

            exportOptions.DestinationOptions = diskOptions;

            exportOptions.FormatOptions = pdfOptions;

            try
            {
                rptDoc.Export(exportOptions);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while exporting the report.", ex);
            }

            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + file.Name);
                Response.AddHeader("content-length", file.Length.ToString());
                Response.ContentType = "application/pdf";
                Response.WriteFile(file.FullName);
                Response.Flush();
                Response.SuppressContent = true;
            }
        }

        private DataTable GetRolesDataFromGridView()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("id", typeof(int));
            dt.Columns.Add("Name", typeof(string));

            var dataSource = RolesGridView.DataSource as DataTable;

            if (dataSource != null)
            {
                foreach (DataRow row in dataSource.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["id"] = Convert.ToInt32(row["id"]);
                    dr["Name"] = row["Name"].ToString();
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Data Source Null.");
            }

            return dt;
        }


        private DataTable GetUsersDataFromGridView()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Username", typeof(string));
            dt.Columns.Add("Password", typeof(string));
            dt.Columns.Add("RoleId", typeof(int));

            var dataSource = UsersGridView.DataSource as DataTable;

            if (dataSource != null)
            {
                foreach (DataRow row in dataSource.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["Username"] = row["Username"].ToString();
                    dr["Password"] = row["Password"].ToString();
                    dr["RoleId"] = Convert.ToInt32(row["RoleId"]);
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }
        private void BindUsersGridView()
        {
            ReportDocument rptdoc = new ReportDocument();
            rptdoc.Load(Server.MapPath("~/UsersReport.rpt"));
            string query = "SELECT Username, Password, RoleId FROM [dbo].[Users]";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                conn.Open();
                da.Fill(dt);
                conn.Close();

                foreach (DataRow row in dt.Rows)
                {
                    string encryptedPassword = row["Password"].ToString();
                    string decryptedPassword = PasswordHelper.Decrypt(encryptedPassword);
                    row["Password"] = decryptedPassword;
                }

                UsersGridView.DataSource = dt;
                UsersGridView.DataBind();
                UsersGridView.EnableViewState = false;
            }
        }

        private bool IsValidUsername(string username)
        {
            var regex = new Regex(@"^[a-zA-Z0-9]{6,}$");
            return regex.IsMatch(username);
        }

        private bool IsValidName(string name)
        {
            var regex = new Regex(@"^[a-zA-Z]+( [a-zA-Z]+)*$");
            return regex.IsMatch(name);
        }

        private bool IsValidPassword(string password)
        {
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
            return regex.IsMatch(password);
        }

        private bool IsRoleAssignedToUsers(int RoleId)
        {
            bool isAssigned = false;
            string query = "SELECT COUNT(*) FROM [dbo].[Users] WHERE RoleId = @RoleId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoleId", RoleId);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                conn.Close();

                isAssigned = count > 0;
            }
            return isAssigned;
        }

        private void BindRolesGridView()
        {
            string query = "SELECT Id, Name FROM Roles";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);


                RolesGridView.DataSource = dt;
                RolesGridView.DataBind();
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            BindUsersGridView();
            BindRolesGridView();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUsersGridView();
                BindRolesGridView();
            }
        }

        protected void UsersGridView_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            if (e.NewValues["Username"] != null && e.NewValues["Password"] != null)
            {
                string username = e.NewValues["Username"].ToString();
                string password = e.NewValues["Password"].ToString();
                string hashedPassword = PasswordHelper.HashPassword(password);
                var RoleId = e.NewValues["RoleId"];

                if (RoleId == null)
                {
                    e.Cancel = true;
                    UsersGridView.JSProperties["cpMessage"] = "Role ID is missing.";
                    return;
                }

                if (!IsValidUsername(username))
                {
                    e.Cancel = true;
                    UsersGridView.JSProperties["cpMessage"] = "Username must be at least 6 characters long, and contain no spaces or special characters.";
                    return;
                }

                if (!IsValidPassword(password))
                {
                    e.Cancel = true;
                    UsersGridView.JSProperties["cpMessage"] = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
                    return;
                }

                string query = "INSERT INTO [dbo].[Users] (Username, Password, RoleId) VALUES (@Username, @Password, @RoleId)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@RoleId", RoleId ?? DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                e.Cancel = true;
                UsersGridView.CancelEdit();
                BindUsersGridView();
            }
        }

        protected void UsersGridView_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            string username = e.Keys["Username"].ToString();
            string password = e.NewValues["Password"].ToString();
            int RoleId = Convert.ToInt32(e.NewValues["RoleId"]);

            string query = "UPDATE [dbo].[Users] SET Password = @Password, RoleId = @RoleId WHERE Username = @Username";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", PasswordHelper.HashPassword(password));
                cmd.Parameters.AddWithValue("@RoleId", RoleId);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            e.Cancel = true;
            UsersGridView.CancelEdit();
            BindUsersGridView();
        }

        protected void UsersGridView_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"CustomCallback triggered. Parameters: {e.Parameters}");
            BindUsersGridView();
        }

        protected void UsersGridView_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            foreach (GridViewColumn row in UsersGridView.Columns)
            {
                GridViewDataColumn dataRow = row as GridViewDataColumn;
                if (dataRow == null) continue;
                if (e.NewValues[dataRow.FieldName] == null)
                    e.Errors[dataRow] = "Value cannot be null.";
            }

            if ((e.NewValues["Username"]) != null &&
                !IsValidUsername(e.NewValues["Username"].ToString()))
            {
                e.Errors[(GridViewDataColumn)UsersGridView.Columns["Username"]] = "The input must be at least 6 characters long, only contain letters and numbers, and have no special characters.";
            }

            if ((e.NewValues["Password"]) != null &&
                !IsValidPassword(e.NewValues["Password"].ToString()))
            {
                e.Errors[(GridViewDataColumn)UsersGridView.Columns["Password"]] = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
            }
            if (e.Errors.Count > 0) e.RowError = "Please fill in all fields.";
        }

        protected void UsersGridView_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            string username = e.Keys["Username"].ToString();

            string query = "DELETE FROM [dbo].[Users] WHERE Username = @Username";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            e.Cancel = true;
            BindUsersGridView();
        }

        protected void RolesGridView_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            if (e.NewValues["Name"] != null)
            {
                string roleName = e.NewValues["Name"].ToString();

                string query = "INSERT INTO [dbo].[Roles] (Name) VALUES (@Name)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Name", roleName);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                e.Cancel = true;
                RolesGridView.CancelEdit();
                BindRolesGridView();
            }
        }

        protected void RolesGridView_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            string RoleId = e.Keys["Id"].ToString();
            string roleName = e.NewValues["Name"].ToString();

            string query = "UPDATE [dbo].[Roles] SET Name = @Name WHERE id = @id";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", roleName);
                cmd.Parameters.AddWithValue("@id", RoleId);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            e.Cancel = true;
            RolesGridView.CancelEdit();
            BindRolesGridView();
        }

        protected void RolesGridView_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            int RoleId = Convert.ToInt32(e.Keys["Id"]);

            if (IsRoleAssignedToUsers(RoleId))
            {
                RolesGridView.JSProperties["cpMessage"] = "Cannot delete the role as it is assigned to one or more users.";
                e.Cancel = true;
            }
            else
            {
                string query = "DELETE FROM [dbo].[Roles] WHERE Id = @id";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", RoleId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                e.Cancel = true;
                BindRolesGridView();
            }
        }


        protected void RolesGridView_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            if (e.NewValues["Name"] == null)
            {
                e.Errors[RolesGridView.Columns["Name"]] = "Value cannot be null.";
            }
            else
            {
                if (!IsValidName(e.NewValues["Name"].ToString()))
                {
                    e.Errors[RolesGridView.Columns["Name"]] = "The name must contain only letters and have at least two words.";
                }
            }

            if (e.Errors.Count > 0)
            {
                e.RowError = "Please fill in all fields correctly.";
            }
        }
    }
}