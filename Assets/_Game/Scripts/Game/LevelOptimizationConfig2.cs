namespace LightItUp.Game
{
    public static class LevelOptimizationConfig2
    {
		public static string LevelsFolderDefault = "GameLevels/";    
		public static string LevelsFolderOptimized = "GameLevels_1_5_3/";
		public static string LevelsFolder = LevelsFolderDefault;

        public static class AbTest
        {
			public const string testKey = "Update7_LevelFunnelOptimization";
			public const string inactive = "Update7_defaultLevelFunnelA";
			public const string active = "Update7_NewLevelFunnelB";
        }
        
        public static void HandleResponse(string abTestResult)
        {
            switch (abTestResult)
            {
                case AbTest.active:
					LevelsFolder = LevelsFolderOptimized;
                    break;
                default:
					LevelsFolder = LevelsFolderDefault;
                    break;                    
            }   
        }        
    }
}