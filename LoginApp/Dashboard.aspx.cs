using System;
using System.Data;
using System.Data.SqlClient;
using DevExpress.Web;

namespace LoginApp
{
    public partial class Dashboard : System.Web.UI.Page
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TrainingApp;Integrated Security=True";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUsersGridView();
                BindRolesGridView();
            }
        }

        private void BindUsersGridView()
        {
            string query = "SELECT Username, Password, RoleId FROM [dbo].[Users]";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string storedHash = reader["Password"].ToString();
                    byte[] hashBytes = Convert.FromBase64String(storedHash);
                    Response.Write(hashBytes);
                    conn.Close();

                }
                DataTable dt = new DataTable();
                da.Fill(dt);
                ASPxGridView1.DataSource = dt;
                ASPxGridView1.DataBind();
            }
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
            }
        }

        protected void ASPxGridView1_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            if (e.NewValues["Username"] != null && e.NewValues["Password"] != null)
            {
                string username = e.NewValues["Username"].ToString();
                string password = PasswordHelper.HashPassword( e.NewValues["Password"].ToString());
                int roleId = Convert.ToInt32(e.NewValues["RoleId"]);

                string query = "INSERT INTO [dbo].[Users] (Username, Password, RoleId) VALUES (@Username, @Password, @RoleId)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
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
                cmd.Parameters.AddWithValue("@Password",PasswordHelper.HashPassword( password));
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

            if (IsRoleAssignedToUsers(roleId))
            {
                ASPxGridView2.JSProperties["cpMessage"] = "Cannot delete the role as it is assigned to one or more users.";
                e.Cancel = true;
            }
            else
            {
                string query = "DELETE FROM [dbo].[Roles] WHERE RoleId = @RoleId";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RoleId", roleId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                e.Cancel = true;
                BindRolesGridView();
            }
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
    }
}
