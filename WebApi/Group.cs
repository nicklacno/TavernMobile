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
                            if (reader.GetValue(2) != System.DBNull.Value)
                            {
                                group.Bio = reader.GetString(2);
                            }

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
        
        /**
         * CreateGroup - returns the newly created group's id or negative if failed
         * @param data - the dictionary that holds all the data
         * @return - the new group id or a negative number
         */
        public static int CreateGroup(Dictionary<string,string> data) 
        {
            SetConnectionString();
            if (GetGroupId(data["name"]) != -1) return -2;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString)) 
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Groups (OwnerID, GroupName) VALUES (@Owner, @Name)";
                        cmd.Parameters.AddWithValue("@Owner", Convert.ToInt32(data["ownerId"]));
                        cmd.Parameters.AddWithValue("@Name", data["name"]);
                        
                        cmd.ExecuteNonQuery();
                    }
                }
                int id = GetGroupId(data["name"]);
                AddMemberToGroup(id, Convert.ToInt32(data["ownerId"]));
                return id;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        
        /**
         * GetGroupId - returns the groupid given a specific group name
         * @param groupName - the name of the group
         * @return - the groupId or -1 if not found
         */
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
                        cmd.CommandText = "SELECT GroupID FROM Groups WHERE GroupName = @GroupName";
                        cmd.Parameters.AddWithValue("@GroupName", groupName);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            return reader.GetInt32(0);
                        }
                        return -1;
                    }

                }
                return -1;

            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        

        /**
         * AddMemberToGroup - helper function that allows database manipulation
         * @param groupId - the group that will be joined
         * @param userId - the user that will be added to the group
         * @return - negative number if failed, 0 if success
         */
        private static int AddMemberToGroup(int groupId, int userId)
        {
            SetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO MemberGroup(GroupID, UserID) VALUES (@GroupId, @UserId)";
                        cmd.Parameters.AddWithValue("@GroupId", groupId);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        cmd.ExecuteNonQuery();
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        /**
         * JoinRequest - Creates a new request for joining a group
         * @param groupId - the id of the group to join
         * @param userId - the id of the user that wishes to join
         */
        public static void JoinRequest(int groupId, int userId)
        {
            SetConnectionString();
            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO GroupRequests (UserID, GroupID) VALUES (@User, @Group)";
                        cmd.Parameters.AddWithValue("@User", userId);
                        cmd.Parameters.AddWithValue("@Group", groupId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static int EditGroup(Dictionary<string, string> data)
        {
            if (data["newName"] == null && data["newBio"] == null) return 0;
            if (DuplicateGroupName(data["newName"])) return -3;
            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString)) 
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        string command = "UPDATE Groups SET ";
                        if (data["newName"] != null)
                        {
                            command += "GroupName = @UserNameP ";
                            cmd.Parameters.AddWithValue("@UserNameP", data["newName"]);
                        }
                        if (data["newBio"] != null)
                        {
                            command += "Bio = @BioP ";
                            cmd.Parameters.AddWithValue("@BioP", data["newBio"]);
                        }
                        command += "WHERE OwnerId = @Owner AND GroupID = @Group;";
                        cmd.CommandText = command;
                        cmd.Parameters.AddWithValue("@Owner", Convert.ToInt32(data["ownerId"]));
                        cmd.Parameters.AddWithValue("@Group", Convert.ToInt32(data["groupId"]));

                        cmd.ExecuteNonQuery();
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private static bool DuplicateGroupName(string newName)
        {
            SetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT GroupID FROM Groups WHERE GroupName = @GroupP";
                        cmd.Parameters.AddWithValue("@GroupP", newName);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        /**
* ModifyRequest - Modifies a request to join the group
* @param requestId - The id for the given request
* @param isAccepted - whether or not to accept or reject the request
*/
        public static int ModifyRequest(int requestId, bool isAccepted)
        {
            throw new NotImplementedException();
        }
    }
}
