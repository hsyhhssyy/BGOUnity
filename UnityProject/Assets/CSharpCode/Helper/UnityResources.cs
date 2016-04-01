using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CSharpCode.Helper
{
    public class UnityResources
    {
        private static bool loaded=false;

        private readonly static Dictionary<String, Sprite> _Sprites = new Dictionary<string, Sprite>(); 

        private static void LoadResource()
        {
            if (loaded)
            {
                return;
            }

            loaded = true;

            LoadSprite("SpriteTile/CardRow_Sprite_CardBackground");
            LoadSprite("SpriteTile/SpriteSheet1");
        }

        public static Sprite GetSprite(String key)
        {
            if (!loaded)
            {
                LoadResource();
            }
            return _Sprites.ContainsKey(key) ? _Sprites[key] : null;
        }

        private static void LoadSprite(String resourcePath)
        {
            var uSprites = Resources.LoadAll<Sprite>(resourcePath);

            foreach (Sprite sp in uSprites)
            {
                _Sprites[sp.name]=sp;
            }
        }
    }
}
