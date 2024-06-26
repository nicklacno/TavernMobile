﻿using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace WebApi
{
    //Universal Error codes for groups
    //-10, group no longer exists
    public class Group
    {
        private static string? connectionString = null;

        public int GroupId { get; set; }
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public int OwnerId { get; set; }
        public List<string>? Members { get; set; }
        public List<Tag>? Tags { get; set; }

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
        private static List<Tag>? GetTags(int id)
        {
            SetConnectionString();
            try
            {
                List<Tag> tags = new List<Tag>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT GroupTags.TagID, Name FROM GroupTags " +
                                            "JOIN Tags ON Tags.TagID = GroupTags.TagID " +
                                            "WHERE GroupID = @GroupP;";
                        cmd.Parameters.AddWithValue("@GroupP", id);

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            tags.Add(new Tag { TagId = reader.GetInt32(0), TagName = reader.GetString(1) });
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
        public static int CreateGroup(Dictionary<string, string> data)
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
                        if (data.ContainsKey("bio") && data["bio"] != null)
                        {
                            cmd.CommandText = "INSERT INTO Groups (OwnerID, GroupName, GroupBio) VALUES (@Owner, @Name, @Bio)";
                            cmd.Parameters.AddWithValue("@Bio", data["bio"]);
                        }
                        else
                        {
                            cmd.CommandText = "INSERT INTO Groups (OwnerID, GroupName) VALUES (@Owner, @Name)";
                        }

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
                    using (SqlCommand cmd = conn.CreateCommand())
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
        public static int JoinRequest(string groupId, string userId)
        {
            SetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO GroupRequests (UserID, GroupID) VALUES (@User, @Group)";
                        cmd.Parameters.AddWithValue("@User", userId);
                        cmd.Parameters.AddWithValue("@Group", groupId);

                        cmd.ExecuteNonQuery();
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
                Console.WriteLine(ex.Message);
            }
        }

        public static int EditGroup(Dictionary<string, string> data)
        {
            if (!Exists(Convert.ToInt32(data["groupId"]))) return -10;
            if (data["newName"] == null && data["newBio"] == null) return 0;
            if (DuplicateGroupName(data["newName"])) return -3;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
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
            SetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT GroupID, UserID FROM GroupRequests WHERE RequestID = @req";
                        cmd.Parameters.AddWithValue("@req", requestId);

                        var reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            DeleteFromTable(reader.GetInt32(0), reader.GetInt32(1));
                            return ProcessModification(reader.GetInt32(0), reader.GetInt32(1), isAccepted);
                        }
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        private static void DeleteFromTable(int groupId, int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM GroupRequests WHERE GroupID = @group AND UserID = @user";
                        cmd.Parameters.AddWithValue("@group", groupId);
                        cmd.Parameters.AddWithValue("@user", userId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static int ProcessModification(int groupId, int userId, bool isAccepted)
        {
            if (!isAccepted) return 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO MemberGroup (GroupID, UserID) VALUES (@group, @user);";
                        cmd.Parameters.AddWithValue("@group", groupId);
                        cmd.Parameters.AddWithValue("@user", userId);

                        cmd.ExecuteNonQuery ();
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public static List<Dictionary<string, string>> Chat(int id, Dictionary<string, string> data)
        {
            SetConnectionString();
            if (!Exists(id)) return null;
            List<Dictionary<string, string>> log = new List<Dictionary<string, string>>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        string query = "SELECT UserName, Message, TimeStamp FROM Messages " +
                            "JOIN Customers ON UserID = SenderID WHERE GroupChatID = @Group";
                        if (data.ContainsKey("timestamp") && data["timestamp"] != null)
                        {
                            query += " AND TimeStamp > @TimeStamp";
                            cmd.Parameters.AddWithValue("@TimeStamp", Convert.ToDateTime(data["timestamp"]));
                        }
                        query += " ORDER BY TimeStamp ASC";

                        cmd.CommandText = query;
                        cmd.Parameters.AddWithValue("@Group", id);

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Dictionary<string, string> message = new Dictionary<string, string>()
                            {
                                {"sender", reader.GetString(0) },
                                {"message", reader.GetString(1) }
                            };

                            message["timestamp"] = reader.IsDBNull(2) ? null : reader.GetDateTime(2).ToString();

                            log.Add(message);
                        }
                    }
                }
                return log;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static int SendMessage(int id, Dictionary<string, string> data)
        {
            SetConnectionString();
            if (!Exists(id)) return -10;
            if (!IsInGroup(id, Convert.ToInt32(data["senderId"]))) return -2;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Messages(GroupChatID, SenderID, Message, TimeStamp) " +
                            "VALUES (@groupid, @userid, @message, @time );";
                        cmd.Parameters.AddWithValue("@groupid", id);
                        cmd.Parameters.AddWithValue("@userid", Convert.ToInt32(data["senderId"]));
                        cmd.Parameters.AddWithValue("@message", data["message"]);
                        cmd.Parameters.AddWithValue("@time", DateTime.UtcNow);

                        cmd.ExecuteNonQuery();
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }
        public static List<Group> GetRandomGroupsForUser(int userId)
        {
            SetConnectionString();
            List<Group> randomGroups = new List<Group>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT TOP 100 GroupID FROM Groups g " +
                                          "WHERE (SELECT COUNT(GroupID) FROM MemberGroup " +
                                          "WHERE GroupID = g.GroupID " +
                                          "AND UserID = @UserID) = 0 " +
                                          "ORDER BY NEWID()";
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Group? group = GetGroup(reader.GetInt32(0));
                            if (group != null)
                            {
                                randomGroups.Add(group);
                            }
                        }
                    }
                }
                return randomGroups;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }


        private static bool IsInGroup(int group, int user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT UserID FROM MemberGroup WHERE GroupID = @Group AND UserID = @User";
                        cmd.Parameters.AddWithValue("@Group", group);
                        cmd.Parameters.AddWithValue("@User", user);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read()) return true;
                        else return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<Tag> GetTags()
        {
            SetConnectionString();
            try
            {
                List<Tag> tags = new List<Tag>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT TagID, Name FROM Tags WHERE ForGroup = 1;";
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            tags.Add(new Tag { TagId = reader.GetInt32(0), TagName = reader.GetString(1) });
                        }
                        return tags;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }

        //Assumes connection string already initialized
        private static bool Exists(int groupid)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT GroupID FROM Groups WHERE GroupID = @id";
                        cmd.Parameters.AddWithValue("@id", groupid);
                        SqlDataReader r = cmd.ExecuteReader();
                        return r.HasRows;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static int AddTag(Dictionary<string, int> data)
        {
            SetConnectionString();
            if (!Exists(data["groupId"])) return -10;
            if (!IsValidTag(data["tagId"])) return -9;
            if (AlreadyInTable(data["groupId"], data["tagId"])) return 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO GroupTags (TagID, GroupID) VALUES (@tag, @group);";
                        cmd.Parameters.AddWithValue("@tag", data["tagId"]);
                        cmd.Parameters.AddWithValue("@group", data["groupId"]);
                        cmd.ExecuteNonQuery();

                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public static int RemoveTag(Dictionary<string, int> data)
        {
            SetConnectionString();
            if (!AlreadyInTable(data["groupId"], data["tagId"])) return 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM GroupTags WHERE GroupID = @group AND TagID = @tag";
                        cmd.Parameters.AddWithValue("@group", data["groupId"]);
                        cmd.Parameters.AddWithValue("@tag", data["tagId"]);

                        cmd.ExecuteNonQuery();
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        private static bool IsValidTag(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT TagID FROM Tags WHERE TagID = @id AND ForGroup = 1";
                        cmd.Parameters.AddWithValue("@id", id);
                        return cmd.ExecuteReader().HasRows;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        private static bool AlreadyInTable(int group, int tag)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT TagID FROM GroupTags WHERE TagID = @tag AND GroupID = @group";
                        cmd.Parameters.AddWithValue("@tag", tag);
                        cmd.Parameters.AddWithValue("@group", group);
                        return cmd.ExecuteReader().HasRows;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return true;
            }
        }

        public static List<Request> Requests(int id)
        {
            SetConnectionString();
            try
            {
                List<Request> requests = new List<Request>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT RequestID, GroupID, c.UserID, UserName FROM GroupRequests r " +
                            "JOIN Customers c ON r.UserID = c.UserID WHERE GroupID = @id";
                        cmd.Parameters.AddWithValue("@id", id);
                        
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            requests.Add(new Request
                            {
                                RequestId = reader.GetInt32(0),
                                GroupId = reader.GetInt32(1),
                                ProfileId = reader.GetInt32(2),
                                ProfileName = reader.GetString(3)
                            });
                        }
                        return requests;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
