using System;
using System.IO;
using Il2CppSystem.Text.RegularExpressions;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace FlxsnxMod
{
    [HarmonyPatch(typeof(MapUI), "Ping")]
    public class Flx : MelonMod
    {
        static bool showMenu = false;
        Manager m;
        ChatWindow chat;
        string itemID = "1";
        float amount = 1;
        public const string version = "1.4.2";
        Vector2 vSbarValue;
        bool ThunderBeam = false;
        AssetBundle Flxasset;
        GUIStyle buttonStyle;
        Color32 ModColor = new Color32(165, 151, 115, 255);
        Rect WindowRect = new Rect(0, (Screen.height - 420) / 2 + 30, 330, 420);
        // GUIStyle ModIcon;
        GUIStyle CloseButton;
        GUIStyle OpenButton;
        static bool MapPing = false;
        static bool rangeClear = false;
        static float clearRange = 5.0f;
        static bool oneTapClearTile = false;
        static bool pullLootToPlayer = false;

        bool showAddItem = false;
        bool showSkill = false;
        bool showOther = false;
        bool showOther2 = false;
        bool showNewRangeClear = false;

        [Obsolete]
        public override void OnApplicationStart()
        {
            Flxasset = AssetBundle.LoadFromMemory(FlxsnxMod.Properties.Res.mod);
            buttonStyle = new GUIStyle()
            {
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("button")
                },
                active = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("button3")
                },
                hover = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("button2")
                },
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                padding = new RectOffset(4, 4, 4, 4),
                margin = new RectOffset(5, 5, 0, 5)
            };

            /*ModIcon = new GUIStyle()
            {
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("avatar")
                },
                fixedHeight = 28,
                fixedWidth = 28,
                margin = new RectOffset(0, 0, 10, 10)
            };*/

            CloseButton = new GUIStyle()
            {
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("button")
                },
                active = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("button3")
                },
                hover = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("button2")
                },
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                margin = new RectOffset(0, 0, 10, 10),
                fontSize = 11
            };

            OpenButton = new GUIStyle()
            {
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("button")
                },
                active = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("button3")
                },
                hover = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("button2")
                },
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                margin = new RectOffset(0, 6, 6, 0),
                fontSize = 11,
            };
        }

        public override void OnUpdate()

        {
            if (m == null)
            {
                m = GameObject.FindObjectOfType<Manager>();
            }

            if (showMenu && Input.GetMouseButton(0))
            {
                m._inputManager.DisableInput(0.1f);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (ThunderBeam)
                {

                    if (isAdminPlayer())
                    {
                        m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), m.player.aimDirection, m.player.entity,114514);
                    }

                }
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                if (ThunderBeam)
                {
                    if (isAdminPlayer())
                    {
                        superThunderBeam();
                    }

                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {

                if (!chat)
                {
                    chat = GameObject.FindObjectOfType<ChatWindow>();
                }
                var text = chat.GetInputText();
                LoggerInstance.Msg(text);
                if (text != "")
                {
                    var code = text.Split(' ');
                    LoggerInstance.Msg(code[0]);

                    if (code.Length >= 2 && code[1] != null && code[1] != "")
                    {
                        if (code[0] == "/item" && IsNumeric(code[2]))
                        {
                            if (isAdminPlayer())
                            {
                                if ((code.Length == 4 && code[3] != null && code[3] != "" && IsNumeric(code[3])))
                                {
                                    m.player.playerCommandSystem.CreateAndDropEntity((ObjectID)Enum.Parse(typeof(ObjectID), code[1]), m.player.RenderPosition + new Vector3(0, 2f, 0), (int)Int32.Parse(code[2]), variation: (int)Int32.Parse(code[3]));
                                }
                                else
                                {
                                    m.player.playerCommandSystem.CreateAndDropEntity((ObjectID)Enum.Parse(typeof(ObjectID), code[1]), m.player.RenderPosition + new Vector3(0, 2f, 0), (int)Int32.Parse(code[2]));
                                }
                            }
                        }

                        if (code[0] == "/buff" && code.Length >= 3 && code[2] != null && code[2] != "")
                        {
                            if (code[1] == "add" && code.Length == 4 && code[3] != null && code[3] != "" && IsNumeric(code[3]))
                            {
                                addBuff(code[2], (int)Int32.Parse(code[3]));
                            }
                            else if (code[1] == "del")
                            {
                                delBuff(code[2], 0);
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.F11) || Input.GetKeyDown(KeyCode.BackQuote))
            {
                showMenu = !showMenu;
                if (showMenu == true)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        public override void OnGUI()
        {
            var oldbackground = GUI.backgroundColor;
            GUI.backgroundColor = new Color32(48, 48, 48, 255);
            GUI.skin.window = new GUIStyle()
            {
                normal = new GUIStyleState
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("window"),
                },
                padding = new RectOffset(25, 25, 25, 25),
                wordWrap = true
            };
            GUI.skin.verticalScrollbarThumb = new GUIStyle()
            {
                normal = new GUIStyleState
                {
                    background = Flxasset.LoadAsset<Texture2D>("button")
                },
                fixedWidth = 6
            };
            GUI.skin.horizontalSliderThumb = new GUIStyle()
            {
                normal = new GUIStyleState
                {
                    background = Flxasset.LoadAsset<Texture2D>("button")
                },
                fixedWidth = 10,
                fixedHeight = 10
            };
            GUI.skin.verticalScrollbar = new GUIStyle()
            {
                normal = new GUIStyleState
                {
                    background = Flxasset.LoadAsset<Texture2D>("windows")
                },
                fixedWidth = 6
            };

            GUILayout.BeginArea(new Rect(5, 5, 120, 30));



            if (showMenu == false)
            {
                if (GUILayout.Button("FlxsnxMod", buttonStyle, new GUILayoutOption[] { GUILayout.Height(30) }))
                {
                    showMenu = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            GUILayout.EndArea();
            if (showMenu)
            {
                GUI.backgroundColor = oldbackground;
                WindowRect = GUI.Window(20029933, WindowRect, (GUI.WindowFunction)menuWindow, "");
            }
        }

        void menuWindow(int winId)
        {
            if (m == null)
            {
                m = GameObject.FindObjectOfType<Manager>();
            }

            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(0,0,0,6)},new GUILayoutOption[0]);

            // GUILayout.Button("", ModIcon, new GUILayoutOption[0]);
            GUILayout.Label("FlxsnxMod V" + version, new GUIStyle()
            {
                normal = new GUIStyleState
                {
                    textColor = Color.white,

                },
                margin = new RectOffset(6, 0, 6, 10),
                padding = new RectOffset(0, 0, 10, 0),
                fixedHeight = 28
            }, new GUILayoutOption[0]); ;
            GUI.backgroundColor = new Color32(245, 45, 45, 255);
            if (GUILayout.Button("X", CloseButton, new GUILayoutOption[] { GUILayout.Width(25), GUILayout.Height(25) }))
            {
                showMenu = false;
            }

            GUILayout.EndHorizontal();

            GUI.backgroundColor = new Color32(58, 58, 58, 255);
            vSbarValue = GUILayout.BeginScrollView(vSbarValue, false, true, new GUILayoutOption[0]);
            GUI.backgroundColor = ModColor;

            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            bool link = GUILayout.Button("GitHub", buttonStyle, new GUILayoutOption[0]);
            if (link)
            {
               System.Diagnostics.Process.Start("https://github.com/FlxSNX/FlxSNXMod");
            }
            bool dumpid = GUILayout.Button("导出物品ID", buttonStyle, new GUILayoutOption[0]);
            if (dumpid)
            {
                StreamWriter dumpItmeID = File.CreateText(Application.dataPath + "/../itemID.txt");
                dumpItmeID.WriteLine("DumpObjectIDTool By FlxSNXMod");
                foreach (ObjectID item in Enum.GetValues(typeof(ObjectID)))
                {
                    dumpItmeID.WriteLine(PugText.ProcessText($"Items/{item}", new Il2CppStringArray(new string[] { }), true, false) + "  " + item + "  " + item.GetHashCode());
                }
                dumpItmeID.Close();
                dumpItmeID.Dispose();
                LoggerInstance.Msg("itemID.txt导出完成,请在游戏目录下查看");
            }
            bool dumpbuffid = GUILayout.Button("导出buffID", buttonStyle, new GUILayoutOption[0]);
            if (dumpbuffid)
            {
                StreamWriter dumpItmeID = File.CreateText(Application.dataPath + "/../buffID.txt.txt");
                dumpItmeID.WriteLine("DumpConditionIDTool By FlxSNXMod");
                foreach (ConditionID buff in Enum.GetValues(typeof(ConditionID)))
                {
                    dumpItmeID.WriteLine(PugText.ProcessText($"Conditions/{buff}", new Il2CppStringArray(new string[] { }), true, false) + "  " + buff + "  " + buff.GetHashCode());
                }
                dumpItmeID.Close();
                dumpItmeID.Dispose();
                LoggerInstance.Msg("buffID.txt导出完成,请在游戏目录下查看");
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("游戏加速:" + (int)UnityEngine.Time.timeScale + "X", new GUILayoutOption[0]);
            UnityEngine.Time.timeScale = GUILayout.HorizontalSlider((int)UnityEngine.Time.timeScale, 1, 50, new GUILayoutOption[0]);

            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("一键清空背包", buttonStyle, new GUILayoutOption[0]))
            {
                var ih = m.player.playerInventoryHandler;
                for (int i = 10; i < ih.size; i++)
                {
                    var info = ih.GetObjectData(i);
                    ih.DestroyObject(i, info.objectID);
                }
            }
            if (GUILayout.Button("背包物品999", buttonStyle, new GUILayoutOption[0]))
            {
                var ih = m.player.playerInventoryHandler;
                for (int i = 10; i < ih.size; i++)
                {
                    var info = ih.GetObjectData(i);
                    if (info.objectID != ObjectID.None)
                    {
                        ih.SetAmount(i, info.objectID, 999);
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("不会饥饿", buttonStyle, new GUILayoutOption[0]))
            {
                m.player.playerCommandSystem.ToggleCanConsumeHunger(m.player.entity, false);
            }
            if (GUILayout.Button("会饿", buttonStyle, new GUILayoutOption[0]))
            {
                m.player.playerCommandSystem.ToggleCanConsumeHunger(m.player.entity, true);
            }
            if (GUILayout.Button("不耗耐久", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("ToolDurabilityLastsLonger", 100);
                addBuff("EquipmentDurabilityLastsLonger", 100);
            }
            if (GUILayout.Button("还原耐久", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("ToolDurabilityLastsLonger", 0);
                delBuff("EquipmentDurabilityLastsLonger", 0);
            }
            GUILayout.EndHorizontal();

            /* 添加物品功能区 */
            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(0, 0, 0, 8) }, new GUILayoutOption[0]);
            GUILayout.Label("添加物品 [该功能仅限管理员玩家使用]", new GUILayoutOption[0]);
            if (GUILayout.Button(showAddItem ? "x" : "+", OpenButton, new GUILayoutOption[] { GUILayout.Width(20), GUILayout.Height(20) }))
            {
                showAddItem = !showAddItem;
            }
            GUILayout.EndHorizontal();
            if (showAddItem)
            {
                addItem();
            }

            /* 修改技能功能区 */
            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(0, 0, 0, 8) }, new GUILayoutOption[0]);
            GUILayout.Label("修改技能", new GUILayoutOption[0]);
            if (GUILayout.Button(showSkill ? "x" : "+", OpenButton, new GUILayoutOption[] { GUILayout.Width(20), GUILayout.Height(20) }))
            {
                showSkill = !showSkill;
            }
            GUILayout.EndHorizontal();
            if (showSkill)
            {
                skill();
            }

            /* 新范围拆除功能区 */
            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(0, 0, 0, 8) }, new GUILayoutOption[0]);
            GUILayout.Label("范围拆墙+铲地" + (rangeClear ? "[已开启]" : "[已关闭]"), new GUILayoutOption[0]);
            if (GUILayout.Button(showNewRangeClear ? "x" : "+", OpenButton, new GUILayoutOption[] { GUILayout.Width(20), GUILayout.Height(20) }))
            {
                showNewRangeClear = !showNewRangeClear;
            }
            GUILayout.EndHorizontal();
            if (showNewRangeClear)
            {
                rangebox();
            }

            /* 高级功能区 */
            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(0, 0, 0, 8) }, new GUILayoutOption[0]);
            GUILayout.Label("高级功能", new GUILayoutOption[0]);
            if (GUILayout.Button(showOther2 ? "x" : "+", OpenButton, new GUILayoutOption[] { GUILayout.Width(20), GUILayout.Height(20) }))
            {
                showOther2 = !showOther2;
            }
            GUILayout.EndHorizontal();
            if (showOther2)
            {
                other();
            }

            /* Buff功能区 */
            GUILayout.BeginHorizontal(new GUIStyle() { margin = new RectOffset(0, 0, 0, 8) }, new GUILayoutOption[0]);
            GUILayout.Label("Buff功能", new GUILayoutOption[0]);
            if (GUILayout.Button(showOther ? "x" : "+", OpenButton, new GUILayoutOption[] { GUILayout.Width(20), GUILayout.Height(20) }))
            {
                showOther = !showOther;
            }
            GUILayout.EndHorizontal();
            if (showOther)
            {
                buff();
            }

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        // 技能修改类
        void skill()
        {
            GUILayout.BeginVertical(new GUIStyle() { margin = new RectOffset(8, 8, 0, 0) }, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("30", buttonStyle, new GUILayoutOption[0]))
            {
                m.player.SetSkillLevel(SkillID.Cooking, 30);
                m.player.SetSkillLevel(SkillID.Crafting, 30);
                m.player.SetSkillLevel(SkillID.Fishing, 30);
                m.player.SetSkillLevel(SkillID.Gardening, 30);
                m.player.SetSkillLevel(SkillID.Melee, 30);
                m.player.SetSkillLevel(SkillID.Mining, 30);
                m.player.SetSkillLevel(SkillID.Range, 30);
                m.player.SetSkillLevel(SkillID.Running, 30);
                m.player.SetSkillLevel(SkillID.Vitality, 30);

            }
            bool btn5 = GUILayout.Button("50", buttonStyle, new GUILayoutOption[0]);
            if (btn5)
            {
                m.player.SetSkillLevel(SkillID.Cooking, 50);
                m.player.SetSkillLevel(SkillID.Crafting, 50);
                m.player.SetSkillLevel(SkillID.Fishing, 50);
                m.player.SetSkillLevel(SkillID.Gardening, 50);
                m.player.SetSkillLevel(SkillID.Melee, 50);
                m.player.SetSkillLevel(SkillID.Mining, 50);
                m.player.SetSkillLevel(SkillID.Range, 50);
                m.player.SetSkillLevel(SkillID.Running, 50);
                m.player.SetSkillLevel(SkillID.Vitality, 50);

            }
            bool btn6 = GUILayout.Button("80", buttonStyle, new GUILayoutOption[0]);
            if (btn6)
            {
                m.player.SetSkillLevel(SkillID.Cooking, 80);
                m.player.SetSkillLevel(SkillID.Crafting, 80);
                m.player.SetSkillLevel(SkillID.Fishing, 80);
                m.player.SetSkillLevel(SkillID.Gardening, 80);
                m.player.SetSkillLevel(SkillID.Melee, 80);
                m.player.SetSkillLevel(SkillID.Mining, 80);
                m.player.SetSkillLevel(SkillID.Range, 80);
                m.player.SetSkillLevel(SkillID.Running, 80);
                m.player.SetSkillLevel(SkillID.Vitality, 80);

            }
            bool btn1 = GUILayout.Button("100", buttonStyle, new GUILayoutOption[0]);
            if (btn1)
            {
                m.player.MaxOutAllSkills();
            }
            bool btn2 = GUILayout.Button("重置", buttonStyle, new GUILayoutOption[0]);
            if (btn2)
            {
                m.player.ResetAllSkills();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("挖掘+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Mining, 1);
            }
            if (GUILayout.Button("奔跑+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Running, 1);
            }
            if (GUILayout.Button("近战+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Melee, 1);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("活力+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Vitality, 1);
            }
            if (GUILayout.Button("制作+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Crafting, 1);
            }
            if (GUILayout.Button("远程+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Range, 1);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("园艺+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Gardening, 1);
            }
            if (GUILayout.Button("钓鱼+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Fishing, 1);
            }
            if (GUILayout.Button("烹饪+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Cooking, 1);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        // buff类功能
        void buff()
        {
            GUILayout.BeginVertical(new GUIStyle() { margin = new RectOffset(8, 8, 0, 0) }, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("无敌", new GUILayoutOption[0]);
            var btn3 = GUILayout.Button("开", buttonStyle, new GUILayoutOption[0]);
            var btn4 = GUILayout.Button("关", buttonStyle, new GUILayoutOption[0]);
            if (btn3)
            {
                m.player.SetInvincibility(true);
            }
            if (btn4)
            {
                m.player.SetInvincibility(false);
            }
            GUILayout.Label("护甲", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("ArmorIncrease", 200);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("ArmorIncrease", 200);

            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("生命上限", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("IncreasedMaxHealth", 1000);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("IncreasedMaxHealth", 1000);
            }
            GUILayout.Label("移速", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("MovementSpeedIncrease", 300);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("MovementSpeedIncrease", 300);

            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("近战伤害", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("MeleeDamageIncrease", 50);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("MeleeDamageIncrease", 50);

            }
            GUILayout.Label("远程伤害", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("RangeDamageIncrease", 50);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("RangeDamageIncrease", 50);

            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("橙色亮光", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("OrangeGlow", 5);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("BlueGlow", 0);
                delBuff("OrangeGlow", 0);

            }
            GUILayout.Label("挖掘速度", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("MiningSpeedIncrease", 30);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("MiningSpeedIncrease", 0);

            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("挖掘伤害", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("MiningIncrease", 1000);
            }
            if (GUILayout.Button("KW", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("MiningIncrease", 10000000);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("MiningIncrease", 0);
            }
            GUILayout.Label("暴击", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("CriticalHitChanceFromShot", 30);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("CriticalHitChanceFromShot", 0);

            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("攻速", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("MeleeAttackSpeedIncrease", 30);
                addBuff("RangeAttackSpeedIncrease", 30);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("MeleeAttackSpeedIncrease", 0);
                delBuff("RangeAttackSpeedIncrease", 0);

            }
            GUILayout.Label("金色植物", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("ChanceToGainRarePlant", 200);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("ChanceToGainRarePlant", 0);

            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("稀有食物", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("ChanceForExtraCookedFoodToBeRare", 200);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("ChanceForExtraCookedFoodToBeRare", 0);

            }
            GUILayout.Label("日炎圣盾", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("AuraApplyBurning", 500);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("AuraApplyBurning", 0);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("拾取范围", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("IncreasedPickUpRadius", 100);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("IncreasedPickUpRadius", 0);

            }
            GUILayout.Label("行船速度", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("IncreasedBoatSpeed", 100);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("IncreasedBoatSpeed", 0);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("远程龙卷风", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("ChanceOnRangeHitToSpawnOctopusBossProjectile", 100);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("ChanceOnRangeHitToSpawnOctopusBossProjectile", 0);

            }
            GUILayout.Label("召唤鬼魂", new GUILayoutOption[0]);
            if (GUILayout.Button("+", buttonStyle, new GUILayoutOption[0]))
            {
                addBuff("ChanceOnKillToSummonGhost", 100);
            }
            if (GUILayout.Button("移除", buttonStyle, new GUILayoutOption[0]))
            {
                delBuff("ChanceOnKillToSummonGhost", 0);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void addItem()
        {
            GUILayout.BeginVertical(new GUIStyle() { margin = new RectOffset(8, 8, 0, 0) }, new GUILayoutOption[0]);
            amount = GUILayout.HorizontalSlider(amount, 1, 999, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            var IDText = "1";
            if (itemID == "" || itemID == null)
            {
                itemID = "1";
            }
            else
            {
                IDText = Regex.Replace(itemID, @"[^0-9]", "");
                if (IDText == "") IDText = "1";
            }

            GUILayout.Label("物品ID:" + Int32.Parse(IDText), new GUILayoutOption[0]);
            GUILayout.Label("数量:" + (int)amount, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            var setid = GUILayout.Button("粘贴ID", buttonStyle, new GUILayoutOption[0]);
            if (setid)
            {
                if (UnityEngine.GUIUtility.systemCopyBuffer != null && UnityEngine.GUIUtility.systemCopyBuffer != "" && IsNumeric(UnityEngine.GUIUtility.systemCopyBuffer))
                {
                    LoggerInstance.Msg("剪切板内容:" + UnityEngine.GUIUtility.systemCopyBuffer);
                    itemID = UnityEngine.GUIUtility.systemCopyBuffer;
                }

            }
            bool add = GUILayout.Button("添加", buttonStyle, new GUILayoutOption[0]);
            bool add2 = GUILayout.Button("召唤生物", buttonStyle, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.Label("武器装备等不可叠加的物品 数量尽量为1 否则会卡顿闪退", new GUILayoutOption[0]);
            if (add)
            {
                if (isAdminPlayer())
                {
                    m.player.playerCommandSystem.CreateAndDropEntity((ObjectID)Enum.Parse(typeof(ObjectID), itemID), m.player.RenderPosition + new Vector3(0, 0, 0), (int)amount);
                }
            }
            if (add2)
            {
                if (isAdminPlayer())
                {
                    m.player.playerCommandSystem.CreateEntity((ObjectID)Enum.Parse(typeof(ObjectID), itemID), m.player.RenderPosition + new Vector3(2f, 0, 0));
                }
            }
            GUILayout.EndVertical();
        }

        void other()
        {
            GUILayout.BeginVertical(new GUIStyle() { margin = new RectOffset(8, 8, 0, 0) }, new GUILayoutOption[0]);
            GUILayout.Label("雷霆光束 [该功能仅限管理员玩家使用]", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("开启", buttonStyle, new GUILayoutOption[0]))
            {
                if (ThunderBeam == false)
                {
                    ThunderBeam = true;
                    m._textManager.SpawnCoolText("已获得天空泰坦真传", new Vector3(0, 2, 0), Color.white, TextManager.FontFace.button, 1f, 1, 5, 0.8f, 0.8f);
                }
            }
            if (GUILayout.Button("关闭", buttonStyle, new GUILayoutOption[0]))
            {
                if (ThunderBeam == true)
                {
                    ThunderBeam = false;
                    m._textManager.SpawnCoolText("已失去雷霆光束", new Vector3(0, 2, 0), Color.red, TextManager.FontFace.button, 1f, 1, 5, 0.8f, 0.8f);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("F键普通光束 V键超级光束", new GUILayoutOption[0]);

            GUILayout.Label("地图传送 (鼠标中键点地图)", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("开启", buttonStyle, new GUILayoutOption[0]))
            {
                if (MapPing == false)
                {
                    MapPing = true;
                    m._textManager.SpawnCoolText("地图传送已开启", new Vector3(0, 2, 0), Color.white, TextManager.FontFace.button, 1f, 1, 5, 0.8f, 0.8f);
                }
            }
            if (GUILayout.Button("关闭", buttonStyle, new GUILayoutOption[0]))
            {
                if (MapPing == true)
                {
                    MapPing = false;
                    m._textManager.SpawnCoolText("地图传送已关闭", new Vector3(0, 2, 0), Color.red, TextManager.FontFace.button, 1f, 1, 5, 0.8f, 0.8f);
                }
            }
            GUILayout.EndHorizontal();

            /*GUILayout.Label("秒挖地皮 开启后需要重新进存档", new GUILayoutOption[0]);
            if (GUILayout.Button("秒挖地皮", buttonStyle, new GUILayoutOption[0]))
            {
                String[] set = { "GroundDirtBlockEntity", "GroundStoneBlockEntity", "GroundHiveBlockEntity", "GroundGrassBlockEntity", "GroundMoldBlockEntity", "GroundBeachSandBlockEntity", "GroundClayBlockEntity", "GroundSandBlockEntity", "GroundTurfBlockEntity" };

                HealthCDAuthoring[] objs = Resources.FindObjectsOfTypeAll<HealthCDAuthoring>();

                foreach (HealthCDAuthoring obj in objs)
                {
                    if (Array.IndexOf(set, obj.name) != -1)
                    {
                        obj.maxHealth = 0;
                        if (Array.IndexOf(set, obj.name) == set.Length) break;
                    }
                }

            }*/

            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("秒挖地皮" + (oneTapClearTile ? "[已开启]" : "[已关闭]"), new GUILayoutOption[0]);
            if (GUILayout.Button(oneTapClearTile ? "关闭" : "开启", buttonStyle, new GUILayoutOption[0]))
            {
                oneTapClearTile = !oneTapClearTile;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("吸附掉落物" + (pullLootToPlayer ? "[已开启]" : "[已关闭]"), new GUILayoutOption[0]);
            if (GUILayout.Button(pullLootToPlayer ? "关闭" : "开启", buttonStyle, new GUILayoutOption[0]))
            {
                pullLootToPlayer = !pullLootToPlayer;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void rangebox()
        {
            GUILayout.BeginVertical(new GUIStyle() { margin = new RectOffset(8, 8, 0, 0) }, new GUILayoutOption[0]);
            GUILayout.Label("范围:" + clearRange + " [该功能仅限管理员玩家使用]", new GUILayoutOption[0]);
            clearRange = (int)GUILayout.HorizontalSlider(clearRange, 2, 100, new GUILayoutOption[0]);
            if (GUILayout.Button(rangeClear ? "关闭" : "开启", buttonStyle, new GUILayoutOption[0]))
            {
                rangeClear = !rangeClear;
            }
            GUILayout.EndVertical();
        }

        // 增加Buff
        void addBuff(string name, int value)
        {
            ConditionID Cid = (ConditionID)Enum.Parse(typeof(ConditionID), name);
            var Buff = EntityUtility.GetConditionValue(Cid, m.player.entity, m.player.world);
            LoggerInstance.Msg("[增加Buff(" + name + ")数值]当前Buff值: " + Buff + " 增加的数值: " + value);
            Buff += value;
            m.player.playerCommandSystem.AddOrRefreshCondition(m.player.entity, Cid, Buff, 0);
        }

        // 减少Buff
        void delBuff(string name, int value)
        {
            LoggerInstance.Msg("[移除Buff(" + name + ")]");
            ConditionID Cid = (ConditionID)Enum.Parse(typeof(ConditionID), name);
            m.player.playerCommandSystem.RemoveCondition(m.player.entity, Cid);
        }

        void addSkill(SkillID ID, int value)
        {
            Skills skills = m._saveManager.GetSkills();
            int skv = m._saveManager.GetSkillValue(ID);
            int level = SkillData.GetLevelFromSkill(ID, skv);
            m.player.SetSkillLevel(ID, level + value);
        }

        void superThunderBeam()
        {
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, 0.1f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, 0.2f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, 0.3f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, -0.1f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, -0.2f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, -0.3f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.9f, 0, 0.5f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.8f, 0, 0.6f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.7f, 0, 0.7f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0.8f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.5f, 0, 0.9f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.4f, 0, 1f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.4f, 0, -1f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.5f, 0, -0.9f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.6f, 0, -0.8f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.7f, 0, -0.7f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.8f, 0, -0.6f), m.player.entity,114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.9f, 0, -0.5f), m.player.entity,114514);

            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, 0.1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, 0.2f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, 0.3f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, -0.1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, -0.2f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, -0.3f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.9f, 0, 0.5f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.8f, 0, 0.6f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.7f, 0, 0.7f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.6f, 0, 0.8f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.5f, 0, 0.9f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.4f, 0, 1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.4f, 0, -1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.5f, 0, -0.9f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.6f, 0, -0.8f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.7f, 0, -0.7f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.8f, 0, -0.6f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.9f, 0, -0.5f), m.player.entity, 114514);

            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.1f, 0, -1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.2f, 0, -1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.3f, 0, -1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.1f, 0, -1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.2f, 0, -1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.3f, 0, -1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.5f, 0, -0.9f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.6f, 0, -0.8f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.7f, 0, -0.7f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.8f, 0, -0.6f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.9f, 0, -0.5f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, -0.4f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, -0.4f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.9f, 0, -0.5f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.8f, 0, -0.6f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.7f, 0, -0.7f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.6f, 0, -0.8f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.5f, 0, -0.9f), m.player.entity, 114514);

            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.1f, 0, 1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.2f, 0, 1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.3f, 0, 1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.1f, 0, 1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.2f, 0, 1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.3f, 0, 1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.5f, 0, 0.9f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0.8f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.7f, 0, 0.7f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.8f, 0, 0.6f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.9f, 0, 0.5f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, 0.4f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, 0.4f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.9f, 0, 0.5f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.8f, 0, 0.6f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.7f, 0, 0.7f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.6f, 0, 0.8f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.5f, 0, 0.9f), m.player.entity, 114514);

            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1, 0, 0), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, 0), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0, 0, 1f), m.player.entity, 114514);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0, 0, -1f), m.player.entity, 114514);

        }

        static void rangeClearTile(int range, Unity.Mathematics.int2 position, bool clearGround = false)
        {
            var m = GameObject.FindObjectOfType<Manager>();
            if (range <= 0 || !isAdminPlayer()) return;
            var playerEntity = m.player.entity;
            var pos = new Unity.Mathematics.int2(0, 0);

            /*
             判断方向
             x = 1,y = 0时向右
             x = -1,y = 0时向左
             x =0,y = -1时向下
             x =0,y = 1时向上
             x = 1,y = 1 时右上
             x = 1,y = -1 时右下
             x = -1,y = 1 时左上
             x = -1,y = -1 时左下
             */

            // 向左右的时候
            if (position.y == 0)
            {
                for (int y = 0; y < range; y++)
                {
                    pos.x = position.x < 0 ? position.x - y : position.x + y;

                    for (int x = 0; x < range; x++)
                    {
                        pos.y = position.y + x;
                        // MelonLogger.Msg(pos.x);
                        if (pos.x == 0 || pos.y == 0)
                        {
                            m.player.playerCommandSystem.CreateTileDamage(pos, 1000000000, playerEntity, false, true);
                        }
                        else
                        {
                            m.player.playerCommandSystem.CreateTileDamage(pos, 1000000000, playerEntity, clearGround, true);
                        }

                    }

                    for (int x = 0; x < range; x++)
                    {
                        pos.y = position.y - x;
                        // MelonLogger.Msg(pos.x);
                        if (pos.x == 0 || pos.y == 0)
                        {
                            continue;
                        }
                        else
                        {
                            m.player.playerCommandSystem.CreateTileDamage(pos, 1000000000, playerEntity, clearGround, true);
                        }

                    }
                }
            }
            // 向上下的时候
            else
            {
                for (int y = 0; y < range; y++)
                {
                    pos.y = position.y < 0 ? position.y - y : position.y + y;

                    for (int x = 0; x < range; x++)
                    {
                        pos.x = position.x + x;
                        // MelonLogger.Msg(pos.x);
                        if (pos.x == 0 || pos.y == 0)
                        {
                            m.player.playerCommandSystem.CreateTileDamage(pos, 1000000000, playerEntity, false, true);
                        }
                        else
                        {
                            m.player.playerCommandSystem.CreateTileDamage(pos, 1000000000, playerEntity, clearGround, true);
                        }

                    }

                    for (int x = 0; x < range; x++)
                    {
                        pos.x = position.x - x;
                        // MelonLogger.Msg(pos.x);
                        if (pos.x == 0 || pos.y == 0)
                        {
                            continue;
                        }
                        else
                        {
                            m.player.playerCommandSystem.CreateTileDamage(pos, 1000000000, playerEntity, clearGround, true);
                        }

                    }
                }
            }


        }

        static bool isAdminPlayer()
        {
            /*
             *  有问题adminlist始终只有自己的id
             *
            var m = GameObject.FindObjectOfType<Manager>();
            var AdminList = m._networkingManager.adminList;
            foreach(PlayerAdminEntry admin in AdminList.adminList)
            {
                if(admin.steamId == steamId)
                {
                    return true;
                }
            }
            return false;*/
            var m = GameObject.FindObjectOfType<Manager>();
            if(m.player.adminPrivileges <= 0)
            {
                m._textManager.SpawnCoolText("非管理员玩家无法使用", new Vector3(0, 2, 0), Color.red, TextManager.FontFace.button, 1f, 1, 5, 0.8f, 0.8f);
                return false;
            }

            return true;

        }

        static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        [HarmonyPrefix, HarmonyPatch(typeof(MapUI), "Ping")]
        public static bool FlxsnxMod_MapUI_MapPing_Patch(PlayerController pc, Vector3 worldPos)
        {
            if (MapPing)
            {
                var m = GameObject.FindObjectOfType<Manager>();
                if (m.player.playerIndex == pc.playerIndex)
                {
                    m.player.SetPlayerPosition(worldPos);
                    return false;
                }
                else
                {
                    return true;
                }

            }
            else
            {
                return true;
            }
        }

        // 修复鼠标闪烁问题
        [HarmonyPrefix, HarmonyPatch(typeof(Cursor), "set_visible")]
        public static bool FlxsnxMod_Cursor_set_visible_Patch(bool value)
        {
            if (value == false && showMenu == true)
            {
                return false;
            }
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Il2CppPlayerCommand.ClientSystem), "CreateTileDamage")]
        public static bool FlxsnxMod_ClientSystem_CreateTileDamage_Patch(Unity.Mathematics.int2 position, ref int damage, ref Unity.Entities.Entity pullAnyLootTowardsPlayerEntity, ref bool pullAnyLootToPlayer, bool canDamageGround = false)
        {
            if (rangeClear && damage != 1000000000)
            {
                rangeClearTile((int)clearRange, position, canDamageGround);
                return false;
            }
            else
            {
                if (oneTapClearTile)
                {
                    damage = 19999;
                }

                if (pullLootToPlayer)
                {
                    var m = GameObject.FindObjectOfType<Manager>();
                    pullAnyLootTowardsPlayerEntity = m.player.entity;
                    pullAnyLootToPlayer = true;
                }
                return true;
            }
        }

        /*[HarmonyPostfix, HarmonyPatch(typeof(SteamNetworking), "Initialize")]
        public static void FlxsnxMod_SteamPlatform_Init(SteamNetworking __instance)
        {
            if (steamId == 0)
            {
                steamId = SteamClient.SteamId.Value;
                MelonLogger.Msg("SteamId:" + steamId);
            }
        }*/
    }
}
