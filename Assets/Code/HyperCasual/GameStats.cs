namespace LightItUp
{
    public static class GameStats
    {
        public const string play_level = "play_level";
        public const string complete_level = "complete_level";
        public const string complete_levels_max_stars = "complete_levels_max_stars";
        public const string light_exploding_blocks = "light_exploding_blocks";
        public const string light_yellow_blocks = "light_yellow_blocks";
        public const string light_red_blocks = "light_red_blocks";
        public const string unlock_skin = "unlock_skin";
        public const string die_on_spikes = "die_on_spikes";
        public const string light_blocks = "light_blocks";
        
        // public const string unlock_skins = "unlock_skins";
        
        public static readonly string[] Types = {
            play_level, 
            complete_level, 
            complete_levels_max_stars, 
            light_exploding_blocks, 
            light_yellow_blocks, 
            light_red_blocks,
            die_on_spikes,
            light_blocks
        };


        public static string[] GetTypes()
        {
            return Types;
        }        
    }

    public static class GameStatsParams
    {
        public const int yellowColorIndex = 0;
        public const int redColorIndex = 2;
    }
}