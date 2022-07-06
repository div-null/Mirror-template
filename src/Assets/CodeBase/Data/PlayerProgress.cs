using Random = UnityEngine.Random;

namespace Game.CodeBase.Data
{
    public class PlayerProgress
    {
        public string Username;
        //public int Money;
        public ColorData ColorData;
        //Текущий lvl и прогресс по нему
        //public LevelProgressData LevelProgressData;
        //Информация о скинах (какие открыты, а какие нет) + текущий скин персонажа
       // public ItemsData SkinData;


        public PlayerProgress()
        {
            Username = "Player" + Random.Range(1000, 10000);
            ColorData = new ColorData();
        }
    }
}