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
                        cmd.CommandText = $"SELECT UserName FROM tavern.dbo.Customers WHERE UserID = {id}";
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

        public static string GetFriends(int id)
        {
            try
            {
                string friends = "";
                List<int> ids = new List<int>();
                using (SqlConnection  conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT UserID_1, UserID_2 FROM tavern.dbo.PrivateChat WHERE Relationship='F' AND (UserID_1={id} OR UserID_2={id})";
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (reader.GetInt32(0) == id)
                                ids.Add(reader.GetInt32(1));
                            else
                                ids.Add(reader.GetInt32(0));
                        }
                    }
                    conn.Close();
                    if (ids.Count < 0)
                        return "";
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT UserID, UserName FROM tavern.dbo.Customers";
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            if (ids.Contains(reader.GetInt32(0)))
                                friends += reader.GetString(1) + ",";
                        }
                    }
                }
                return friends;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}
