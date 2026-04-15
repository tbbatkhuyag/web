using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connStr = "Data Source=103.48.116.81; Database=iumc_db; User Id=sa; Password=Sun_222; Trusted_Connection=false; MultipleActiveResultSets=true;TrustServerCertificate=True";
        
        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            try {
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 20 Id, Username, Rolename, Userpass FROM [user]", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"User: {reader["Username"]}, Role: {reader["Rolename"]}, Pass: {reader["Userpass"]}");
                        }
                    }
                }
            } catch(Exception e) { Console.WriteLine(e.Message); }
            
            InsertUser("testadmin", "admin", "admin", conn);
            InsertUser("testteacher", "teacher", "teacher", conn);
            InsertUser("teststudent", "student", null, conn);
        }
    }
    
    public static void InsertUser(string username, string password, string role, SqlConnection conn)
    {
        string sql = $"IF NOT EXISTS (SELECT 1 FROM [user] WHERE Username='{username}') INSERT INTO [user] (Username, Userpass, Rolename) VALUES ('{username}', @pass, {(role == null ? "NULL" : "@role")})";
        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@pass", HashPassword(password));
            if (role != null) cmd.Parameters.AddWithValue("@role", role);
            int rows = cmd.ExecuteNonQuery();
            if (rows > 0) Console.WriteLine($"Inserted {username} with password '{password}'");
        }
    }

    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashedBytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
