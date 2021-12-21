/*  Scripted By
 *        █▀▀▀ █▀▀ █▀▀█ █▀▀ █▀▀ ▀█ █▀ █▀▀█ █▀▀█ 
 *        █▀▀▀ ▀▀█ █  █ █   █▀   █▄█  █▄▄█ █  █ 
 *        █▄▄▄ ▀▀▀ █▀▀▀ ▀▀▀ ▀▀▀   ▀   ▀  ▀ █  █
 *                 █
 *  
 *     █ █ █▀█ ▄▀█ █▀▀ █▀▀ █▀▀ █▀ █▀   █▀▀ █▀█ █▀█▀█
 *     █▄█ █▄█ █▀█ █▄▄ █▄▄ ██▄ ▄█ ▄█ ▄ █▄▄ █▄█ █ ▀ █
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.IO;
using System.Reflection;

namespace Server.Commands
{
	public static class GenerateMobilesList
	{
		private static CsvFile csv;

		private delegate void ProcessObject(object obj);

		public static void Initialize()
		{
            CommandSystem.Register("GenMobilesList", AccessLevel.GameMaster, new CommandEventHandler(GenMobilesList_OnCommand));

        }
        private static void GenMobilesList_OnCommand( CommandEventArgs e )
        {
            csv = new CsvFile();
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => t.IsClass && t.Namespace == "Server.Mobiles" && typeof(Mobiles.BaseCreature).IsAssignableFrom(t) )
                .ToList()
                .ForEach(t => ConsumeType(t, HandleBaseCreatureRegionEditor));
            csv.Write("MobilesList.txt");
        }

        private static void HandleBaseCreatureRegionEditor( object obj )
        {
            Mobiles.BaseCreature creature = obj as Mobiles.BaseCreature;
            Mobiles.BaseVendor vendor = obj as Mobiles.BaseVendor;
            Mobiles.BaseEscortable escort  =  obj as Mobiles.BaseEscortable;
            Mobiles.BaseFamiliar familiar  =  obj as Mobiles.BaseFamiliar;
            Mobiles.BaseGuard guard  =  obj as Mobiles.BaseGuard;
            Mobiles.BaseGuildmaster guildmaster  =  obj as Mobiles.BaseGuildmaster;
            Mobiles.BaseSummoned summoned  =  obj as Mobiles.BaseSummoned;
            Mobiles.BaseTurnInMobile turninmobile  =  obj as Mobiles.BaseTurnInMobile;
            Mobiles.BaseHire hireable  =  obj as Mobiles.BaseHire;


            if ( creature == null )
                return;
            else if ( creature == vendor || creature == escort || creature == familiar
                || guard != null || creature == guildmaster || creature == summoned
                || creature == turninmobile || creature == hireable )
            {
                return;
            }

            csv.AddRow();
            csv.AddValue("", creature.GetType().Name);
        }

        private static void ConsumeType(Type t, ProcessObject proc)
		{
			ConstructorInfo ctor = t.GetConstructor(new Type[] { });
			if (ctor == null)
				return;

			object obj;
			try
			{
				obj = ctor.Invoke(new object[] { });
			}
			catch (Exception)
			{
				return;
			}

			if (obj == null)
				return;

			proc(obj);
		}

		private class CsvFile
		{
			private List<Dictionary<String, String>> rows = new List<Dictionary<string, string>>();
			private Dictionary<String, String> currentRow = null;
			private HashSet<String> headerSet = new HashSet<string>();
			private List<String> allHeaders = new List<string>();

			public CsvFile()
			{
			}

			public void AddRow()
			{
				currentRow = new Dictionary<String, String>();
				rows.Add(currentRow);
			}

			public void AddValue(String name, object value)
			{
				if (name == null)
					return;

				String v = "";
				if (value != null)
					v = value.ToString();

				currentRow.Add(name, v.ToString());
				if (headerSet.Contains(name))
					return;
				headerSet.Add(name);
				allHeaders.Add(name);
			}

			public void Write(String path)
			{
				StreamWriter outf = new StreamWriter(path);
				foreach(String header in allHeaders)
				{
                    
					outf.Write(String.Format("{0}", header));
				}
				outf.WriteLine("");

				foreach(Dictionary<String, String> row in rows)
				{
					foreach (String header in allHeaders)
					{
						String value = "";
						row.TryGetValue(header, out value);
						outf.Write(String.Format("{0}", value));
					}
					outf.WriteLine("");
				}

				outf.Close();
			}
		}
	}
}
