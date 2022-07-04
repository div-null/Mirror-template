namespace CodeBase.Infrastructure
{
    public class AllServices
    {
        public static MainInputActions MainInputActions;
        public static PlayerProgressData PlayerProgressData;
        
        public AllServices(MainInputActions mainInputActions, PlayerProgressData playerProgressData)
        {
            MainInputActions = mainInputActions;
            PlayerProgressData = playerProgressData;
        }
    }
}