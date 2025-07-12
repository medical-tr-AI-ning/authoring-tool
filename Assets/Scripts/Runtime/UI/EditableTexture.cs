using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace Runtime.UI
{
    class EditableTexture : MonoBehaviour
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField] private Texture2D _rgb;
        [SerializeField] private Texture2D _alpha;


        public EditableTexture(Texture2D texture)
        {
            SetTexture(texture);
        }


        public Texture2D GetTexture()
        {
            return _texture;
        }


        public void SetTexture(Texture2D texture)
        {
            _texture = texture;
            UpdateRGB();
            UpdateAlpha();
        }


        public Texture2D GetRGB() 
        { 
            return _rgb; 
        }


        private void UpdateRGB()
        {
            // Convert current texture to TextureFormat RGB24
            Texture2D textureRGB = new Texture2D(_texture.width, _texture.height, TextureFormat.RGB24, false);
            textureRGB.SetPixels32(_texture.GetPixels32());
            textureRGB.Apply();
            _rgb = textureRGB;
        }


        public Texture2D GetAlpha()
        {
            return _alpha;
        }


        private void UpdateAlpha()
        {
            // Convert current texture to TextureFormat Alpha8
            Texture2D textureAlpha = new Texture2D(_texture.width, _texture.height, TextureFormat.Alpha8, false);
            textureAlpha.SetPixels32(_texture.GetPixels32());
            textureAlpha.Apply();
            _alpha = textureAlpha;
        }


        // The MIT License
        // Copyright © 2020 Roger Cabo Ashauer
        // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute,     // sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:     // The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.     // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,     // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,     // WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
        // https://de.wikipedia.org/wiki/MIT-Lizenz

        // This solution is based on Fast image convolutions by Wojciech Jarosz.
        // http://elynxsdk.free.fr/ext-docs/Blur/Fast_box_blur.pdf
        // And Ivan Kutskir
        // http://blog.ivank.net/fastest-gaussian-blur.html
        // And Mike Demyl
        // https://github.com/mdymel  // https://github.com/mdymel/superfastblur
        // And Quantum1000
        // https://discussions.unity.com/t/contribution-texture2d-blur-in-c/506950/25

        // Adapted for Alpha8 textures


        public Texture2D GaussianBlurAlpha(int radial, Texture2D inTexture, bool inplace = false)
        {
            var t = Time.realtimeSinceStartup;

            Texture2D texture2D;
            if (inplace)
            {
                texture2D = inTexture;
            }
            else
            {
                texture2D = new Texture2D(inTexture.width, inTexture.height, TextureFormat.Alpha8, false);
                Graphics.CopyTexture(inTexture, texture2D);
            } 

            NativeArray<Color32> _RawTextureData = texture2D.GetRawTextureData<Color32>();
            int width = texture2D.width;
            int height = texture2D.height;
            int pixelCount = texture2D.width * texture2D.height;

            int[] m_alpha = new int[width * height];

            ParallelOptions _pOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            };

            Parallel.For(0, width * height / 4, _pOptions, i =>
            {
                m_alpha[i * 4 + 0] = _RawTextureData[i].r;
                m_alpha[i * 4 + 1] = _RawTextureData[i].g;
                m_alpha[i * 4 + 2] = _RawTextureData[i].b;
                m_alpha[i * 4 + 3] = _RawTextureData[i].a;
            });

            int[] newAlpha = new int[width * height];

            Parallel.Invoke(
                () => GaussBlur_4(m_alpha, newAlpha, radial, width, height));

            Parallel.For(0, width * height / 4, _pOptions, i =>
            {
                for (int j = 0; j < 4; j++)
                {
                    if (newAlpha[i * 4 + j] > 255) newAlpha[i * 4 + j] = 255;
                    if (newAlpha[i * 4 + j] < 0) newAlpha[i * 4 + j] = 0;
                }

                _RawTextureData[i] = new Color32(
                    (byte)newAlpha[i * 4 + 0], 
                    (byte)newAlpha[i * 4 + 1], 
                    (byte)newAlpha[i * 4 + 2], 
                    (byte)newAlpha[i * 4 + 3]);
            });

            texture2D.Apply();
            var TimeUsedMilliseconds = (Time.realtimeSinceStartup - t) * 1000;
            Debug.Log($"Generating GaussianBlurAlpha completed in {TimeUsedMilliseconds} ms");
            return texture2D;
        }

        private int[] BoxesForGauss(int sigma, int n)
        {
            double wIdeal = Math.Sqrt((12 * sigma * sigma / n) + 1);
            int wl = (int)Math.Floor(wIdeal);
            if (wl % 2 == 0) wl--;
            int wu = wl + 2;

            double mIdeal = (double)(12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
            double m = Math.Round(mIdeal);

            var sizes = new List<int>();
            for (var i = 0; i < n; i++) sizes.Add(i < m ? wl : wu);
            return sizes.ToArray();
        }

        private void GaussBlur_4(int[] colorChannel, int[] destChannel, int r, int w, int h)
        {
            int[] bxs = BoxesForGauss(r, 3);
            BoxBlur_4(colorChannel, destChannel, w, h, (bxs[0] - 1) / 2);
            BoxBlur_4(destChannel, colorChannel, w, h, (bxs[1] - 1) / 2);
            BoxBlur_4(colorChannel, destChannel, w, h, (bxs[2] - 1) / 2);
        }

        private void BoxBlur_4(int[] colorChannel, int[] destChannel, int w, int h, int r)
        {
            for (var i = 0; i < colorChannel.Length; i++) destChannel[i] = colorChannel[i];
            BoxBlurH_4(destChannel, colorChannel, w, h, r);
            BoxBlurT_4(colorChannel, destChannel, w, h, r);
        }

        private void BoxBlurH_4(int[] colorChannel, int[] dest, int w, int h, int radial)
        {
            ParallelOptions _pOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            };

            var iar = (double)1 / (radial + radial + 1);
            Parallel.For(0, h, _pOptions, i =>
            {
                var ti = i * w;
                var li = ti;
                var ri = ti + radial;
                var fv = colorChannel[ti];
                var lv = colorChannel[ti + w - 1];
                var val = (radial + 1) * fv;
                for (var j = 0; j < radial; j++) val += colorChannel[ti + j];
                for (var j = 0; j <= radial; j++)
                {
                    val += colorChannel[ri++] - fv;
                    dest[ti++] = (int)Math.Round(val * iar);
                }
                for (var j = radial + 1; j < w - radial; j++)
                {
                    val += colorChannel[ri++] - dest[li++];
                    dest[ti++] = (int)Math.Round(val * iar);
                }
                for (var j = w - radial; j < w; j++)
                {
                    val += lv - colorChannel[li++];
                    dest[ti++] = (int)Math.Round(val * iar);
                }
            });
        }

        private void BoxBlurT_4(int[] colorChannel, int[] dest, int w, int h, int r)
        {
            ParallelOptions _pOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            };

            var iar = (double)1 / (r + r + 1);
            Parallel.For(0, w, _pOptions, i =>
            {
                var ti = i;
                var li = ti;
                var ri = ti + r * w;
                var fv = colorChannel[ti];
                var lv = colorChannel[ti + w * (h - 1)];
                var val = (r + 1) * fv;
                for (var j = 0; j < r; j++) val += colorChannel[ti + j * w];
                for (var j = 0; j <= r; j++)
                {
                    val += colorChannel[ri] - fv;
                    dest[ti] = (int)Math.Round(val * iar);
                    ri += w;
                    ti += w;
                }
                for (var j = r + 1; j < h - r; j++)
                {
                    val += colorChannel[ri] - colorChannel[li];
                    dest[ti] = (int)Math.Round(val * iar);
                    li += w;
                    ri += w;
                    ti += w;
                }
                for (var j = h - r; j < h; j++)
                {
                    val += lv - colorChannel[li];
                    dest[ti] = (int)Math.Round(val * iar);
                    li += w;
                    ti += w;
                }
            });
        }


        // Adapted from GaussianBlurAlpha

        public Texture2D SpreadAlpha(int radius)
        {
            if (radius < 0)
            {
                return ErosionAlpha(radius);
            }

            if (radius > 0)
            {
                return DilationAlpha(radius);
            }

            return _alpha;
        }


        public Texture2D ErosionAlpha(int radius)
        {
            return ParallelFilterAlpha(Mathf.Min, Math.Abs(radius));
        }


        public Texture2D DilationAlpha(int radius)
        {
            return ParallelFilterAlpha(Mathf.Max, Math.Abs(radius));
        }


        public Texture2D ParallelFilterAlpha(Func<int[], int> filter, int radius)
        {
            return ParallelFilterAlpha(filter, radius, _alpha, inplace: false);
        }


        public Texture2D ParallelFilterAlpha(Func<int[], int> filter, int radius, Texture2D inTexture, bool inplace = false)
        {
            var t = Time.realtimeSinceStartup;

            Texture2D texture2D;
            if (inplace)
            { 
                texture2D = inTexture;
            } 
            else 
            {
                texture2D = new Texture2D(inTexture.width, inTexture.height, TextureFormat.Alpha8, false);
                Graphics.CopyTexture(inTexture, texture2D);
            }

            NativeArray<Color32> _RawTextureData = texture2D.GetRawTextureData<Color32>();
            int width = texture2D.width;
            int height = texture2D.height;
            int pixelCount = texture2D.width * texture2D.height;

            int[] m_alpha = new int[width * height];

            ParallelOptions _pOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            };

            Parallel.For(0, width * height / 4, _pOptions, i =>
            {
                m_alpha[i * 4 + 0] = _RawTextureData[i].r;
                m_alpha[i * 4 + 1] = _RawTextureData[i].g;
                m_alpha[i * 4 + 2] = _RawTextureData[i].b;
                m_alpha[i * 4 + 3] = _RawTextureData[i].a;
            });

            int[] newAlpha = new int[width * height];

            Parallel.Invoke(
                () => BoxFilter(m_alpha, newAlpha, filter, width, height, radius));

            Parallel.For(0, width * height / 4, _pOptions, i =>
            {
                for (int j = 0; j < 4; j++)
                {
                    if (newAlpha[i * 4 + j] > 255) newAlpha[i * 4 + j] = 255;
                    if (newAlpha[i * 4 + j] < 0) newAlpha[i * 4 + j] = 0;
                }

                _RawTextureData[i] = new Color32(
                    (byte)newAlpha[i * 4 + 0],
                    (byte)newAlpha[i * 4 + 1],
                    (byte)newAlpha[i * 4 + 2],
                    (byte)newAlpha[i * 4 + 3]);
            });

            texture2D.Apply();
            var TimeUsedMilliseconds = (Time.realtimeSinceStartup - t) * 1000;
            Debug.Log("ParallelFilterAlpha Time: " + TimeUsedMilliseconds + " ms");
            return texture2D;
        }

        private void BoxFilter(int[] colorChannel, int[] destChannel, Func<int[], int> filter, int w, int h, int radius)
        {
            for (var i = 0; i < colorChannel.Length; i++) destChannel[i] = colorChannel[i];
            FilterHorizontal(destChannel, colorChannel, filter, w, h, radius);
            FilterVertical(colorChannel, destChannel, filter, w, h, radius);
        }

        private void FilterHorizontal(int[] colorChannel, int[] dest, Func<int[], int> filter, int w, int h, int radius)
        {
            /* Run the filter horizontally (Important: The elements in the window are not ordered!) */

            ParallelOptions _pOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            };

            int size = radius + radius + 1;
            Parallel.For(0, h, _pOptions, i =>
            {
                var ti = i * w;
                var ri = ti + radius;
                int[] window = new int[size];
                int windowIdx = size - 1;
                // Initialize window
                for (var j = 0; j < radius; j++) window[j] = colorChannel[ti];
                for (var j = 0; j < radius; j++) window[radius + j] = colorChannel[ti + j];
                // Slide window and apply filter
                for (var j = 0; j < w - radius; j++)
                {
                    window[windowIdx] = colorChannel[ri++];
                    dest[ti++] = filter(window);
                    windowIdx = (windowIdx + 1) % size;
                }
                for (var j = w - radius; j < w; j++)
                {
                    window[windowIdx] = colorChannel[ri-1];
                    dest[ti++] = filter(window);
                    windowIdx = (windowIdx + 1) % size;
                }
            });
        }

        private void FilterVertical(int[] colorChannel, int[] dest, Func<int[], int> filter, int w, int h, int radius)
        {
            /* Run the filter vertically (Important: The elements in the window are not ordered!) */
            
            ParallelOptions _pOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 8
            };

            int size = radius + radius + 1;
            Parallel.For(0, w, _pOptions, i =>
            {
                var ti = i;
                var ri = ti + radius * w;
                int[] window = new int[size];
                int windowIdx = size - 1;
                // Initialize window
                for (var j = 0; j < radius; j++) window[j] = colorChannel[ti];
                for (var j = 0; j < radius; j++) window[radius + j] = colorChannel[ti + j * w];
                // Slide window and apply filter
                for (var j = 0; j < h - radius; j++)
                {
                    window[windowIdx] = colorChannel[ri];
                    dest[ti] = filter(window);
                    windowIdx = (windowIdx + 1) % size;
                    ri += w;
                    ti += w;
                }
                for (var j = h - radius; j < h; j++)
                {
                    window[windowIdx] = colorChannel[ri - w];
                    dest[ti] = filter(window);
                    windowIdx = (windowIdx + 1) % size;
                    ti += w;
                }
            });
        }

    }
}
