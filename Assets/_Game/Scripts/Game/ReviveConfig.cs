namespace LightItUp.Game
{
    public static class ReviveConfig
    {
        public enum ReviveType
        {
            LastBlock, ClosestBlock
        }
        
        public static class ReviveAbTest
        {
            public const string testKey = "Update3_reviveBugFix_Test";
            public const string last_block = "Update3_DefaultA";
            public const string closest_block = "Update3_CloseLITB";
        }

        public static void SetReviveType(string abTestResult)
        {
            switch (abTestResult)
            {
                case ReviveAbTest.last_block:
                    reviveType =  ReviveType.LastBlock;
                    break;
                case ReviveAbTest.closest_block:
                    reviveType = ReviveType.ClosestBlock;
                    break;
                default:
                    reviveType = ReviveType.LastBlock;
                    break;
            }   
        }

        public static ReviveType reviveType = ReviveType.ClosestBlock;
    }
}