using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DevExpress.Web;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace LoginApp
{
    public partial class Dashboard : System.Web.UI.Page
    {

        readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrainingApp;Integrated Security=True";

        protected DataTable GetUsersDataFromProcedure()
        {
            DataTable dt = new DataTable();
            string storedProcedureName = "RetriveAllData"; // Ensure this matches exactly with your stored procedure name
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt); // Fill the DataTable with the result from the stored procedure
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
            return dt;
        }

        protected DataTable GetRolesDataFromProcedure()
        {
            DataTable dt = new DataTable();
            string storedProcedureName = "RetriveRoles";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
            return dt;
        }

        protected void UsersbtnExportPDF_Click(object sender, EventArgs e)
{
    DataTable dataTable = GetUsersDataFromProcedure(); // Get data from stored procedure
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("The DataTable is empty. No data returned from the procedure.");
                return; // Exit if there's no data
            }
            foreach (DataRow row in dataTable.Rows)
            {
                System.Diagnostics.Debug.WriteLine("Row: " + row["username"] + ", " + row["roleid"]);
            }

            ReportDocument rptDoc = new ReportDocument();
    string reportPath = Server.MapPath("~/UsersReport.rpt"); // Path to Crystal Report file
    rptDoc.Load(reportPath); // Load the Crystal Report
    rptDoc.SetDataSource(dataTable); // Set data source to the DataTable

    string directoryPath = Server.MapPath("~/Reports");
    string filePath = Path.Combine(directoryPath, "UsersReport.pdf");
    if (!Directory.Exists(directoryPath))
    {
        Directory.CreateDirectory(directoryPath); // Create directory if not exists
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
            DataTable dataTable = GetRolesDataFromProcedure();

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
                Session["UsersDs"] = dt;

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

            Session["RolesDs"] = dt;
            }
            Debug.Write(" call \n");
            RolesGridView.DataBind();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Debug.Write("Here Page Init \n");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Debug.Write("Here Page Load \n");
                Debug.Write("Here BindUsersGridView call \n");
                BindUsersGridView();
                Debug.Write("Here BindRolesGridView call \n");
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

                string checkQuery = "SELECT COUNT(1) FROM [dbo].[Users] WHERE Username = @Username";
                bool usernameExists = false;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Username", username);

                    conn.Open();
                    usernameExists = Convert.ToBoolean(checkCmd.ExecuteScalar());
                    conn.Close();
                }

                if (usernameExists)
                {
                    e.Cancel = true;
                    UsersGridView.JSProperties["cpMessage"] = "Username already exists. Please choose a different username.";
                    return;
                }

                string insertQuery = "INSERT INTO [dbo].[Users] (Username, Password, RoleId) VALUES (@Username, @Password, @RoleId)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand insertCmd = new SqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@Username", username);
                    insertCmd.Parameters.AddWithValue("@Password", hashedPassword);
                    insertCmd.Parameters.AddWithValue("@RoleId", RoleId ?? DBNull.Value);

                    conn.Open();
                    insertCmd.ExecuteNonQuery();
                    conn.Close();
                }

                e.Cancel = true;
                UsersGridView.CancelEdit();

            }
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
                    Session["UsersDs"] = dt;
                }
            }
            }

        protected void UsersGridView_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
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
            }
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
                    Session["UsersDs"] = dt;
                }
            }
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
            {   string username = e.Keys["Username"].ToString();
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
        }
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
                    Session["UsersDs"] = dt;
                }
            } }

        protected void RolesGridView_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            if (e.NewValues["Name"] != null)
            {
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
                }
            }
            {
                string query = "SELECT Id, Name FROM Roles";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    Session["RolesDs"] = dt;
                }

            }
        }

        protected void RolesGridView_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
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
        }
            {
                string query = "SELECT Id, Name FROM Roles";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    Session["RolesDs"] = dt;
                }
            }
        }

        protected void RolesGridView_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            int RoleId = Convert.ToInt32(e.Keys["Id"]);
            if (IsRoleAssignedToUsers(RoleId))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "script", "alert('Cannot delete the role as it is assigned to one or more users.')", true);
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
            }
            {
                string query = "SELECT Id, Name FROM Roles";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    Session["RolesDs"] = dt;
                }
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

        protected void UsersGridView_DataBinding(object sender, EventArgs e)
        {
            Debug.Write("Here Data Binding\n");
            UsersGridView.DataSource = Session["UsersDs"];        
        }

        protected void RolesGridView_DataBinding(object sender, EventArgs e)
        {       
            RolesGridView.DataSource = Session["RolesDs"];
        }
    }
}