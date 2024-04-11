using Microsoft.Data.SqlClient;

namespace WebApi
{
    public class Group
    {
        private static string? connectionString = null;

        public int GroupId { get; set; }
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public int OwnerId { get; set; }
        public List<string>? Members {  get; set; }
        public List<string>? Tags { get; set; }

        /**
         * GetGroup - Returns the group given the specific id
         * @param id - Id for the group
         * @return - The data for the group
         */
        public static Group? GetGroup(int id)
        {
            SetConnectionString();
            try
            {
                Group group = new Group();
                group.GroupId = id;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT OwnerID, GroupName, GroupBio FROM Groups WHERE GroupID = @GID";
                        cmd.Parameters.AddWithValue("@GID", id);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            group.OwnerId = reader.GetInt32(0);
                            group.Name = reader.GetString(1);
                            group.Bio = reader.GetString(2);

                            group.Members = GetMembers(id);
                            group.Tags = GetTags(id);
                        }
                    }
                }

                return group;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /**
         * GetMembers - helper method that retrieves the data from the database
         * @param id - Group Id the members are apart of
         * @return - the list of names of the group members, null if error
         */
        private static List<string>? GetMembers(int id)
        {
            SetConnectionString();
            try
            {
                List<string> members = new List<string>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT UserName FROM MemberGroup JOIN Customers " +
                                            "ON MemberGroup.UserID = Customers.UserID " +
                                            "WHERE GroupID = @GroupP";
                        cmd.Parameters.AddWithValue("@GroupP", id);

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            members.Add(reader.GetString(0));
                        }
                    }
                }
                return members;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /**
         * GetTags - helper method that retrieves the data from the database
         * @param id - Group Id the members are apart of
         * @return - the list of names of the group members, null if error
         */
        private static List<string>? GetTags(int id)
        {
            SetConnectionString();
            try
            {
                List<string> tags = new List<string>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Name FROM GroupTags " +
                                            "JOIN Tags ON Tags.TagID = GroupTags.TagID " +
                                            "WHERE GroupID = @GroupP;";
                        cmd.Parameters.AddWithValue("@GroupP", id);

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            tags.Add(reader.GetString(0));
                        }
                    }
                }
                return tags;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static void SetConnectionString()
        {
            if (connectionString == null)
            {
                IConfigurationRoot builder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                connectionString = builder.GetConnectionString("sqlServerString");
            }
        }

        public static int CreateGroup(Dictionary<string,string> data) 
        {
            SetConnectionString();
            try
            {
                return -1;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private static int GetGroupId(string groupName)
        {
            SetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        
                    }
                }
                return -1;

            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private static bool DuplicateGroupName(string groupName)
        {
            SetConnectionString();
            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT groupId, ";
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                return true;
            }
        }

        public static int AddMemberToGroup(int groupId, int userId)
        {
            SetConnectionString();
            return 1;
        }
    }
}
