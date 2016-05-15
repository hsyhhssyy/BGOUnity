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

        private static readonly Dictionary<String, Func<Sprite>> lazyLoadSprites=new Dictionary<string, Func<Sprite>>(); 

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

            var cardBackgrounds = new String[]
            {
                "action", "urban", "production", "military", "special", "leader", "government", "wonder", "tactic", "war",
                "event", "aggression", "pact","defend"
            };
            foreach (String t in cardBackgrounds)
            {
                String s = t;
                LazyLoadSprite("pc-board-card-small-"+s+"-background",
                () => ZoomSprite("SpriteTile/UI/"+s, new Vector2(0.5f, 0.5f), 70f / 297f));
                LazyLoadSprite("pc-board-card-normal-" + s + "-background",
                () => ZoomSprite("SpriteTile/UI/" + s, new Vector2(0.5f, 0.5f), 210f / 297f));
            }
        }
        

        private static Sprite ZoomSprite(String spName,Vector2 pviot,float scale)
        {
            var smallSp = UnityResources.GetSprite(spName);
            return smallSp != null ? smallSp.CloneResize(pviot, scale) : null;
        }

        public static void LazyLoadSprite(String key, Func<Sprite> func)
        {
            lazyLoadSprites[key] = func;
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
            if (_Sprites.ContainsKey(key))
            {
                return _Sprites[key];
            }

            if (lazyLoadSprites.ContainsKey(key))
            {
                var spriteFunc = lazyLoadSprites[key];

                lazyLoadSprites.Remove(key);
                var sprite = spriteFunc();

                if (sprite != null)
                {
                    _Sprites[key] = sprite;
                }

                return sprite;
            }

             return Resources.Load<Sprite>(key);
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
