using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DobakBot.Controller
{
    class GambleController
    {
        #region 싱글톤
        private GambleController() { }
        private static readonly Lazy<GambleController> _instance = new Lazy<GambleController>(() => new GambleController());
        public static GambleController Instance { get { return _instance.Value; } }
        #endregion

        public AnimalRaceController animalRace = new AnimalRaceController();
        public DBController DB = new DBController();

    }
}
