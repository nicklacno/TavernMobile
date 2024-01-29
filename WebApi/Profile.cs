using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace WebApi
{
    public class Profile
    {
        private const string connectionString = 
            "Server = satou.cset.oit.edu, 5433; " +
            "Database = tavern; " +
            "User Id = tavern; " +
            "Password = T@vern5!; " +
            "TrustServerCertificate=true";

        public Profile (int id, string name, string bio) 
        {
            Id= id;
            Name= name;
            Bio= bio;
        }

        public Profile()
        {
            Id = -1;
            Name = "";
            Bio = "";
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }

        public static Profile GetProfile(int id)
        {
            try
            {
                Profile profile = new Profile();
                profile.Id= id;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT UserName FROM tavern.dbo.Customer WHERE UserID = {id}";
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            profile.Name = reader.GetString(0);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    conn.Close();
                }

                return profile;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}
