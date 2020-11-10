using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.ServiceModel;

namespace WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceChat" in both code and config file together.
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> users = new List<ServerUser>();

        static string connectionString = @"Data Source=DESKTOP-M0UINHO\SQLSERVER;Initial Catalog=chat;Integrated Security=True";
        SqlConnection sqlConnection = new SqlConnection(connectionString);

        public int Connect(string name)
        {
            int id;
            sqlConnection.Open();
            SqlCommand cmd0 = new SqlCommand("SELECT id FROM[Users] WHERE username = @name", sqlConnection);
            cmd0.Parameters.AddWithValue("name", name);

            if (cmd0.ExecuteScalar() == null)
            {

                SqlCommand cmd1 = new SqlCommand("INSERT INTO[Users] (username) VALUES (@name)", sqlConnection);
                cmd1.Parameters.AddWithValue("name", name);
                cmd1.ExecuteNonQuery();

                SqlCommand cmd2 = new SqlCommand("SELECT id from[Users] where username = @name", sqlConnection);
                cmd2.Parameters.AddWithValue("name", name);
                cmd2.ExecuteNonQuery();

                id = Convert.ToInt32(cmd2.ExecuteScalar());
            }  
            else
                id = Convert.ToInt32(cmd0.ExecuteScalar());

            ServerUser user = new ServerUser()
            {
                ID = id,
                Name = name,
                operationContext = OperationContext.Current
            };

            SendMsg(": " + user.Name + " подключился к чату!", 0);
            users.Add(user);
            sqlConnection.Close();
            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if(user!=null)
            {
                users.Remove(user);
                SendMsg(": " + user.Name + " покинул чат!", 0);
            }
        }

        public void SendMsg(string msg, int id)
        {
            foreach(var item in users)
            {
                string answer = DateTime.Now.ToString("HH:mm:ss");
                var user = users.FirstOrDefault(i => i.ID == id);
                if (user != null)
                    answer += ": " + user.Name + " ";
                answer += msg;

                item.operationContext.GetCallbackChannel<IServiceChatCallBack>().MsgCallBack(answer);
            }
        }

        public AllData[] GetData()
        {
            sqlConnection.Open();
            SqlCommand command = new SqlCommand("SELECT username, msg, time FROM[MsgV]", sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            List<AllData> data = new List<AllData>();
            while (reader.Read())
            {
                AllData info = new AllData
                {
                    Username = reader["username"].ToString(),
                    Message = reader["msg"].ToString(),
                    Time = reader["time"].ToString(),
                };
                data.Add(info);
            }
            reader.Close();
            sqlConnection.Close();
            return data.ToArray();
        }

        public void NewMsg(int id_u, string msg, string time)
        {
            sqlConnection.Open();
            SqlCommand command = new SqlCommand("INSERT INTO[Messages] (id_u, msg, time) VALUES (@id_u, @msg, @time)", sqlConnection);
            command.Parameters.AddWithValue("id_u", id_u);
            command.Parameters.AddWithValue("msg", msg);
            command.Parameters.AddWithValue("time", Convert.ToDateTime(time));
            command.ExecuteNonQuery();
            sqlConnection.Close();
        }
    }
}
