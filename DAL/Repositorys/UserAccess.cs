using Contracts.DTOs;
using Contracts.Enums;
using Contracts.Interfaces;
using DAL.Mappers;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositorys
{
    public class UserAccess : IUserAccess
    {
        private readonly string _connectionString;
        private readonly IUserRole _roleRepo;

        public UserAccess(string connectionString, IUserRole roleRepo)
        {
            _connectionString = connectionString;
            _roleRepo = roleRepo;
        }

        public UserDto? GetUserByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"
                SELECT u.Id, u.Name, u.Email, u.Password_Hash, u.Role_Id, r.Name AS RoleName
                FROM [User] u
                JOIN [User_Role] r ON u.Role_Id = r.Id
                WHERE u.Email = @Email", conn);

            cmd.Parameters.AddWithValue("@Email", email);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new UserDto
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.GetString(3),
                    RoleId = reader.GetInt32(4),
                    RoleName = reader.GetString(5)
                };
            }
            return null;
        }

        public void CreateUser(UserDto user)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(
                "INSERT INTO [User] (Name, Email, Password_Hash, Role_Id) VALUES (@Name, @Email, @PasswordHash, @RoleId)", conn);

            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@RoleId", user.RoleId);

            cmd.ExecuteNonQuery();
        }

        public void UpdateUser(UserDto user)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(
                "UPDATE [User] SET Name = @Name, Email = @Email, Password_Hash = @PasswordHash, Role_Id = @RoleId WHERE Id = @Id", conn);

            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.Parameters.AddWithValue("@Name", user.Name);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RoleId", user.RoleId);

            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand("DELETE FROM [User] WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public List<(int Id, string Name)> GetAllRoles()
        {
            var roles = new List<(int, string)>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT id, name FROM user_role", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                roles.Add((reader.GetInt32(0), reader.GetString(1)));
            }
            return roles;
        }

        public List<UserDto> GetAllUsers()
        {
            var users = new List<UserDto>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            var cmd = new SqlCommand(@"
                SELECT u.Id, u.Name, u.Email, u.Password_Hash, u.Role_Id, r.Name AS RoleName
                FROM [User] u
                JOIN [User_Role] r ON u.Role_Id = r.Id", conn); 

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new UserDto
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    PasswordHash = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    RoleId = reader.GetInt32(4),
                    RoleName = reader.GetString(5)
                });
            }
            return users;
        }
    }
}