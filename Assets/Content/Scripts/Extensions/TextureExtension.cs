using UnityEngine;

namespace Content.Scripts.Extensions
{
    public static class TextureExtension
    {
        public static void Release(this Texture texture)
        {
            if (texture != null)
            {
                Object.Destroy(texture);
            }
        }
    }
}