using DevExpress.Web;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace LoginApp
{
    public partial class Dashboard : System.Web.UI.Page
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrainingApp;Integrated Security=True";

        private void BindUsersGridView()
        {
            string query = "SELECT Username, Password, RoleId FROM [dbo].[Users]";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                conn.Open();
                da.Fill(dt); // Use Fill to directly load the data into the DataTable
                conn.Close();

                foreach (DataRow row in dt.Rows)
                {
                    string encryptedPassword = row["Password"].ToString();
                    string decryptedPassword = PasswordHelper.Decrypt(encryptedPassword);
                    row["Password"] = decryptedPassword;
                }

                ASPxGridView1.DataSource = dt;
                ASPxGridView1.DataBind();
                ASPxGridView1.EnableViewState = false;
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

        private bool IsRoleAssignedToUsers(int roleId)
        {
            bool isAssigned = false;
            string query = "SELECT COUNT(*) FROM [dbo].[Users] WHERE RoleId = @RoleId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoleId", roleId);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                conn.Close();

                isAssigned = count > 0;
            }
            return isAssigned;
        }

        private void BindRolesGridView()
        {
            string query = "SELECT RoleId, RoleName FROM [dbo].[Roles]";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ASPxGridView2.DataSource = dt;
                ASPxGridView2.DataBind();
                ASPxGridView2.EnableViewState = false;

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
 
        protected void ASPxGridView1_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            if (e.NewValues["Username"] != null && e.NewValues["Password"] != null)
            {
                string username = e.NewValues["Username"].ToString();
                string password = e.NewValues["Password"].ToString();
                string hashedPassword = PasswordHelper.HashPassword(password);
                int roleId = Convert.ToInt32(e.NewValues["RoleId"]);

                if (!IsValidUsername(username))
                {
                    e.Cancel = true;
                    ASPxGridView1.JSProperties["cpMessage"] = "Username must be at least 6 characters long, and contain no spaces or special characters.";
                    return;
                }

                if (!IsValidPassword(password))
                {
                    e.Cancel = true;
                    ASPxGridView1.JSProperties["cpMessage"] = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
                    return;
                }

                string query = "INSERT INTO [dbo].[Users] (Username, Password, RoleId) VALUES (@Username, @Password, @RoleId)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                e.Cancel = true;
                ASPxGridView1.CancelEdit();
                BindUsersGridView();
            }
        }

        protected void ASPxGridView1_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            string username = e.Keys["Username"].ToString();
            string password = e.NewValues["Password"].ToString();
            int roleId = Convert.ToInt32(e.NewValues["RoleId"]);

            string query = "UPDATE [dbo].[Users] SET Password = @Password, RoleId = @RoleId WHERE Username = @Username";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", PasswordHelper.HashPassword(password));
                cmd.Parameters.AddWithValue("@RoleId", roleId);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            e.Cancel = true;
            ASPxGridView1.CancelEdit();
            BindUsersGridView();
        }

        protected void ASPxGridView1_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
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

        protected void ASPxGridView2_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            if (e.NewValues["RoleName"] != null)
            {
                string roleName = e.NewValues["RoleName"].ToString();

                string query = "INSERT INTO [dbo].[Roles] (RoleName) VALUES (@RoleName)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RoleName", roleName);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                e.Cancel = true;
                ASPxGridView2.CancelEdit();
                BindRolesGridView();
            }
        }

        protected void ASPxGridView2_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            string roleId = e.Keys["RoleId"].ToString();
            string roleName = e.NewValues["RoleName"].ToString();

            string query = "UPDATE [dbo].[Roles] SET RoleName = @RoleName WHERE RoleId = @RoleId";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RoleName", roleName);
                cmd.Parameters.AddWithValue("@RoleId", roleId);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            e.Cancel = true;
            ASPxGridView2.CancelEdit();
            BindRolesGridView();
        }

        protected void ASPxGridView2_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            int roleId = Convert.ToInt32(e.Keys["RoleId"]);

            // Check if the role is assigned to any users
            if (IsRoleAssignedToUsers(roleId))
            {
                // Display a message to the user
                ASPxGridView2.JSProperties["cpMessage"] = "Cannot delete the role as it is assigned to one or more users.";
                e.Cancel = true; // Cancel the delete operation
            }
            else
            {
                // Proceed with deletion
                string query = "DELETE FROM [dbo].[Roles] WHERE RoleId = @RoleId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                // Cancel the default delete operation and manually refresh the GridView
                e.Cancel = true;
                BindRolesGridView();
            }
        }

        protected void ASPxGridView1_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"CustomCallback triggered. Parameters: {e.Parameters}");
            BindUsersGridView();
        }

        protected void ASPxGridView1_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            foreach (GridViewColumn row in ASPxGridView1.Columns)
            {
                GridViewDataColumn dataRow = row as GridViewDataColumn;
                if (dataRow == null) continue;
                if (e.NewValues[dataRow.FieldName] == null)
                    e.Errors[dataRow] = "Value cannot be null.";
            }

            if ((e.NewValues["Username"]) != null &&
                !IsValidUsername(e.NewValues["Username"].ToString()))
            {
                e.Errors[(GridViewDataColumn)ASPxGridView1.Columns["Username"]] = "The input must be at least 6 characters long, only contain letters and numbers, and have no special characters.";
            }

            if ((e.NewValues["Password"]) != null &&
                !IsValidPassword(e.NewValues["Password"].ToString()))
            {
                e.Errors[(GridViewDataColumn)ASPxGridView1.Columns["Password"]] = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
            }
            if (e.Errors.Count > 0) e.RowError = "Please fill in all fields.";
        }
        
        protected void ASPxGridView2_RowValidating(object sender, DevExpress.Web.Data.ASPxDataValidationEventArgs e)
        {
            // Check if the "RoleName" value is null
            if (e.NewValues["RoleName"] == null)
            {
                e.Errors[ASPxGridView2.Columns["RoleName"]] = "Value cannot be null.";
            }
            else
            {
                // Validate the "RoleName" value using the IsValidName method
                if (!IsValidName(e.NewValues["RoleName"].ToString()))
                {
                    e.Errors[ASPxGridView2.Columns["RoleName"]] = "The name must contain only letters and have at least two words.";
                }
            }

            // Set a general error if any specific errors are found
            if (e.Errors.Count > 0)
            {
                e.RowError = "Please fill in all fields correctly.";
            }
        }
    }
}
