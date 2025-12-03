using Contracts.Interfaces;
using DAL.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Repositorys
{
    public class UserRole : IUserRole
    {
        private readonly string _connectionString;

        public UserRole(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<UserRole> GetAllRoles()
        {
            var roles = new List<UserRole>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT id, name FROM user_role", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                roles.Add(new UserRole
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
            return roles;
        }

        public UserRole? GetRoleById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT id, name FROM user_role WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new UserRole
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
            }
            return null;
        }
    }
}
