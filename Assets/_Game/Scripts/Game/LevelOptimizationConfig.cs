namespace LightItUp.Game
{
    public static class LevelOptimizationConfig
    {
		public static string LevelsFolderDefault = "GameLevels/";    
		public static string LevelsFolderOptimized = "GameLevels_1_4_5/";
		public static string LevelsFolder = LevelsFolderDefault;

        public static class AbTest
        {
            public const string testKey = "Update4LevelsOptimization";
			public const string inactive = "Update4DefaultLevels_A";
			public const string active = "Update4OptimizedLevels_B";
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