
#region Apply To PlayerMobile.cs
/** The Following region must be added to the player class.
 *   #region Language

        public enum PlayerLanguage
        {  
           Invalid,
           Common,
           Ancient,
           Tribal,
           Pagan,
           Glyph
        }

        public int[] LevelofUnderstanding = new int[]
        {
          0, 100, 0, 0, 0, 0
        };

        private PlayerLanguage speakingLanguage = PlayerLanguage.Common;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerLanguage CurrentLanguage
        {
            get { return speakingLanguage; }
            set { speakingLanguage = value; }
        }

        #endregion
 * 
 * !! The Following Must Be Serialized and Deserialized in the player class. !!
 * 
 *      writer.Write((int)speakingLanguage);
        writer.Write(LevelofUnderstanding.Length);

            for (int i = 0; i < LevelofUnderstanding.Length; i++)
            {
                writer.Write(LevelofUnderstanding[i]);
            }
 * 
 *          if (version > = 2)
            {
                speakingLanguage = (PlayerLanguage)reader.ReadInt();

                int count = reader.ReadInt();

                LevelofUnderstanding = new int[count];

                for (int i = 0; i < count; i++)
                {
                    LevelofUnderstanding[i] = reader.ReadInt();
                }
            }
 * */
#endregion

using System;
using System.Text;
using Server.Gumps;
using Server.Network;
using System.Collections;
using Server.Mobiles;
using Server.Commands;

namespace Server.Language
{
    public class Language
    {
        public static void Initialize()
        {
            EventSink.Speech += new SpeechEventHandler(EventSink_Speech);
        }

        public static void EventSink_Speech(SpeechEventArgs args)
        {
            if (args.Mobile is PlayerMobile)
            {
                MessageType mt = args.Type;
                PlayerMobile player = args.Mobile as PlayerMobile;
                string playerSpeech = args.Speech;
                int speechRange;

                int[] FontHue = new int[] { 0, 0, 1175, 1157, 1155, 1152 };

                if ((player.AccessLevel > AccessLevel.PlayerMobile) && player.Hidden)
                    return;

                if ((!player.Alive) || String.IsNullOrEmpty(playerSpeech))
                    return;

                if (player.CurrentLanguage == PlayerMobile.PlayerLanguage.Common)
                    return;

                if (args.Type == MessageType.Emote)
                {
                    player.Emote(playerSpeech);
                    return;
                }

                switch (args.Type)
                {
                    case MessageType.Yell:
                        speechRange = 20;
                        break;

                    case MessageType.Regular:
                        speechRange = 10;
                        break;

                    case MessageType.Whisper:
                        speechRange = 1;
                        break;

                    default:
                        speechRange = 10;
                        break;
                }

                ArrayList list = new ArrayList();

                foreach (Mobile m in player.Map.GetMobilesInRange(player.Location, speechRange))
                {
                    list.Add(m);
                }

                for (int x = 0; x < list.Count; x++)
                {
                    Mobile m = list[x] as Mobile;
                    if (m.PlayerMobile)
                    {
                        PlayerMobile listener = m as PlayerMobile;

                        if (listener.LevelofUnderstanding[(int)player.CurrentLanguage] < Utility.RandomMinMax(90, 100))
                        {
                            args.Blocked = true;
                            listener.Send(new AsciiMessage(player.Serial, -1, MessageType.Regular, FontHue[(int)player.CurrentLanguage], 8, "Runes", playerSpeech.ToUpper()));
                            player.RevealingAction();

                            if (Utility.RandomDouble() <= 0.10)
                            {
                                listener.LevelofUnderstanding[(int)player.CurrentLanguage] += 1;
                            }
                        }
                    }
                }
            }
        }
    }

    public class BookOfLanguage : Item
    {
        public enum Language
        {
            Invalid,
            Common,
            Ancient,
            Tribal,
            Pagan,
            Glyph
        }

        private Language WritLanguage;

        [CommandProperty(AccessLevel.GameMaster)]
        public Language BookType
        {
            get { return WritLanguage; }
            set { WritLanguage = value; }
        }

        private int nameRef = Utility.RandomMinMax(1, 5);

        public string GenerateBookName()
        {
            switch (nameRef)
            {
                case 1:
                    return "Articles on the " + BookType.ToString() + " Language";
                case 2:
                    return BookType.ToString() + " Syntax and Grammar";
                case 3:
                    return BookType.ToString() + " Language Structure";
                case 4:
                    return "Conjugating Verbs in " + BookType.ToString();
                case 5:
                    return "A History of the " + BookType.ToString() + " Language";

                default: return "A book on" + BookType.ToString() + " dialects";
            }
        }

        public override string DefaultName
        {
            get { return GenerateBookName(); }
        }

        [Constructable]
        public BookOfLanguage()
            : base(0xFBE)
        {
            Weight = 1.0;
        }

        public BookOfLanguage(Serial serial)
            : base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            writer.Write((int)WritLanguage);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            WritLanguage = (Language)reader.ReadInt();

        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }

            else if (pm.LevelofUnderstanding[(int)BookType] >= 100)
            {
                pm.SendMessage("You have already learned all you can about this language.");
            }
            else
            {
                pm.LevelofUnderstanding[(int)BookType] += Utility.RandomMinMax(1, 3);
                pm.SendMessage("Your knowledge of the {0} language has increased.", BookType.ToString());

                if (pm.LevelofUnderstanding[(int)BookType] > 100)
                    pm.LevelofUnderstanding[(int)BookType] = 100;

                Delete();
            }
        }
    }

    public class SpeakCommand
    {
        [CommandAttribute("Speak", AccessLevel.PlayerMobile)]
        public static void SayCommand_OnCommand(CommandEventArgs args)
        {
            string text = args.ArgString.Trim();
            PlayerMobile p = args.Mobile as PlayerMobile;
            PlayerMobile.PlayerLanguage newLanguage;
            Enum.TryParse<PlayerMobile.PlayerLanguage>(text, true, out newLanguage);

            Console.WriteLine(newLanguage);
            Console.WriteLine((int)newLanguage);
            Console.WriteLine(p.LevelofUnderstanding[(int)newLanguage]);

            switch (newLanguage)
            {
                case PlayerMobile.PlayerLanguage.Common:
                    {
                        if (p.LevelofUnderstanding[(int)newLanguage] == 100)
                        {
                            p.CurrentLanguage = PlayerMobile.PlayerLanguage.Common;
                            p.SendMessage("You're now speaking a Common language.");
                        }
                        else
                        {
                            p.SendMessage("You do not posess the level of understanding needed to speak that language.");
                        }

                        break;
                    }

                case PlayerMobile.PlayerLanguage.Ancient:
                    {
                        if (p.LevelofUnderstanding[(int)newLanguage] == 100)
                        {
                            p.CurrentLanguage = PlayerMobile.PlayerLanguage.Ancient;
                            p.SendMessage("You're now speaking in Ancient tongues.");
                        }
                        else
                        {
                            p.SendMessage("You do not posess the level of understanding needed to speak that language.");
                        }

                        break;
                    }

                case PlayerMobile.PlayerLanguage.Tribal:
                    {
                        if (p.LevelofUnderstanding[(int)newLanguage] == 100)
                        {
                            p.CurrentLanguage = PlayerMobile.PlayerLanguage.Tribal;
                            p.SendMessage("You're now speaking in a Tribal dialect.");
                        }
                        else
                        {
                            p.SendMessage("You do not posess the level of understanding needed to speak that language.");
                        }

                        break;
                    }

                case PlayerMobile.PlayerLanguage.Pagan:
                    {
                        if (p.LevelofUnderstanding[(int)newLanguage] == 100)
                        {
                            p.CurrentLanguage = PlayerMobile.PlayerLanguage.Pagan;
                            p.SendMessage("You are now using a Pagan vernacular.");
                        }
                        else
                        {
                            p.SendMessage("You do not posess the level of understanding needed to speak that language.");
                        }

                        break;
                    }

                case PlayerMobile.PlayerLanguage.Glyph:
                    {
                        if (p.LevelofUnderstanding[(int)newLanguage] == 100)
                        {
                            p.CurrentLanguage = PlayerMobile.PlayerLanguage.Tribal;
                            p.SendMessage("You're now speaking in phonetic Glyph.");
                        }
                        else
                        {
                            p.SendMessage("You do not posess the level of understanding needed to speak that language.");
                        }

                        break;
                    }

            }
        }
    }
}