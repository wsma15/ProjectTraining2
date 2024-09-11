using System;
using System.Data.SqlClient;
using System.Linq;

namespace LoginApp
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Loginbtn_Click(object sender, EventArgs e)
        {
            try
            {
                string username = Usernametxt.Text.ToUpper();
                string password = Passwordtxt.Text;
                string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=TrainingApp;Trusted_Connection=True;";

                // Fetch the stored password hash and role ID
                string query = "SELECT Password, RoleId FROM Users WHERE Username = @Username";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string storedHash = reader["Password"].ToString();
                        int roleId = Convert.ToInt32(reader["RoleId"]);

                        // Verify the password
                        if (PasswordHelper.VerifyPassword(password, storedHash))
                        {
                            // Redirect based on role
                            switch (roleId)
                            {
                                case 1:
                                    Response.Redirect("Dashboard.aspx"); // Admin
                                    break;
                                case 2:
                                    Response.Write("UserDashboard.aspx"); // User
                                    break;
                                default:
                                    Response.Write("Access denied."); // Handle other roles if necessary
                                    break;
                            }
                        }
                        else
                        {
                            Response.Write("Incorrect Username or Password.");
                        }
                    }
                    else
                    {
                        Response.Write("Incorrect Username or Password.");
                    }
                }
            }
            catch (Exception)
            {

                Response.Write("An error occurred. Please try again later.");
            }
        }
    }
}
