using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text.Json;

namespace WebApi
{
    /**
     * Profile - Handles retrieving any data refering to a profile
     */
    public class Profile
    {
        //connection string
        private const string connectionString = 
            "Server = satou.cset.oit.edu, 5433; " +
            "Database = tavern; " +
            "User Id = tavern; " +
            "Password = T@vern5!; " +
            "TrustServerCertificate=true";

        /**
         * Profile - Basic Profile Constructor that initializes any values passed to it
         * @param id - Profile ID
         * @param name - Profile Username
         * @param bio - Profile Bio
         */
        public Profile (int id=-1, string name="", string bio = "") 
        {
            Id= id;
            Name= name;
            Bio= bio;
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
        public static Profile GetProfile(int id)
        {
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
                            profile = null; 
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
        public static string GetFriends(int id)
        {
            try
            {
                List<string> friends = new List<string>(); //List that stores the friend username(s)
                List<int> ids = new List<int>(); //List that stores the friend's ID's
                using (SqlConnection  conn = new SqlConnection(connectionString))
                {
                    conn.Open(); //Opens connection to database
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT UserID_1, UserID_2 FROM tavern.dbo.PrivateChat WHERE Relationship='F' AND " +
                            "(UserID_1=@ProfileID OR UserID_2=@ProfileID)"; //Query
                        cmd.Parameters.AddWithValue("@ProfileID", id);
                        SqlDataReader reader = cmd.ExecuteReader(); //Passes friends ids to reader
                        while (reader.Read()) //Grabs all friend ids and adds them to ids list
                        {
                            if (reader.GetInt32(0) == id) //UserId_1 is the Current Profile
                                ids.Add(reader.GetInt32(1));
                            else //UserId_2 is the current profile
                                ids.Add(reader.GetInt32(0));
                        }
                    }
                    conn.Close(); //closes first connection
                    if (ids.Count < 0) //Checks for any friends
                    {
                        conn.Open(); //Opens new connection, conn doesnt like creating multiple commands. Double check for later!!!
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT UserID, UserName FROM tavern.dbo.Customers";  //Gets all Profiles. Double check if theres a better way!!!
                            SqlDataReader reader = cmd.ExecuteReader(); //Passes to reader
                            while (reader.Read())
                            {
                                if (ids.Contains(reader.GetInt32(0)))//checks if profile is in list, there's got to be a better way!!!
                                    friends.Add(reader.GetString(1));
                            }
                        }
                        conn.Close(); //close connection
                    }
                    
                }
                return JsonSerializer.Serialize(friends); //returns the json of the friends names

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
    }
}
