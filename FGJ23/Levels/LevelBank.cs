using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FGJ23.Core;
using FGJ23.Support;
using FGJ23.Entities;

namespace FGJ23.Levels
{
    public static class LevelBank
    {
        public static Proto.Level GetLevel(string name)
        {
            Log.Information("GetLevel {name}", name);
            return name switch
            {
                "level1" => Level1(),
                "level2" => Level2(),
                "level3" => Level3(),
                "level4" => Level4(),
                "level5" => Level5(),
                "level6" => Level6(),
                "level7" => Level7(),
                "level8" => Level8(),
                "level9" => Level9(),
                "level10" => Level10(),
                "level11" => Level11(),
                "level12" => Level12(),
                "level13" => Level13(),
                "level14" => Level14(),
                "" => null,
                _ => throw new Exception("Unknown level name: " + name),
            };
        }

        /*
         * ABCDE
         * FGHIJ
         * KLMNO
         */
        // Bit order: content top right bottom left
        // with content most significant bit
        static string[] tiles = new string[] {
            // 00000
            "OOOOOOOO",
            // 01000
            "OOOOOOOO",
            // 10000
            "NNNC",
            // 10100
            "NNAB",
            // 11000
            "NMNH",
            // 11100
            "KLFG",
        };

        static int getMagic( int content, int top, int right, int bottom, int left) {
            // what the fuck?
            return (
                (content << 4) |
                (top << 3) |
                (right << 2) |
                (bottom << 1) |
                (left << 0)
           );
        }

        private static int get(int x, int y, string[] layers) {
            if( x < 0 || y < 0 || x >= layers[0].Length || y >= layers.Length) {
                return 1;
            }
            return layers[y][x] == '.' ? 0 : 1;
        }

        private static List<int> unpack(int delta, string[] layers) {
            var maxy = layers.Length;
            var maxx = layers[0].Length;
            string jtiles = String.Join("", tiles);

            List<int> res = new List<int>();
            for(int y = 0; y < maxy; y++) {
                for(int x = 0; x < maxx; x++) {
                    var content = get(x, y, layers);
                    var top = get(x, y-1, layers);
                    var right = get(x+1, y, layers);
                    var bottom = get(x, y+1, layers);
                    var left = get(x-1, y, layers);

                    var magic = getMagic(content, top, right, bottom, left);
                    var t = jtiles[magic]; 

                    if(t == 'G') {
                        if(get(x-1, y-1, layers) == 0) t = 'J';
                        if(get(x+1, y-1, layers) == 0) t = 'I';
                        if(get(x-1, y+1, layers) == 0) t = 'E';
                        if(get(x+1, y+1, layers) == 0) t = 'D';
                    }

                    res.Add(t - 'A' + delta);
                }
            }
            return res;
        }

        public static Proto.Layer createLayer(int offset, string[] data) {
            var protoLayer = new Proto.Layer() {
                TilesetIndex = 0,
                 Height = data.Length,
                 Width = data[0].Length,
                 LoopX = false,
                 LoopY = false,
                 XSpeed = 1,
                 YSpeed = 1,
            };
            protoLayer.Tiles.AddRange(unpack(offset, data));

            return protoLayer;
        }

        public static Proto.Level Base(string id, string tileset, string music) {
            var lev = new Proto.Level
            {
                Title = "Title",
                Name = id,
                SpriteLayer = 0,
                Music = music
            };

            var protoTileset = new Proto.Tileset()
            {
                Image = tileset,
                Mask = tileset,
            };
            lev.Tilesets.Add(protoTileset);

            return lev;
        }

        public static Proto.AreaEvent levelEnd(int x, int y, string data) {
            return new Proto.AreaEvent()
            {
                Id = Proto.AreaEvent.Types.AreaEventId.LevelEnd,
                   X = x * 16,
                   Y = y * 16,
                   Width = 32,
                   Height = 32,
                   Data = ByteString.CopyFromUtf8(data)
            };
        }

        public static Proto.CoordinateEvent enemy(int x, int y, string sprites) {
            return new Proto.CoordinateEvent()
            {
                    Id = Proto.CoordinateEvent.Types.CoordinateEventId.Enemy,
                    X = x * 16 - 8,
                    Y = y * 16 - 8,
                    Data = ByteString.CopyFromUtf8(sprites)
            };
        }


#pragma warning disable format
        public static Proto.Level Level1()
        {
            var lev = Base("level1", "eka_maa&tausta.png", "Musa2");
            {
                var protoLayer = createLayer(0, new string[] {
                    "########################################",
                    "#........................####..........#",
                    "#........................####..........#",
                    "#..######.............###########......#",
                    "#..######.............###########......#",
                    "#.....................###########......#",
                    "#####...########......###########......#",
                    "#####...########......###########......#",
                    "#.......########.......###.............#",
                    "#......................###.............#",
                    "#......................###.............#",
                    "#..........##########################..#",
                    "#..........##########################..#",
                    "#..........##########################..#",
                    "#......................................#",
                    "#......................................#",
                    "#..#######.............................#",
                    "#..#######..#######....................#",
                    "#..#######..#######....................#",
                    "#..#######..#######....................#",
                    "########################################",
                    "########################################",
                    "###..................###################",
                    "###..................###################",
                    "###........###..........################",
                    "###........###..........################",
                    "###........#############################",
                    "########################################",
                    "########################################",
                    "########################################",
                });

                protoLayer.AreaEvents.Add(levelEnd(31, 2, "Eka_voittolinna.png;level2"));
                protoLayer.CoordinateEvents.Add(enemy(23, 11, "Eka_hahmot.png"));
                lev.Layers.Add(protoLayer);
            }

            {
                var protoLayer = createLayer(15, new string[] {
                    "........................................",
                    "......................####..............",
                    "......................####..............",
                    "......................####..............",
                    "....................######..............",
                    ".....###............######..............",
                    ".....###...........#############........",
                    ".....###...........#############........",
                    ".#######.........###############........",
                    ".###############################........",
                    ".#################################......",
                    ".#######################################",
                    ".#######################################",
                    ".#######################################",
                    ".#########.......#######......####......",
                    ".#########.......#######......####......",
                    ".###########.....#######......####......",
                    ".#######################......####......",
                    "..........##############......####......",
                    "..........##############......####......",
                    "..........##############......####......",
                    ".................#######......####......",
                    "########################################",
                    "########################################",
                    "########################################",
                    "########################################",
                    "########################################",
                    "########################################",
                    "########################################",
                    "########################################",
                });

                lev.Layers.Add(protoLayer);
            }

            return lev;
        }

        public static Proto.Level Level2()
        {
            var lev = Base("level2", "eka_maa&tausta.png", "Musa2");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "Eka_voittolinna.png;level3"));
            layer.CoordinateEvents.Add(enemy(6, 6, "Eka_hahmot.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level3()
        {
            var lev = Base("level3", "eka_maa&tausta.png", "Musa2");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "Eka_voittolinna.png;level4"));
            layer.CoordinateEvents.Add(enemy(6, 6, "Eka_hahmot.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level4()
        {
            var lev = Base("level4", "eka_maa&tausta.png", "Musa2");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "Eka_voittolinna.png;level5"));
            layer.CoordinateEvents.Add(enemy(6, 6, "Eka_hahmot.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level5()
        {
            var lev = Base("level5", "eka_maa&tausta.png", "Musa2");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "Eka_voittolinna.png;level6"));
            layer.CoordinateEvents.Add(enemy(6, 6, "Eka_hahmot.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level6()
        {
            var lev = Base("level6", "MB_maa&tausta.png", "Musa3");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "MB_voittolinna.png;level7"));
            layer.CoordinateEvents.Add(enemy(6, 6, "MB_hahmot.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level7()
        {
            var lev = Base("level7", "MB_maa&tausta.png", "Musa3");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "MB_voittolinna.png;level8"));
            layer.CoordinateEvents.Add(enemy(6, 6, "MB_hahmot.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level8()
        {
            var lev = Base("level8", "parineliota_maa&tausta.png", "Musa4");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "parineliota_voittolinna.png;level9"));
            layer.CoordinateEvents.Add(enemy(6, 6, "parineliota_hahmot.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level9()
        {
            var lev = Base("level9", "literallynelio_maa&tausta_color.png", "Musa5");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "LN_voittolinna_color.png;level10"));
            layer.CoordinateEvents.Add(enemy(6, 6, "LN_hahmot_color.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level10()
        {
            var lev = Base("level10", "literallynelio_maa&tausta_BW.png", "Musa5");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "LN_voittolinna_BW.png;level11"));
            layer.CoordinateEvents.Add(enemy(6, 6, "LN_hahmot_BW.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level11()
        {
            var lev = Base("level11", "literallynelio_maa&tausta_BW.png", "Musa5");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "LN_voittolinna_BW.png;level12"));
            layer.CoordinateEvents.Add(enemy(6, 6, "LN_hahmot_BW.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level12()
        {
            var lev = Base("level12", "literallynelio_maa&tausta_BW.png", "Musa5");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "LN_voittolinna_BW.png;level13"));
            layer.CoordinateEvents.Add(enemy(6, 6, "LN_hahmot_BW.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level13()
        {
            var lev = Base("level13", "literallynelio_maa&tausta_BW.png", "");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "LN_voittolinna_BW.png;level14"));
            layer.CoordinateEvents.Add(enemy(6, 6, "LN_hahmot_BW.png"));
            lev.Layers.Add(layer);
            return lev;
        }

        public static Proto.Level Level14()
        {
            var lev = Base("level14", "literallynelio_maa&tausta_BW.png", "");
            var layer = createLayer(0, new string[] {
                "##########",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "#........#",
                "##########",
            });

            layer.AreaEvents.Add(levelEnd(8, 6, "LN_voittolinna_BW.png;"));
            layer.CoordinateEvents.Add(enemy(6, 6, "LN_hahmot_BW.png"));
            lev.Layers.Add(layer);
            return lev;
        }

#pragma warning restore format
        public static StoryComponent getStartStory(string name)
        {
            return name switch
            {
                "level1" =>
                    new StoryBuilder()
                    .Line("Files/Eka_seta_selittaa.png", "", "I was born in 1982. Back in my day, games were about CONTENT. You'd walk around with the arrow keys. Jump by pressing up.")
                    .Exposition("Maybe shoot some bad guys with the space bar. Double jump by jumping again in the air and wall jump by pointing towards a wall in mid-air and jumping. The good old days")
                    .Exposition("Nowadays the kids can't stay focused for more than two seconds unless it's some virtual reality thirst trap.")
                    .GoToLevel(),

                "level2" =>
                    new StoryBuilder()
                    .Exposition("2082. What a year this could've been for games. A century of evolution from the legends of yore.")
                    .Exposition("Every first level should be easy, but showcase the main features without hand-holding.")
                    .Exposition("There's an enemy. You can shoot it. You may need to jump over things to progress.")
                    .GoToLevel(),

                "level3" =>
                    new StoryBuilder()
                    .Exposition("Sometimes just jumping is not enough. Sometimes an enemy is in your way. Sometimes, you have to make the right choices")
                    .Exposition("And a grand finale, a true puzzle! Do I jump here or there?")
                    .GoToLevel(),

                "level4" =>
                    new StoryBuilder()
                    .Exposition("That didn't feel right. It has to be rewarding! You have to use all your jumps to clear it!")
                    .GoToLevel(),

                "level5" =>
                    new StoryBuilder()
                    .Exposition("THERE we go! A proper, pure challenge! Hit it just right. Special jumps are for whippersnappers.")
                    .GoToLevel(),

                "level6" =>
                    new StoryBuilder()
                    .Exposition("Graphics became frivolous much too early. You don’'t need more than a suggestion. A hint to jump-start your own imagination. No spoon-fed high-definition dreamscapes")
                    .Exposition("Change of pace. Enemies in the way. More of them. A challenge of a different kind. A satisfying contrast.")
                    .GoToLevel(),

                "level7" =>
                    new StoryBuilder()
                    .Exposition("Shooting from afar is cowardly. You have to make it personal. Look losing your only life right in the eye to clear every single monster. ")
                    .GoToLevel(),

                "level8" =>
                    new StoryBuilder()
                    .Exposition("Back in my day, every mechanic was necessary. No useless frills.\r\n")
                    .Exposition("You jump higher if you jump on an enemy. That's a clue. You need it somewhere. Use it.")
                    .GoToLevel(),

                "level9" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level10" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level11" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level12" =>
                    new StoryBuilder()
                    .GoToLevel(),

                _ => throw new Exception("Unknown level name: " + name),

            };
        }

        public static StoryComponent getEndStory(string name)
        {
            return name switch
            {
                "level1" =>
                    new StoryBuilder()
                    .Exposition("Ever since the 2050's, kids have been unable to function without constantly tending to their retinal and cochlear implants.")
                    .Exposition("Sharing four-dimensional memes. Living third lives on some D-Corporation Hyperverse.")
                    .Exposition("I want none of that. I just want a challenge and to reach that castle on the far right.")
                    .Exposition("I retired this year at the ripe old age of one hundred, and I intend to spend the rest of my days enjoying what games should always have been about!")
                    .GoToLevel(),

                "level2" =>
                    new StoryBuilder()
                    .Exposition("Simple. Pure. Beautiful.")
                    .GoToLevel(),

                "level3" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level4" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level5" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level6" =>
                    new StoryBuilder()
                    .Exposition("You really gave it to them!")
                    .GoToLevel(),
                    
                "level7" =>
                    new StoryBuilder()
                    .Exposition("THIS is true skill. THIS is how you feel accomplishment. ")
                    .GoToLevel(),

                "level8" =>
                     new StoryBuilder()
                    .Exposition("Chekhov’s guns are sloppy writing.")
                    .Exposition("This is perfect form, that requires perfect execution of the player. Exquisite.")
                    .GoToLevel(),

                "level9" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level10" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level11" =>
                    new StoryBuilder()
                    .GoToLevel(),

                "level12" =>
                    new StoryBuilder()
                    .GoToLevel(),

                _ => throw new Exception("Unknown level name: " + name),
            };
        }

        public static void setPlayerAttributes(Player player, string name)
        {
            switch (name) {
                case "level1":
                case "level2":
                case "level3":
                case "level4":
                case "level5":
                    player.CanJump = true;
                    player.CanWalljump = true;
                    player.CanShoot = true;
                    player.ExtraJumps = 1;
                    break;

                case "level6":
                case "level7":
                case "level8":
                case "level9":
                case "level10":
                case "level11":
                case "level12":
                    player.CanJump = true;
                    player.CanWalljump = false;
                    player.CanShoot = false;
                    player.ExtraJumps = 0;
                    break;

                case "level13":
                case "level14":
                    player.CanJump = false;
                    player.CanWalljump = false;
                    player.CanShoot = false;
                    player.ExtraJumps = 0;
                    break;

                default:
                    throw new Exception("Unknown level name: " + name);
            }
        }
    }
}
