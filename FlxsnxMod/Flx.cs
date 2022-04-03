using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Il2CppSystem.Text.RegularExpressions;
using MelonLoader;
using UnityEngine;
using HarmonyLib;

namespace FlxsnxMod
{
    [HarmonyPatch(typeof(MapUI), "Ping")]
    public class Flx: MelonMod
    {
        private bool showMenu = false;
        public Manager m;
        public ChatWindow chat;
        public PlayerInput pinput;
        public string itemID = "1";
        public float amount = 1;
        public const string version = "1.1.0";
        Vector2 vSbarValue;
        public bool ThunderBeam = false;
        public AssetBundle Flxasset;
        public GUIStyle windowStyle;
        public GUIStyle buttonStyle;
        public GUIStyle vSbarStyle;
        public Color32 ModColor = new Color32(165, 151, 115, 255);
        public Rect WindowRect = new Rect(0, (Screen.height - 420) / 2 + 30, 320, 420);
        public GUIStyle ModIcon;
        public static bool MapPing = false;
        public override void OnApplicationStart()
        {
            Flxasset = AssetBundle.LoadFromMemory(FlxsnxMod.Properties.Res.mod);
            buttonStyle = new GUIStyle()
            {
                normal = new GUIStyleState  // 正常样式
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("button")
                },
                active = new GUIStyleState  // 点击样式
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
            ModIcon = new GUIStyle()
            {
                normal = new GUIStyleState  // 正常样式
                {
                    textColor = Color.white,
                    background = Flxasset.LoadAsset<Texture2D>("avatar")
                },
                fixedHeight = 28,
                fixedWidth = 28,
                margin = new RectOffset(0,0,6,6)
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
                    
                    if (m.player.playerIndex == 1)
                    {
                        m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), m.player.aimDirection, m.player.entity);
                    }

                }
            }

            if (showMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                if (ThunderBeam)
                {
                    if (m.player.playerIndex == 1)
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
                        if(code[0] == "/item" && IsNumeric(code[2]))
                        {
                            if (m.player.playerIndex == 1)
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
                            else
                            {
                                m._textManager.SpawnCoolText("非房主无法使用", new Vector3(0, 2, 0), Color.red, TextManager.FontFace.button, 1f, 1, 5, 0.8f, 0.8f);
                            }
                        }

                        if(code[0] == "/buff" && code.Length >= 3 && code[2] != null && code[2] != "")
                        {
                            if(code[1] == "add" && code.Length == 4 && code[3] != null && code[3] != "" && IsNumeric(code[3]))
                            {
                                addBuff(code[2], (int)Int32.Parse(code[3]));
                            }else if(code[1] == "del")
                            {
                                delBuff(code[2], 0);
                            }
                        }
                    }
                }
            }
        }

        public override void OnGUI()
        {
            GUILayout.BeginArea(new Rect(5, 5, 120, 30));
            var oldbackground = GUI.backgroundColor;
            GUI.backgroundColor = new Color32(48, 48, 48, 255);
            GUI.skin.window = new GUIStyle()
            {
                normal = new GUIStyleState  // 正常样式
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

            bool flag = GUILayout.Button("FlxSNXMod", buttonStyle, new GUILayoutOption[]
            {
                GUILayout.Height(30)
            });

            if (flag)
            {
                showMenu = !showMenu;
            }
            
            GUILayout.EndArea();
            if (showMenu)
            {
                GUI.backgroundColor = oldbackground;
                WindowRect = GUI.Window(20029933, WindowRect, (GUI.WindowFunction)menuWindow,"");
                /*if (showSkillWindow)
                {
                    GUI.Window(20029934, new Rect(330, (Screen.height - 420) / 2 + 50, 200, 120), (GUI.WindowFunction)SkillWindow, "", windowStyle);
                }*/
            }
        }
        public void menuWindow(int winId)
        {
            if (m == null)
            {
                m = GameObject.FindObjectOfType<Manager>();
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Button("", ModIcon, new GUILayoutOption[0]);
            GUILayout.Label("FlxSNXMod Version " + version, new GUIStyle()
            {
                normal = new GUIStyleState
                {
                    textColor = Color.white,

                },
                margin = new RectOffset(6, 0, 6, 6),
                padding = new RectOffset(0, 0, 8, 0),
                fixedHeight = 28
            }, new GUILayoutOption[0]); ;
            GUILayout.EndHorizontal();
            vSbarValue = GUILayout.BeginScrollView(vSbarValue, false, true, new GUILayoutOption[0]);
            GUI.backgroundColor = ModColor;
            GUILayout.Label("添加物品", new GUILayoutOption[0]);
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

            // LoggerInstance.Msg("IDText" + IDText);
            GUILayout.Label("物品ID:" + Int32.Parse(IDText), new GUILayoutOption[0]);
            GUILayout.Label("数量:" + (int)amount, new GUILayoutOption[0]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            var setid = GUILayout.Button("粘贴ID", buttonStyle, new GUILayoutOption[0]);
            if (setid)
            {
                if (UnityEngine.GUIUtility.systemCopyBuffer != null || UnityEngine.GUIUtility.systemCopyBuffer != "")
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
                if (m.player.playerIndex == 1)
                {
                    m.player.playerCommandSystem.CreateAndDropEntity((ObjectID)Enum.Parse(typeof(ObjectID), itemID), m.player.RenderPosition + new Vector3(0, 2f, 0), (int)amount);
                }
            }
            if (add2)
            {
                if (m.player.playerIndex == 1)
                {
                    m.player.playerCommandSystem.CreateEntity((ObjectID)Enum.Parse(typeof(ObjectID), itemID), m.player.RenderPosition + new Vector3(0, 2f, 0));
                }
            }
            GUILayout.Label("技能修改", new GUILayoutOption[0]);
            /*if (GUI.Button(new Rect(70, 2, 80, 20), "详细修改", buttonStyle))
            {
                showSkillWindow = !showSkillWindow;
            }*/
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
            GUILayout.Label("雷霆光束", new GUILayoutOption[0]);
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
            GUILayout.Label("由于部分人使用Mod恶意炸房,非房主时部分功能将会被限制", new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            bool link = GUILayout.Button("作者B站主页", buttonStyle, new GUILayoutOption[0]);
            if (link)
            {
                m._platformManager.OpenLink("https://space.bilibili.com/36224682");
            }
            bool idtable = GUILayout.Button("物品ID表", buttonStyle, new GUILayoutOption[0]);
            if (idtable)
            {
                m._platformManager.OpenLink("https://docs.qq.com/sheet/DRUdjeW53QUN5T3B1");
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("其他功能", new GUILayoutOption[0]);
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
            if (GUILayout.Button("一键清空背包", buttonStyle, new GUILayoutOption[0]))
            {
                var ih = m.player.playerInventoryHandler;
                for (int i = 10; i < 45; i++)
                {
                    var info = ih.GetObjectData(i);
                    ih.DestroyObject(i, info.objectID);
                }
            }
            if (GUILayout.Button("背包物品999", buttonStyle, new GUILayoutOption[0]))
            {
                var ih = m.player.playerInventoryHandler;
                for (int i = 10; i < 45; i++)
                {
                    var info = ih.GetObjectData(i);
                    if (info.objectID != ObjectID.None)
                    {
                        ih.SetAmount(i, info.objectID, 999);
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

       /* public void SkillWindow(int winId)
        {
            if (m == null)
            {
                m = GameObject.FindObjectOfType<Manager>();
            }
            GUILayout.Label("技能修改", new GUILayoutOption[0]);
            GUI.backgroundColor = ModColor;
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("挖掘+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Mining,1);
            }
            if (GUILayout.Button("奔跑+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Running,1);
            }
            if (GUILayout.Button("近战+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Melee,1);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("活力+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Vitality,1);
            }
            if (GUILayout.Button("制作+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Crafting,1);
            }
            if (GUILayout.Button("远程+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Range,1);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (GUILayout.Button("园艺+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Gardening,1);
            }
            if (GUILayout.Button("钓鱼+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Fishing,1);
            }
            if (GUILayout.Button("烹饪+", buttonStyle, new GUILayoutOption[0]))
            {
                addSkill(SkillID.Cooking,1);
            }
            GUILayout.EndHorizontal();
        }*/

        // 增加Buff
        public void addBuff(string name,int value)
        {
            ConditionID Cid = (ConditionID)Enum.Parse(typeof(ConditionID),name);
            var Buff = EntityUtility.GetConditionValue(Cid, m.player.entity, m.player.world);
            LoggerInstance.Msg("[增加Buff(" + name + ")数值]当前Buff值: " + Buff + " 增加的数值: " + value);
            Buff += value;
            m.player.playerCommandSystem.AddOrRefreshCondition(m.player.entity, Cid, Buff, 0);
        }

        // 减少Buff
        public void delBuff(string name, int value)
        {
            /* 
             * 有bug 改成移除buff了
             * ConditionID Cid = (ConditionID)Enum.Parse(typeof(ConditionID), name);
             var Buff = EntityUtility.GetConditionValue(Cid, m.player.entity, m.player.world);
             LoggerInstance.Msg("[减少Buff("+name+")数值]当前Buff值: " + Buff+" 减少的数值: "+value);
             Buff -= value;
             if (Buff <= 0)
             {
                 m.player.playerCommandSystem.RemoveCondition(m.player.entity, Cid);
             }
             else
             {
                 m.player.playerCommandSystem.AddOrRefreshCondition(m.player.entity, Cid, Buff, 0);
             }*/
            LoggerInstance.Msg("[移除Buff(" + name + ")]");
            ConditionID Cid = (ConditionID)Enum.Parse(typeof(ConditionID), name);
            m.player.playerCommandSystem.RemoveCondition(m.player.entity, Cid);
        }

        public void addSkill(SkillID ID,int value)
        {
            Skills skills = m._saveManager.GetSkills();
            int skv = m._saveManager.GetSkillValue(ID);
            int level = SkillData.GetLevelFromSkill(ID, skv);
            m.player.SetSkillLevel(ID, level+value);
        }

        public void superThunderBeam()
        {
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, 0.1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, 0.2f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, 0.3f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, -0.1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, -0.2f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, -0.3f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.9f, 0, 0.5f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.8f, 0, 0.6f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.7f, 0, 0.7f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0.8f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.5f, 0, 0.9f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.4f, 0, 1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.4f, 0, -1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.5f, 0, -0.9f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.6f, 0, -0.8f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.7f, 0, -0.7f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.8f, 0, -0.6f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.9f, 0, -0.5f), m.player.entity);

            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, 0.1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, 0.2f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, 0.3f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, -0.1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, -0.2f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, -0.3f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.9f, 0, 0.5f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.8f, 0, 0.6f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.7f, 0, 0.7f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.6f, 0, 0.8f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.5f, 0, 0.9f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.4f, 0, 1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.4f, 0, -1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.5f, 0, -0.9f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.6f, 0, -0.8f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.7f, 0, -0.7f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.8f, 0, -0.6f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.9f, 0, -0.5f), m.player.entity);

            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.1f, 0, -1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.2f, 0, -1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.3f, 0, -1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.1f, 0, -1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.2f, 0, -1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.3f, 0, -1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.5f, 0, -0.9f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.6f, 0, -0.8f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.7f, 0, -0.7f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.8f, 0, -0.6f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.9f, 0, -0.5f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, -0.4f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, -0.4f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.9f, 0, -0.5f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.8f, 0, -0.6f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.7f, 0, -0.7f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.6f, 0, -0.8f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.5f, 0, -0.9f), m.player.entity);

            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.1f, 0, 1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.2f, 0, 1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.3f, 0, 1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.1f, 0, 1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.2f, 0, 1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.3f, 0, 1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.5f, 0, 0.9f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0.8f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.7f, 0, 0.7f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.8f, 0, 0.6f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0.9f, 0, 0.5f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, 0.4f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1f, 0, 0.4f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.9f, 0, 0.5f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.8f, 0, 0.6f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.7f, 0, 0.7f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.6f, 0, 0.8f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-0.5f, 0, 0.9f), m.player.entity);

            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(-1, 0, 0), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(1f, 0, 0), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0, 0, 1f), m.player.entity);
            m.player.playerCommandSystem.SpawnThunderBeam(new Vector3(0, 0, 0), new Vector3(0, 0, -1f), m.player.entity);

        }

        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        [HarmonyPrefix, HarmonyPatch(typeof(MapUI), "Ping")]
        public static bool PlantMod_MapUI_MapPing_Patch(PlayerController pc, Vector3 worldPos)
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
    }
}
