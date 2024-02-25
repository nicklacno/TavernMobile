﻿using Microsoft.Data.SqlClient;

namespace WebApi
{
    public class Group
    {
        private static string? connectionString = null;

        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public int OwnerId { get; set; }
        public List<string> members { get; set; }

        /**
         * GetGroup - Returns the group given the specific id
         * @param id - Id for the group
         * @return - The data for the group
         */
        static Group? GetGroup(int id)
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
                        cmd.CommandText = "SELECT ";
                    }
                }

                return group;
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
    }
}