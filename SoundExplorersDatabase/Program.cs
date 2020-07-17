using System;
using System.Diagnostics;
using SoundExplorersDatabase.Data;
using VelocityDb.Session;

namespace SoundExplorersDatabase {
  class Program {
    const string DatabaseFolderPath = 
      @"E:\Simon\OneDrive\Documents\Software\Sound Explorers Audio Archive\Database";
    static void Main() {
      Console.WriteLine("Initialising.");
      const string name = "Pyramid Club";
      Trace.Listeners.Add(new ConsoleTraceListener());
      using (var session = new SessionNoServer(DatabaseFolderPath)) {
        try {
          session.TraceIndexUsage = true;
          var location = new Location { Name = name };
          session.BeginUpdate();
          session.Persist(location);
          session.Commit();
          session.BeginRead();
          location = Location.Read(name, session);
          session.Commit();
          session.BeginUpdate();
          session.Unpersist(location);
          session.Commit();
        }
        catch (Exception e) {
          session.Abort();
          Console.WriteLine(e);
        }
      }
      Console.WriteLine("Press ENTER to finish.");
      Console.Read();
    }
  }
}
