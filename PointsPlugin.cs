using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Json;
using btbcomm;


namespace btbplugin
{
   public class btbCommand : ICommand
   {
      private Dictionary<Int64, UInt64> userPoints;
      private string[] pointCommands = { "!points", "!bucks", "!dolleriedoos", "!dollarydoos" };
      private string[] topCommands = { "!top10points", "!top10 points" };
      private string[] allCommands;
      private IBtbInterface btbInterface;
      public string Name
      {
         get
         {
            return "pointsplugin";
         }
      }

      public string Help
      {
         get
         {
            return "Give points every minute to viewers while the stream is live!";
         }
      }

      public string[] Command
      {
         get
         {
            if (allCommands == null)
            {
               allCommands = pointCommands.Concat(topCommands).ToArray();
            }
            return allCommands;
         }
      }

      public PluginResponse ValidateParameters(string command, string[] args)
      {
         return PluginResponse.Accept;
      }

      public bool Execute(out string message, string command, User usr, string[] args)
      {
         if (pointCommands.Contains(command))
         {
            message = String.Format("{0} has {1} points!", usr.displayName, userPoints[usr.id]);
         }
         else
         {
            var list = (from t in userPoints
                        orderby t.Value descending
                        select t).Take(10);

            string total = "The people with the most points are: ";
            int it = 1;
            foreach (var p in list)
            {
               total += (it++).ToString() + "." + btbInterface.UserLookup(p.Key) + " " + p.Value + ". ";
            }
            message = total;
         }
         return true;
      }

      public byte[] Save()
      {
         using (MemoryStream ms = new MemoryStream(1048576)) // 1MB
         {
            var pew = new DataContractJsonSerializer(userPoints.GetType());
            pew.WriteObject(ms, userPoints);
            return ms.ToArray();
         }
      }

      public void Load(byte[] data, IBtbInterface notIgnored)
      {
         using (MemoryStream ms = new MemoryStream(data))
         {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(userPoints.GetType());
            userPoints = ser.ReadObject(ms) as Dictionary<Int64, UInt64>;
         }
         btbInterface = notIgnored;
      }
   }
}
