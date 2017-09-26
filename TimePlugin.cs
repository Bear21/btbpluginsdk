using System;
using System.IO;
using btbcomm;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace btbplugin
{
   [Serializable()]
   public class TimePlugin : ICommand
   {
      private int count = 1;
      public string Name
      {
         get
         {
            return "timeplugin";
         }
      }

      public string Help
      {
         get
         {
            return "Displays the streamer time. Set time with `!time [\"timezone\"]` See timezones with `!time options` ";
         }
      }

      public string[] Command
      {
         get
         {
            string[] commands = { "!time" };
            return commands;
         }
      }

      public TimeData timeData;

      public PluginResponse ValidateParameters(string command, string[] args)
      {
         return PluginResponse.Accept;
      }

      public bool Execute(out string message, string command, User usr, string[] args)
      {
         DateTime now = DateTime.UtcNow;
         //"AUS Eastern Standard Time"
         if (usr.admin)
         {
            if (args[0].Equals("options"))
            {
               message = "Options are";
               foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones())
                  message += ", " + z.Id;
               return true;
            }
            if (args.Length > 0)
            {
               timeData.tz = args[0];
            }
         }
         TimeZoneInfo tz;
         if (timeData.tz == null)
            tz = TimeZoneInfo.Utc;
         else
            tz = TimeZoneInfo.FindSystemTimeZoneById(timeData.tz);
         now = TimeZoneInfo.ConvertTime(now, tz);
         message = String.Format("My Time is {0:00}:{1:00}:{2:00}", now.Hour, now.Minute, now.Second);
         return true;
      }

      public byte[] Save()
      {
         try
         {
            using (MemoryStream ms = new MemoryStream(1048576)) // 1MB
            {
               var pew = new DataContractJsonSerializer(timeData.GetType());
               pew.WriteObject(ms, timeData);
               return ms.ToArray();
            }
         }
         catch (Exception)
         {
            return new byte[0];
         }
      }

      public void Load(byte[] data, IBtbInterface Ignored)
      {
         try
         {
            if (data != null)
            {
               using (MemoryStream ms = new MemoryStream(data))
               {
                  DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(TimeData));
                  timeData = ser.ReadObject(ms) as TimeData;
               }
            }
            if (timeData == null)
            {
               // Import
               timeData = new TimeData();
            }
         }
         catch (Exception)
         {
            timeData = new TimeData();
         }

      }

      [DataContract]
      public class TimeData
      {
         private ExtensionDataObject extensionDataObjectValue;
         public ExtensionDataObject ExtensionData
         {
            get
            {
               return extensionDataObjectValue;
            }
            set
            {
               extensionDataObjectValue = value;
            }
         }

         [DataMember(Name = "time_zone")]
         public string tz;

         public TimeData()
         {
         }
      }
   }
}
