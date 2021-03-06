using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Configuration;
using System.IO;

namespace ShopProject
{
    public class DatabaseAccountAuthentication
    {
        static string ConnectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        public static string getConnectionString()
        {
            return ConnectionString;
        }

        public static bool Register(string Username, string Password)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand select = new SqlCommand("Select * from Customer WHERE Username = @user;");
            
            select.Connection = conn;
            select.Parameters.AddWithValue("@user", Username);
            adapter.SelectCommand = select;
            conn.Open();
            SqlCommand update = new SqlCommand("Insert into Customer (Username, Password) VALUES (@user, @pass);");
            update.Parameters.AddWithValue("@user", Username);
            update.Parameters.AddWithValue("@pass", Password);
            update.Connection = conn;
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if(dt.Rows.Count > 0)
            {
                conn.Close();
                return false; //If registration fails here, it is because the username is already in use.
            }
            update.ExecuteNonQuery();
            conn.Close();
            return true; 
        }
        public static bool Login(string Username, string Password) //Can change return type if necessary
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand select = new SqlCommand("Select * from Customer WHERE Username = @user AND Password = @pass;");

            select.Connection = conn;
            select.Parameters.AddWithValue("@user", Username);
            select.Parameters.AddWithValue("@pass", Password);
            adapter.SelectCommand = select;
            conn.Open();

            DataTable dt = new DataTable();
            adapter.Fill(dt);
            conn.Close();
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        public static void submitOrder(decimal orderTotal)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();

            SqlCommand update = new SqlCommand("Insert into Orders (OrderAmount) VALUES (@total);");
            update.Parameters.AddWithValue("@total", orderTotal);
            update.Connection = conn;
            update.ExecuteNonQuery();
            conn.Close();
        }
    }
    public class DatabasePullItems 
    {
        static string ConnectionString = DatabaseAccountAuthentication.getConnectionString();
        public static DataTable getSelectItems(List<string> items) //pass list of strings that match the names in the db
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            var command = new StringBuilder("Select * from Items WHERE name = @name");
            SqlCommand select = new SqlCommand();
            select.Parameters.AddWithValue("@name", items[0]);
            
            for(int i = 1; i<items.Count; i++)
            {
                string newParam = "@name"+i.ToString();
                command.Append(" OR name = "+newParam);
                select.Parameters.AddWithValue(newParam, items[i]);
            }
            command.Append(";");
            select.CommandText = command.ToString();
            select.Connection = conn;
            adapter.SelectCommand = select;
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            conn.Close();

            addImagePath(ref dt);

            return dt;
        }
        public static DataTable getItemById(int id) //pass list of strings that match the names in the db
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            var command = new StringBuilder("Select * from Items WHERE id = @id;");
            SqlCommand select = new SqlCommand();
            select.Parameters.AddWithValue("@id", id);
            select.CommandText = command.ToString();
            select.Connection = conn;
            adapter.SelectCommand = select;
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            conn.Close();

            addImagePath(ref dt);

            return dt;
        }
        public static DataTable getAllItems()
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection conn = new SqlConnection(ConnectionString);
            conn.Open();
            SqlCommand select = new SqlCommand("Select * from Items;");
            select.Connection = conn;
            adapter.SelectCommand = select;
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            conn.Close();

            addImagePath(ref dt);

            return dt;
        }
        public static decimal calculatePrice(DataTable table)
        {
            decimal totalPrice = 0m;
            foreach(DataRow row in table.Rows)
            {
                totalPrice += (decimal)row.ItemArray[2];
            }

            return totalPrice;
        }

        private static void addImagePath(ref DataTable dt)
        {
            dt.Columns.Add("ImagePath");
            foreach (DataRow dr in dt.Rows)
                dr["ImagePath"] = "ART\\" + dr["name"] + ".jpg";
        }
    } 
}