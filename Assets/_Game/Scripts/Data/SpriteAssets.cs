using System.Collections.Generic;
using UnityEngine;
using LightItUp.Game;
using LightItUp.Singletons;

namespace LightItUp.Data
{
    public class SpriteAssets : SingletonAsset<SpriteAssets>
    {
        [System.Serializable]
        public class ColorScheme
        {
            public string name;
            public List<Color> colors;
            public List<Material> colorFillMats;
            public List<Material> colorOutlineMats;
        }
        [System.Serializable]
        public class TutorialImageData
        {
            public BlockController.TutorialImages imgType;
            public Sprite sprite;
        }
        public List<BlockSprites> blockSprites;
        public Sprite polyDot;
        public List<ColorScheme> colorSchemes;
        public Texture2D outlineUnlit;
        public Texture2D outlineLit;
        public List<TutorialImageData> tutorialImages;
        public Sprite effectorAttractArrow;
        public Sprite effectorDeflectArrow;
        public Sprite starFilled;
		public Sprite starEmpty;
		public Sprite auraOrb;
        public Sprite aura;
		public Sprite boosterIconShoes;
		public Sprite boosterIconBoots;
		public Sprite boosterIconAura;

		public Sprite boosterIconSmallShoes;
		public Sprite boosterIconSmallBoots;
		public Sprite boosterIconSmallAura;
		public Sprite boosterIconBigShoes;
		public Sprite boosterIconBigBoots;
		public Sprite boosterIconBigAura;
		public Sprite boosterSelectedFrame;
		public Sprite boosterUnselectedFrame;
		public Sprite boosterButtonShoesDefault;
		public Sprite boosterButtonBootsDefault;
		public Sprite boosterButtonAuraDefault;
		public Sprite boosterSelectedCheckmark;
		public Sprite boosterAddMore;
		public Sprite boosterLock;



        public Sprite GetTutorialImage(BlockController.TutorialImages imgType) {
            var a = tutorialImages.Find(x => x.imgType == imgType);
            if (a != null)
                return a.sprite;
            return null;
        }

        public Color GetColorFromScheme(int scheme, int color) {
            scheme %= colorSchemes.Count;
            color %= colorSchemes[scheme].colors.Count;
            return colorSchemes[scheme].colors[color];
        }
        public Material GetColorOutlineMatFromScheme(int scheme, int color)
        {
            scheme %= colorSchemes.Count;
            color %= colorSchemes[scheme].colors.Count;
            return colorSchemes[scheme].colorOutlineMats[color];
        }
        public Material GetColorFillMatFromScheme(int scheme, int color)
        {
            scheme %= colorSchemes.Count;
            color %= colorSchemes[scheme].colors.Count;
            return colorSchemes[scheme].colorFillMats[color];
        }
        [System.Serializable]
        public class BlockSprites {
            public BlockController.ShapeType shape;
            public Sprite lit;
            public Sprite unlit;
        }
        public BlockSprites GetBlockSprite(BlockController.ShapeType st)
        {
            foreach (var b in blockSprites)
            {
                if (b.shape == st)
                {
                    return b;
                }
            }
            Debug.LogWarning("No BlockSprite for shape: "+st.ToString());
            if (blockSprites.Count > 0)
            {
                return blockSprites[0];
            }
            return null;        
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/SpriteAssets")]
        public static void CreateSpriteAssets()
        {
            CreateAsset();
        }

        public override void Init()
        {
        }
#endif
    }
}
