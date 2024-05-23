using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;

namespace WebApi
{
    /**
     * Profile - Handles retrieving any data refering to a profile
     */
    public class Profile
    {
        //connection string
        private static string connectionString = null;

        /**
         * Profile - Basic Profile Constructor that initializes any values passed to it
         * @param id - Profile ID
         * @param name - Profile Username
         * @param bio - Profile Bio
         */
        public Profile(int id = -1, string name = "", string bio = "")
        {
            Id = id;
            Name = name;
            Bio = bio;
        }
        //Basic setters and getters
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Bio { get; private set; }

        /**
         * GetProfile - Static Method that returns a Profile object or null
         * @param id - Id of the profile
         * @return valid Profile Data or null if not found
         */
        public static Profile? GetProfile(int id)
        {
            SetConnectionString();
            try
            {
                Profile profile = new Profile(id); //Creates Profile to store data

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT UserName, Bio FROM tavern.dbo.Customers WHERE UserID = @IdP"; //Query to get data
                        cmd.Parameters.AddWithValue("@IdP", id); //Adding using parameters to prevent injection, though it cant be accessed by users
                        SqlDataReader reader = cmd.ExecuteReader(); //Run query and pass data to reader
                        if (reader.Read()) //If statement because there should only be one
                        {
                            profile.Name = reader.GetString(0);//Sets Profile Name
                            if (!reader.IsDBNull(1))
                                profile.Bio = reader.GetString(1);//Sets Profile Bio
                        }
                        else //if reader has no rows, sets profile to null to return null
                        {
                            throw new Exception("Missing Profile");
                        }
                    }
                    conn.Close(); //closes connection
                }

                return profile; //return final value of profile
            }
            catch (Exception e) //If error, return null
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        /**
         * GetFriends - Returns a Json of Friends
         * @param id - Profile Id to retrieve friends
         * @return - Json of Friends, empty if none, null if error along the way
         */
        public static List<MiniInfo>? GetFriends(int id)
        {
            SetConnectionString();
            try
            {
                
                List<MiniInfo> ids = new List<MiniInfo>(); //List that stores the friend's ID's
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open(); //Opens connection to database
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT UserID_2, UserName FROM Relationships JOIN Customers ON UserID_2 = UserID " +
                            "WHERE Relationship='F' AND UserID_1 = @ProfileID"; //Query
                        cmd.Parameters.AddWithValue("@ProfileID", id);
                        SqlDataReader reader = cmd.ExecuteReader(); //Passes friends ids to reader
                        while (reader.Read()) //Grabs all friend ids and adds them to ids list
                        {
                            ids.Add(new MiniInfo { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                        }
                    }

                }
                return ids; //returns the json of the friends names

            }
            catch (Exception e) //returns null if error
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        /**
         * GetProfileId - Method to return the id of a profile when using id 
         * @param username - username for the profile
         * @param password - password for the profile
         * @returns - the id given the username and password, -1 if failed
         */
        public static int GetProfileId(string username, string password)
        {
            SetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT UserID,SaltedPassword,Salt FROM Customers WHERE UserName = @UserP";
                        cmd.Parameters.AddWithValue("@UserP", username); //query parameter

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string temp = Convert.ToBase64String(HashPassword(password, Convert.FromBase64String(reader["Salt"].ToString())));
                            if (string.Equals(reader["SaltedPassword"].ToString(), temp)) //salted passwords match
                            {
                                return reader.GetInt32(0);  //return id of profile
                            }
                        }
                        conn.Close();
                        return -1; //not found, return -1
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return -1; //error, return -1;
            }
        }

        /**
         * HashPassword - Helper function that hashes the password for inserting or retrieving
         * @param password - unsalted password
         * @param salt - salt used for hashing
         * @return - the salted password
         */
        private static byte[] HashPassword(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1, 6, 48);
        }

        /**
         * GetGroups - calls the database and returns a json of a list of group names
         * @param id - the id of the user
         * @return - the json of the users groups, null if error, empty if none
         */
        public static List<Group>? GetGroups(int id)
        {
            SetConnectionString();
            try
            {
                List<Group> groups = new List<Group>();//creates list of group names

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT GroupID FROM MemberGroup WHERE UserID = @IdParam;";
                        cmd.Parameters.AddWithValue("@IdParam", id);

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read()) //add to list of groups if has rows
                        {
                            Group? g = Group.GetGroup(reader.GetInt32(0));
                            if (g != null)
                            {
                                groups.Add(g);
                            }
                        }
                    }
                    conn.Close();
                }

                return groups; //serializes list of group names to a json string
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null; //returns null if empty
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
         * Register - Checks information for data already in table
         * @param data - The Dictionary of data that has all the data for the account
         * @return - The profile id or an error code
         */
        public static int Register(Dictionary<string, string> data)
        {
            if (DuplicateUsername(data["username"]))
                return -2;
            if (DuplicateEmail(data["email"]))
                return -3;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        byte[] salt = GetSalt();
                        cmd.CommandText = " INSERT INTO Customers(UserName, Bio, SaltedPassword, Salt, UserEmail, UserCity, UserState) " +
                            "VALUES (@UserName, null, @SaltedPassword, @Salt, @UserEmail, @UserCity, @UserState);";
                        cmd.Parameters.AddWithValue("@UserName", data["username"]);
                        cmd.Parameters.AddWithValue("@SaltedPassword", Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                data["password"],
                                salt,
                                KeyDerivationPrf.HMACSHA1,
                                6,
                                48)));
                        cmd.Parameters.AddWithValue("@Salt", Convert.ToBase64String(salt));
                        cmd.Parameters.AddWithValue("@UserEmail", data["email"]);
                        cmd.Parameters.AddWithValue("@UserCity", data["city"]);
                        cmd.Parameters.AddWithValue("@UserState", data["state"]);

                        cmd.ExecuteNonQuery();
                    }
                }
                return GetProfileId(data["username"], data["password"]);
            }
            catch (Exception ex)
            { return -1; }
        }

        static byte[] GetSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[48];
            rng.GetNonZeroBytes(salt);
            return salt;
        }
        /**
         * Checks in the database if the username already exists
         * @param name - the name of the user trying to be added/modified
         * @return - whether the name was already in the database or not
         */
        static bool DuplicateUsername(string name)
        {
            SetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT UserID FROM Customers WHERE UserName = @UserP";
                        cmd.Parameters.AddWithValue("@UserP", name);

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
         * Checks in the database if the email already exists
         * @param email - the email of the user trying to be added
         * @return - whether the email was already in the database or not
         */
        static bool DuplicateEmail(string email)
        {
            SetConnectionString();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT UserID FROM Customers WHERE UserEmail = @EmailP";
                        cmd.Parameters.AddWithValue("@EmailP", email);

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
         * EditProfile - Makes proper modifications based on data given
         * @param data - the dictionary that stores the data to perform the action
         * @return - correct status code, 0 for modified, anything else will be error
         */
        public static int EditProfile(Dictionary<string, string> data) //newBio and/o newUsername should be null if no changes
        {
            if (!VerifyCredentials(data["profileId"], data["password"])) return -2;
            if (data["newUsername"] != null && DuplicateUsername(data["newUsername"])) return -3;
            if (data["newUsername"] == null && data["newBio"] == null) return 0; //if no data needed to be changed, then it success
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        string command = "UPDATE Customers SET ";
                        if (data["newUsername"] != null)
                        {
                            command += "UserName = @UserNameP ";
                            cmd.Parameters.AddWithValue("@UserNameP", data["newUsername"]);
                        }
                        if (data["newBio"] != null)
                        {
                            command += "Bio = @BioP ";
                            cmd.Parameters.AddWithValue("@BioP", data["newBio"]);
                        }
                        command += "WHERE UserID = @IdP;";
                        cmd.CommandText = command;
                        cmd.Parameters.AddWithValue("@IdP", Convert.ToInt32(data["profileId"]));

                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0) return -10;
                    }
                }
                return 0;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        /**
         * Helper function that verifies the cridentials for making modifications to the profile
         * @param id - the string of the id for the user
         * @param password - the password for the user
         * @return - whether they are valid to modify this profile
         */
        static bool VerifyCredentials(string id, string password)
        {
            SetConnectionString();
            int profileId = Convert.ToInt32(id);
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Salt, SaltedPassword FROM Customers WHERE UserId = @UserId";
                        cmd.Parameters.AddWithValue("@UserId", profileId);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string temp = Convert.ToBase64String(HashPassword(password, Convert.FromBase64String(reader["Salt"].ToString())));
                            if (string.Equals(reader["SaltedPassword"].ToString(), temp))
                            {
                                return true;
                            }
                            return false;
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
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
                        cmd.CommandText = "SELECT TagID, Name FROM Tags WHERE ForPlayer = 1;";
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

        public static List<Tag> GetProfileTags(int id)
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
                        cmd.CommandText = "SELECT Tags.TagID, Name FROM PlayerTags " +
                                          "JOIN Tags ON Tags.TagID = PlayerTags.TagID " +
                                          "WHERE PlayerID = @id";
                        cmd.Parameters.AddWithValue("@id", id);

                        SqlDataReader r = cmd.ExecuteReader();
                        while (r.Read())
                        {
                            tags.Add(new Tag { TagId = r.GetInt32(0), TagName = r.GetString(1) });
                        }
                    }
                    return tags;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        //Assumes connection string already initialized
        private static bool Exists(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT UserID FROM Customers WHERE UserID = @id";
                        cmd.Parameters.AddWithValue("@id", userId);
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

        //If already in table, does nothing but still returns 0
        public static int AddTag(Dictionary<string, int> data)
        {
            SetConnectionString();
            if (!Exists(data["userId"])) return -10;
            if (!IsValidTag(data["tagId"])) return -9;
            if (AlreadyInTable(data["userId"], data["tagId"])) return 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO PlayerTags (TagID, PlayerID) VALUES (@tag, @player);";
                        cmd.Parameters.AddWithValue("@tag", data["tagId"]);
                        cmd.Parameters.AddWithValue("@player", data["userId"]);
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
                        cmd.CommandText = "SELECT TagID FROM Tags WHERE TagID = @id AND ForPlayer = 1";
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

        private static bool AlreadyInTable(int user, int tag)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT TagID FROM PlayerTags WHERE TagID = @tag AND PlayerID = @player";
                        cmd.Parameters.AddWithValue("@tag", tag);
                        cmd.Parameters.AddWithValue("@player", user);
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

        //If not in table, does nothing, but returns positive error code
        public static int RemoveTag(Dictionary<string, int> data)
        {
            SetConnectionString();
            if (!AlreadyInTable(data["userId"], data["tagId"])) return 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM PlayerTags WHERE PlayerID = @player AND TagID = @tag";
                        cmd.Parameters.AddWithValue("@player", data["userId"]);
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

        public static int DeleteGroup(Dictionary<string, string> data)
        {
            throw new NotImplementedException();
        }

        public static List<MiniInfo> GetOwnedGroups(int id)
        {
            SetConnectionString();
            try
            {
                List<MiniInfo> groups = new List<MiniInfo>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT GroupID, GroupName FROM Groups WHERE OwnerID = @id";
                        cmd.Parameters.AddWithValue("@id", id);

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var group = new MiniInfo();
                            group.Id = reader.GetInt32(0);
                            group.Name = reader.GetString(1);
                            groups.Add(group);
                        }
                        return groups;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static List<MiniInfo> GetOpenPrivateRequests(int id)
        {
            SetConnectionString();
            try
            {
                List<MiniInfo> requests = new List<MiniInfo>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT UserID_1, UserName FROM Relationships " +
                            "JOIN Customers ON UserID_1 = UserID WHERE Relationship = 'R' AND UserID_2 = @id;";
                        cmd.Parameters.AddWithValue("@id", id);

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var request = new MiniInfo();
                            request.Id = reader.GetInt32(0);
                            request.Name = reader.GetString(1);

                            requests.Add(request);
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

        public static int SendFriendRequest(Dictionary<string, int> data)
        {
            SetConnectionString();
            if (RequestExists(data, "userId", "otherId")) return 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Relationships (UserID_1, UserID_2, Relationship) " +
                            "VALUES (@userId, @otherId, 'R');";
                        cmd.Parameters.AddWithValue("@userId", data["userId"]);
                        cmd.Parameters.AddWithValue("@otherId", data["otherId"]);
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

        public static int ModifyRequest(Dictionary<string, int> data)
        {
            SetConnectionString();
            if (!RequestExists(data, "requestorID", "userID")) return -2;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Relationships WHERE UserID_1 = @requestor AND UserID_2 = @user AND Relationship = 'R'";
                        cmd.Parameters.AddWithValue("@requestor", data["requestorID"]);
                        cmd.Parameters.AddWithValue("@user", data["userID"]);

                        cmd.ExecuteNonQuery();
                    }

                    if (!data.ContainsKey("isAccepted")) return -1;
                    if (data.ContainsKey("isAccepted") && data["isAccepted"] == 0) return 0;

                    int chatId;
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT Max(RelationshipID) FROM Relationships";
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            chatId = reader.GetInt32(0);
                            chatId++;
                        }

                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Relationships (UserID_1, UserID_2, Relationship, PrivateChatID) " +
                            "VALUES (@requestor, @user, 'F', @chatId), (@user, @requestor, 'F', @chatId)";
                        cmd.Parameters.AddWithValue("@requestor", data["requestorID"]);
                        cmd.Parameters.AddWithValue("@user", data["userID"]);
                        cmd.Parameters.AddWithValue("@chatId", chatId);

                        cmd.ExecuteNonQuery();
                    }
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        private static bool RequestExists(Dictionary<string, int> data, string key1, string key2)
        {
            if (!data.ContainsKey(key1) || !data.ContainsKey(key2)) return false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT RelationshipID FROM Relationships WHERE UserID_1 = @requestor " +
                            "AND UserID_2 = @user AND Relationship = 'R'";
                        cmd.Parameters.AddWithValue("@requestor", data[key1]);
                        cmd.Parameters.AddWithValue("@user", data[key2]);

                        var reader = cmd.ExecuteReader();
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
                Debug.WriteLine(ex);
                return false;
            }
        }

        private static int GetChatID(int user1, int user2)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT PrivateChatID FROM Relationships WHERE UserID_1 = @user1 AND UserID_2 = @user2 AND " +
                            "Relationship = 'F'";
                        cmd.Parameters.AddWithValue("@user1", user1);
                        cmd.Parameters.AddWithValue("@user2", user2);

                        var reader = cmd.ExecuteReader();
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
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public static int SendPrivateMessage(Dictionary<string, string> data)
        {
            SetConnectionString();
            int chatId = GetChatID(Convert.ToInt32(data["senderId"]), Convert.ToInt32(data["recieverId"]));
            if (chatId < 0) return -2;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Messages (PrivateChatID, SenderID, Message, TimeStamp) " +
                            "VALUES (@chatId, @sender, @message, @timestamp)";
                        cmd.Parameters.AddWithValue("@chatId", chatId);
                        cmd.Parameters.AddWithValue("@sender", Convert.ToInt32(data["senderId"]));
                        cmd.Parameters.AddWithValue("@message", data["message"]);
                        cmd.Parameters.AddWithValue("@timestamp", Convert.ToDateTime(data["timestamp"]));

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

        //Delete Account
        //MyRequests
        //PrivateRequests to me
        //GroupRequests for groups that I own
        //Remove Friend
        //IsFriend
        
    }
}
