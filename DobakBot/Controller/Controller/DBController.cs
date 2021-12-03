using Dapper;
using DobakBot.Model;
using MySql.Data.MySqlClient;

namespace DobakBot.Controller
{
    class DBController
    {
        const string conString = "server=127.0.0.1;Port=3306;uid=root;pwd=kiki*3279;database=dalsubot";

        private MySqlConnection Connection => new MySqlConnection(conString);

        public DBModelUser GetUserByDiscordId(ulong discordId)
        {
            using(var conn = Connection)
            {
                var query = $"SELECT * FROM user WHERE id={discordId}";
                return conn.QueryFirst<DBModelUser>(query);
            }
        }

        public bool InsertUser(ulong discordID)
        {
            using (var conn = Connection)
            {
                var query = $"INSERT INTO user(id) VALUES({discordID})";
                return conn.Execute(query) == 1;
            }
        }


        public bool TryAddUserCoin(ulong discordID, int coin)
        {
            using (var conn = Connection)
            {
                var query = $"UPDATE user SET coin=coin+{coin} WHERE id={discordID};";
                return conn.Execute(query) == 1;
            }
        }

        public bool TryAddUsersCoin(BettingMembers members)
        {
            using (var conn = Connection)
            {
                foreach (var item in members)
                {
                    var query = $"UPDATE user SET coin=coin+{item.Money} WHERE id={item.ID};";
                    if(conn.Execute(query) != 1) return false;
                }
            }
            return true;
        }

        public bool TrySubtractUserCoin(ulong discordID, int coin)
        {
            using (var conn = Connection)
            {
                var query = $"UPDATE user SET coin=coin-{coin} WHERE id={discordID};";
                return conn.Execute(query) == 1;
            }
        }
    }
}
