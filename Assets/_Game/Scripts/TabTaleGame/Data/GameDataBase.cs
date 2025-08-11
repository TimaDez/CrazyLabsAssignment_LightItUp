using LightItUp.Data;

namespace LightItUp.Data
{
    public class GameDataBase {

        #region PlayerData

        static PlayerData _playerData;
        public static PlayerData PlayerData
        {
            get
            {
                if (_playerData == null)
                {
                    _playerData = new PlayerData().Load();
                }
                return _playerData;
            }
        }
        #endregion

    }
}
