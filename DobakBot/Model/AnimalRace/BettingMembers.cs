using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Model
{
    class BettingMembers : List<BettingMember>
    {

        public void SetOdd(float odd) => ForEach(item => item.Money = (int)(item.Money * odd));

        public override string ToString()
        {
            string text = string.Empty;
            foreach (var item in this)
            {
                text += $"{item.Nickname} : {item.Money}:coin:\n";
            }
            return text;
        }

        public List<DBModelUser> ToDBModelUsers()
        {
            var list = new List<DBModelUser>();
            foreach (var item in this)
            {
                list.Add(new DBModelUser(item.ID, item.Money));
            }
            return list;
        }
    }
}
