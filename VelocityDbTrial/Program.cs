using System;
using SoundExplorersDatabase.Data;

namespace SoundExplorersDatabase {
  class Program {
    static void Main() {
      Console.WriteLine("Initialising.");
      using (DatabaseSchema schema = new DatabaseSchema(expectedVersionNo: 2)) {
        try {
          Console.WriteLine("Checking schema.");
          if (schema.IsUpToDate) {
            Console.WriteLine(
              "Schema already up to date: version = {0}.", schema.VersionNo);
          } else {
            Console.WriteLine("Updating schema.");
            schema.Update();
            Console.WriteLine("Schema updated: version = {0}.", schema.VersionNo);
          }
        } catch (Exception ex) {
          Console.WriteLine(ex);
        }
      }
      Console.WriteLine("Press ENTER to finish.");
      Console.Read();
    }
  }
}
