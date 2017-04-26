using System;
using System.Collections.Generic;
using btbcomm;
using System.Runtime.Serialization.Formatters.Binary;

namespace btbplugin
{
   public class btbCommand : ICommand
   {
      private Dictionary<Int64, UInt64> userPoints;
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
            string[] commands = { "!points", "!top10points", "!top10 points" };
            return commands;
         }
      }

      public PluginResponse ValidateParameters(string[] args)
      {
         return PluginResponse.Accept;
      }

      public bool Execute(out string message, User usr, string[] args)
      {
         message = "";
         return true;
      }

      public byte[] Save()
      {
         return new Byte[0];
      }

      public void Load(byte[] data, IBtbInterface ignored)
      {
      }
   }
}
