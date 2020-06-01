using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Models
{
    public class LaunchJob
    {
        public LaunchJob()
        {
            Path = "C:\\Users\\Orlando Falcone\\source\\repos\\ConsoleApp\\bin\\Release\\netcoreapp3.1\\publish\\ConsoleApp.exe";
        }

        public int Id { get; set; }
        public string Orario { get; set; }
        public string Path { get; set; }
    }
}
