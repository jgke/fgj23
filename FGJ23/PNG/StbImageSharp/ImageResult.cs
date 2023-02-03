using System;
using System.Runtime.InteropServices;

namespace StbImageSharp
{
#if !STBSHARP_INTERNAL
    public
#else
	internal
#endif
    class ImageResult
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public ColorComponents SourceComp { get; set; }
        public ColorComponents Comp { get; set; }
        public byte[] Data { get; set; }

        internal unsafe static ImageResult FromResult(byte* result, int width, int height, ColorComponents comp, ColorComponents req_comp)
        {
            if (result == null)
            {
                throw new InvalidOperationException(StbImage.LastError);
            }

            var image = new ImageResult
            {
                Width = width,
                Height = height,
                SourceComp = comp,
                Comp = req_comp == ColorComponents.Default ? comp : req_comp
            };

            // Convert to array
            image.Data = new byte[width * height * (int)image.Comp];
            Marshal.Copy(new IntPtr(result), image.Data, 0, image.Data.Length);

            return image;
        }

        public unsafe static ImageResult FromMemory(byte[] bytes, ColorComponents requiredComponents = ColorComponents.Default)
        {
            byte* result = null;

            try
            {
                int x, y, comp;
                fixed (byte* b = bytes)
                {
                    result = StbImage.stbi_load_from_memory(b, bytes.Length, &x, &y, &comp, (int)requiredComponents);
                }

                return FromResult(result, x, y, (ColorComponents)comp, requiredComponents);
            }
            finally
            {
                if (result != null)
                {
                    CRuntime.free(result);
                }
            }
        }

        public unsafe static ImageResult FromIndexedWithFirstColorBeingTransparent(byte[] bytes)
        {

            static int stbi__parse_png_file(StbImage.stbi__png z, int scan, int req_comp)
            {
                byte* palette = stackalloc byte[1024];
                byte pal_img_n = (byte)(0);
                byte has_trans = (byte)(0);
                byte* tc = stackalloc byte[3];
                tc[0] = (byte)(0);

                ushort* tc16 = stackalloc ushort[3];
                uint ioff = (uint)(0);
                uint idata_limit = (uint)(0);
                uint i = 0;
                uint pal_len = (uint)(0);
                int first = (int)(1);
                int k = 0;
                int interlace = (int)(0);
                int color = (int)(0);
                int is_iphone = (int)(0);
                StbImage.stbi__context s = z.s;
                z.expanded = (null);
                z.idata = (null);
                z._out_ = (null);
                if (StbImage.stbi__check_png_header(s) == 0)
                    return (int)(0);
                if ((scan) == (StbImage.STBI__SCAN_type))
                    return (int)(1);
                for (; ; )
                {
                    StbImage.stbi__pngchunk c = (StbImage.stbi__pngchunk)(StbImage.stbi__get_chunk_header(s));
                    switch (c.type)
                    {
                        case (((uint)('C') << 24) + ((uint)('g') << 16) + ((uint)('B') << 8) + (uint)('I')):
                            is_iphone = (int)(1);
                            StbImage.stbi__skip(s, (int)(c.length));
                            break;
                        case (((uint)('I') << 24) + ((uint)('H') << 16) + ((uint)('D') << 8) + (uint)('R')):
                            {
                                int comp = 0;
                                int filter = 0;
                                if (first == 0)
                                    return (int)(StbImage.stbi__err("multiple IHDR"));
                                first = (int)(0);
                                if (c.length != 13)
                                    return (int)(StbImage.stbi__err("bad IHDR len"));
                                s.img_x = (uint)(StbImage.stbi__get32be(s));
                                if ((s.img_x) > (1 << 24))
                                    return (int)(StbImage.stbi__err("too large"));
                                s.img_y = (uint)(StbImage.stbi__get32be(s));
                                if ((s.img_y) > (1 << 24))
                                    return (int)(StbImage.stbi__err("too large"));
                                z.depth = (int)(StbImage.stbi__get8(s));
                                if (((((z.depth != 1) && (z.depth != 2)) && (z.depth != 4)) && (z.depth != 8)) && (z.depth != 16))
                                    return (int)(StbImage.stbi__err("1/2/4/8/16-bit only"));
                                color = (int)(StbImage.stbi__get8(s));
                                if ((color) > (6))
                                    return (int)(StbImage.stbi__err("bad ctype"));
                                if (((color) == (3)) && ((z.depth) == (16)))
                                    return (int)(StbImage.stbi__err("bad ctype"));
                                if ((color) == (3))
                                    pal_img_n = (byte)(3);
                                else if ((color & 1) != 0)
                                    return (int)(StbImage.stbi__err("bad ctype"));
                                comp = (int)(StbImage.stbi__get8(s));
                                if ((comp) != 0)
                                    return (int)(StbImage.stbi__err("bad comp method"));
                                filter = (int)(StbImage.stbi__get8(s));
                                if ((filter) != 0)
                                    return (int)(StbImage.stbi__err("bad filter method"));
                                interlace = (int)(StbImage.stbi__get8(s));
                                if ((interlace) > (1))
                                    return (int)(StbImage.stbi__err("bad interlace method"));
                                if ((s.img_x == 0) || (s.img_y == 0))
                                    return (int)(StbImage.stbi__err("0-pixel image"));
                                if (pal_img_n == 0)
                                {
                                    s.img_n = (int)(((color & 2) != 0 ? 3 : 1) + ((color & 4) != 0 ? 1 : 0));
                                    if (((1 << 30) / s.img_x / s.img_n) < (s.img_y))
                                        return (int)(StbImage.stbi__err("too large"));
                                    if ((scan) == (StbImage.STBI__SCAN_header))
                                        return (int)(1);
                                }
                                else
                                {
                                    s.img_n = (int)(1);
                                    if (((1 << 30) / s.img_x / 4) < (s.img_y))
                                        return (int)(StbImage.stbi__err("too large"));
                                }
                                break;
                            }
                        case (((uint)('P') << 24) + ((uint)('L') << 16) + ((uint)('T') << 8) + (uint)('E')):
                            {
                                if ((first) != 0)
                                    return (int)(StbImage.stbi__err("first not IHDR"));
                                if ((c.length) > (256 * 3))
                                    return (int)(StbImage.stbi__err("invalid PLTE"));
                                pal_len = (uint)(c.length / 3);
                                if (pal_len * 3 != c.length)
                                    return (int)(StbImage.stbi__err("invalid PLTE"));
                                for (i = (uint)(0); (i) < (pal_len); ++i)
                                {
                                    palette[i * 4 + 0] = (byte)(StbImage.stbi__get8(s));
                                    palette[i * 4 + 1] = (byte)(StbImage.stbi__get8(s));
                                    palette[i * 4 + 2] = (byte)(StbImage.stbi__get8(s));
                                    /*
                                     *  Here is the magic
                                     */
                                    if (i == 0)
                                    {
                                        palette[i * 4 + 3] = 0;
                                    }
                                    else
                                    {
                                        palette[i * 4 + 3] = (byte)(255);
                                    }
                                }
                                break;
                            }
                        case (((uint)('t') << 24) + ((uint)('R') << 16) + ((uint)('N') << 8) + (uint)('S')):
                            {
                                if ((first) != 0)
                                    return (int)(StbImage.stbi__err("first not IHDR"));
                                if ((z.idata) != null)
                                    return (int)(StbImage.stbi__err("tRNS after IDAT"));
                                if ((pal_img_n) != 0)
                                {
                                    if ((scan) == (StbImage.STBI__SCAN_header))
                                    {
                                        s.img_n = (int)(4);
                                        return (int)(1);
                                    }
                                    if ((pal_len) == (0))
                                        return (int)(StbImage.stbi__err("tRNS before PLTE"));
                                    if ((c.length) > (pal_len))
                                        return (int)(StbImage.stbi__err("bad tRNS len"));
                                    pal_img_n = (byte)(4);
                                    for (i = (uint)(0); (i) < (c.length); ++i)
                                    {
                                        palette[i * 4 + 3] = (byte)(StbImage.stbi__get8(s));
                                    }
                                }
                                else
                                {
                                    if ((s.img_n & 1) == 0)
                                        return (int)(StbImage.stbi__err("tRNS with alpha"));
                                    if (c.length != (uint)(s.img_n) * 2)
                                        return (int)(StbImage.stbi__err("bad tRNS len"));
                                    has_trans = (byte)(1);
                                    if ((z.depth) == (16))
                                    {
                                        for (k = (int)(0); (k) < (s.img_n); ++k)
                                        {
                                            tc16[k] = ((ushort)(StbImage.stbi__get16be(s)));
                                        }
                                    }
                                    else
                                    {
                                        for (k = (int)(0); (k) < (s.img_n); ++k)
                                        {
                                            tc[k] = (byte)((byte)(StbImage.stbi__get16be(s) & 255) * StbImage.stbi__depth_scale_table[z.depth]);
                                        }
                                    }
                                }
                                break;
                            }
                        case (((uint)('I') << 24) + ((uint)('D') << 16) + ((uint)('A') << 8) + (uint)('T')):
                            {
                                if ((first) != 0)
                                    return (int)(StbImage.stbi__err("first not IHDR"));
                                if (((pal_img_n) != 0) && (pal_len == 0))
                                    return (int)(StbImage.stbi__err("no PLTE"));
                                if ((scan) == (StbImage.STBI__SCAN_header))
                                {
                                    s.img_n = (int)(pal_img_n);
                                    return (int)(1);
                                }
                                if (((int)(ioff + c.length)) < ((int)(ioff)))
                                    return (int)(0);
                                if ((ioff + c.length) > (idata_limit))
                                {
                                    uint idata_limit_old = (uint)(idata_limit);
                                    byte* p;
                                    if ((idata_limit) == (0))
                                        idata_limit = (uint)((c.length) > (4096) ? c.length : 4096);
                                    while ((ioff + c.length) > (idata_limit))
                                    {
                                        idata_limit *= (uint)(2);
                                    }
                                    p = (byte*)(CRuntime.realloc(z.idata, (ulong)(idata_limit)));
                                    if ((p) == (null))
                                        return (int)(StbImage.stbi__err("outofmem"));
                                    z.idata = p;
                                }
                                if (StbImage.stbi__getn(s, z.idata + ioff, (int)(c.length)) == 0)
                                    return (int)(StbImage.stbi__err("outofdata"));
                                ioff += (uint)(c.length);
                                break;
                            }
                        case (((uint)('I') << 24) + ((uint)('E') << 16) + ((uint)('N') << 8) + (uint)('D')):
                            {
                                uint raw_len = 0;
                                uint bpl = 0;
                                if ((first) != 0)
                                    return (int)(StbImage.stbi__err("first not IHDR"));
                                if (scan != StbImage.STBI__SCAN_load)
                                    return (int)(1);
                                if ((z.idata) == (null))
                                    return (int)(StbImage.stbi__err("no IDAT"));
                                bpl = (uint)((s.img_x * z.depth + 7) / 8);
                                raw_len = (uint)(bpl * s.img_y * s.img_n + s.img_y);
                                z.expanded = (byte*)(StbImage.stbi_zlib_decode_malloc_guesssize_headerflag((sbyte*)(z.idata), (int)(ioff), (int)(raw_len), (int*)(&raw_len), is_iphone != 0 ? 0 : 1));
                                if ((z.expanded) == (null))
                                    return (int)(0);
                                CRuntime.free(z.idata);
                                z.idata = (null);
                                if (((((req_comp) == (s.img_n + 1)) && (req_comp != 3)) && (pal_img_n == 0)) || ((has_trans) != 0))
                                    s.img_out_n = (int)(s.img_n + 1);
                                else
                                    s.img_out_n = (int)(s.img_n);
                                if (StbImage.stbi__create_png_image(z, z.expanded, (uint)(raw_len), (int)(s.img_out_n), (int)(z.depth), (int)(color), (int)(interlace)) == 0)
                                    return (int)(0);
                                if ((has_trans) != 0)
                                {
                                    if ((z.depth) == (16))
                                    {
                                        if (StbImage.stbi__compute_transparency16(z, tc16, (int)(s.img_out_n)) == 0)
                                            return (int)(0);
                                    }
                                    else
                                    {
                                        if (StbImage.stbi__compute_transparency(z, tc, (int)(s.img_out_n)) == 0)
                                            return (int)(0);
                                    }
                                }
                                if ((((is_iphone) != 0) && ((StbImage.stbi__de_iphone_flag) != 0)) && ((s.img_out_n) > (2)))
                                    StbImage.stbi__de_iphone(z);
                                if ((pal_img_n) != 0)
                                {
                                    s.img_n = (int)(pal_img_n);
                                    s.img_out_n = (int)(pal_img_n);
                                    if ((req_comp) >= (3))
                                        s.img_out_n = (int)(req_comp);
                                    if (StbImage.stbi__expand_png_palette(z, palette, (int)(pal_len), (int)(s.img_out_n)) == 0)
                                        return (int)(0);
                                }
                                else if ((has_trans) != 0)
                                {
                                    ++s.img_n;
                                }
                                CRuntime.free(z.expanded);
                                z.expanded = (null);
                                return (int)(1);
                            }
                        default:
                            if ((first) != 0)
                                return (int)(StbImage.stbi__err("first not IHDR"));
                            if ((c.type & (1 << 29)) == (0))
                            {
                                string invalid_chunk = c.type + " PNG chunk not known";
                                return (int)(StbImage.stbi__err(invalid_chunk));
                            }
                            StbImage.stbi__skip(s, (int)(c.length));
                            break;
                    }
                    StbImage.stbi__get32be(s);
                }
            }

            static void* stbi__do_png(StbImage.stbi__png p, int* x, int* y, int* n, int req_comp, StbImage.stbi__result_info* ri)
            {
                void* result = (null);
                if (((req_comp) < (0)) || ((req_comp) > (4)))
                    return ((byte*)((ulong)((StbImage.stbi__err("bad req_comp")) != 0 ? ((byte*)null) : (null))));
                if ((stbi__parse_png_file(p, (int)(StbImage.STBI__SCAN_load), (int)(req_comp))) != 0)
                {
                    if ((p.depth) < (8))
                        ri->bits_per_channel = (int)(8);
                    else
                        ri->bits_per_channel = (int)(p.depth);
                    result = p._out_;
                    p._out_ = (null);
                    if (((req_comp) != 0) && (req_comp != p.s.img_out_n))
                    {
                        if ((ri->bits_per_channel) == (8))
                            result = StbImage.stbi__convert_format((byte*)(result), (int)(p.s.img_out_n), (int)(req_comp), (uint)(p.s.img_x), (uint)(p.s.img_y));
                        else
                            result = StbImage.stbi__convert_format16((ushort*)(result), (int)(p.s.img_out_n), (int)(req_comp), (uint)(p.s.img_x), (uint)(p.s.img_y));
                        p.s.img_out_n = (int)(req_comp);
                        if ((result) == (null))
                            return result;
                    }
                    *x = (int)(p.s.img_x);
                    *y = (int)(p.s.img_y);
                    if ((n) != null)
                        *n = (int)(p.s.img_n);
                }

                CRuntime.free(p._out_);
                p._out_ = (null);
                CRuntime.free(p.expanded);
                p.expanded = (null);
                CRuntime.free(p.idata);
                p.idata = (null);
                return result;
            }
            static void* stbi__png_load(StbImage.stbi__context s, int* x, int* y, int* comp, int req_comp, StbImage.stbi__result_info* ri)
            {
                StbImage.stbi__png p = new StbImage.stbi__png();
                p.s = s;
                return stbi__do_png(p, x, y, comp, (int)(req_comp), ri);
            }

            static void* stbi__load_main(StbImage.stbi__context s, int* x, int* y, int* comp, int req_comp, StbImage.stbi__result_info* ri, int bpc)
            {
                ri->bits_per_channel = (int)(8);
                ri->channel_order = (int)(StbImage.STBI_ORDER_RGB);
                ri->num_channels = (int)(0);
                if ((StbImage.stbi__png_test(s)) != 0)
                    return stbi__png_load(s, x, y, comp, (int)(req_comp), ri);
                return ((byte*)((ulong)((StbImage.stbi__err("unknown image type")) != 0 ? ((byte*)null) : (null))));
            }

            static byte* stbi__load_and_postprocess_8bit(StbImage.stbi__context s, int* x, int* y, int* comp, int req_comp)
            {
                StbImage.stbi__result_info ri = new StbImage.stbi__result_info();
                void* result = stbi__load_main(s, x, y, comp, (int)(req_comp), &ri, (int)(8));
                if ((result) == (null))
                    return (null);
                if (ri.bits_per_channel != 8)
                {
                    result = StbImage.stbi__convert_16_to_8((ushort*)(result), (int)(*x), (int)(*y), (int)((req_comp) == (0) ? *comp : req_comp));
                    ri.bits_per_channel = (int)(8);
                }

                if ((StbImage.stbi__vertically_flip_on_load) != 0)
                {
                    int channels = (int)((req_comp) != 0 ? req_comp : *comp);
                    StbImage.stbi__vertical_flip(result, (int)(*x), (int)(*y), (int)(channels * sizeof(byte)));
                }

                return (byte*)(result);
            }

            static byte* stbi_load_from_memory(byte* buffer, int len, int* x, int* y, int* comp, int req_comp)
            {
                StbImage.stbi__context s = new StbImage.stbi__context();
                StbImage.stbi__start_mem(s, buffer, (int)(len));
                return stbi__load_and_postprocess_8bit(s, x, y, comp, (int)(req_comp));
            }



            byte* result = null;

            try
            {
                int x, y, comp;
                fixed (byte* buffer = bytes)
                {
                    result = stbi_load_from_memory(buffer, bytes.Length, &x, &y, &comp, (int)ColorComponents.RedGreenBlueAlpha);
                }

                return FromResult(result, x, y, (ColorComponents)comp, ColorComponents.RedGreenBlueAlpha);
            }
            finally
            {
                if (result != null)
                {
                    CRuntime.free(result);
                }
            }
        }
    }
}