using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.CSharpCode.Helper
{
    public static class UnityResources
    {
        private static bool _loaded=false;

        private readonly static Dictionary<String, Sprite> _Sprites = new Dictionary<string, Sprite>(); 

        private static void LoadResource()
        {
            if (_loaded)
            {
                return;
            }

            _loaded = true;

            LoadSprite("SpriteTile/CardRow_Sprite_CardBackground");
            LoadSprite("SpriteTile/SpriteSheet1");
            LoadSprite("SpriteTile/PCBoard/pc-board-sprite-sheet");
        }

        public static Sprite GetSprite(String key)
        {
            if (key == null)
            {
                return null;
            }
            if (!_loaded)
            {
                LoadResource();
            }
            return _Sprites.ContainsKey(key) ? _Sprites[key] : Resources.Load<Sprite>(key);
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
